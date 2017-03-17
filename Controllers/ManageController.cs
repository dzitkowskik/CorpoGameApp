using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using CorpoGameApp.Models;
using CorpoGameApp.Services;
using CorpoGameApp.ViewModels.Manage;
using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Net.Http.Headers;
using System.Linq;
using CorpoGameApp.Controllers.Enums;
using System.Collections.Generic;
using Hangfire;

namespace CorpoGameApp.Controllers
{
    [Authorize]
    public partial class ManageController : BaseController
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger _logger;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IEmailServices _emailServices;

        private const string AVATAR_FOLDER_PATH = "images/avatars";
        private const long MAX_AVATAR_FILE_SIZE_IN_BYTES = 2097152; // 2MB
        private static readonly string[] ALLOWED_AVATAR_FILE_EXTENSIONS = { "jpg", "png" };

        public ManageController(
            IHostingEnvironment hostingEnvironment,
            IPlayerServices playerServices,
            IEmailServices emailServices,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILoggerFactory loggerFactory) : base(userManager, playerServices)
        {
            _signInManager = signInManager;
            _logger = loggerFactory.CreateLogger<ManageController>();
            _hostingEnvironment = hostingEnvironment;
            _emailServices = emailServices;
        }

        [HttpGet]
        public async Task<IActionResult> Index(ManageMessageType? message = null)
        {
            ViewData["StatusMessage"] = GetStatusMessageText(message);
            ViewData["IsStatusError"] = IsStatusMessageAnError(message);

            var player = await GetCurrentPlayer();
            if (player == null) 
                return NotFound("Cannot find a player");

            var model = new ManageAccountViewModel
            {
                Name = player.Name,
                Surname = player.Surname,
                Email = player.User.Email,
                HasPassword = await _userManager.HasPasswordAsync(player.User),
                Avatar = string.IsNullOrEmpty(player.Avatar) 
                    ? "/images/blank_avatar.png" 
                    : player.Avatar
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateAvatar(IFormFile file)
        {
            var player = await GetCurrentPlayer();
            if (player == null)
                return NotFound("Cannot find a player");

            var rootPath = _hostingEnvironment.WebRootPath;
            var basePath = Path.Combine(rootPath, AVATAR_FOLDER_PATH);
            if (!Directory.Exists(basePath))
                Directory.CreateDirectory(basePath);

            try
            {
                if(!IsTheFileCompatible(file))
                    return RedirectToAction(nameof(Index), new { Message = ManageMessageType.FileNotCompatible });

                // Save uploaded file
                var fileName = GetFileName(file);
                var fileAbsolutePath = Path.Combine(basePath, fileName);
                if (file.Length > 0)
                    using (var stream = new FileStream(fileAbsolutePath, FileMode.Create))
                        await file.CopyToAsync(stream);

                // Remove player's old avatar file
                var oldFileAbsolutePath = Path.Combine(rootPath, player.Avatar);
                System.IO.File.Delete(oldFileAbsolutePath);

                // Update file path for player
                var fileRelativePath = Path.Combine(AVATAR_FOLDER_PATH, fileName);
                player.Avatar = fileRelativePath;
                _playerServices.UpdatePlayer(player);
                return RedirectToAction(nameof(Index), new { Message = ManageMessageType.UpdatePlayerSuccess });
            }
            catch(Exception ex)
            {
                var errorMessage = "Cannot update avatar for player";
                _logger.LogError(null, ex, errorMessage);
                ModelState.AddModelError(string.Empty, errorMessage);
                return RedirectToAction(nameof(Index), new { Message = ManageMessageType.ErrorUpdatingAvatar });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(ManageAccountViewModel model)
        {
            if(!ModelState.IsValid)
            {
                return View(model);
            }

            var player = await GetCurrentPlayer();
            if (player == null) 
                return NotFound("Cannot find a player");
            
            try
            {
                player.Name = model.Name;
                player.Surname = model.Surname;
                player.User.Email = model.Email;
                _playerServices.UpdatePlayer(player);
                return RedirectToAction(nameof(Index), new { Message = ManageMessageType.UpdatePlayerSuccess });
            }
            catch(Exception ex)
            {
                _logger.LogError(null, ex, "Cannot update player");
                ModelState.AddModelError(string.Empty, "Cannot update player");
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var player = await GetCurrentPlayer();
            if (player == null) 
                return NotFound("Cannot find a player");
            var user = player.User;

            var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);

                BackgroundJob.Enqueue<IEmailServices>(es =>
                    es.SendEmail(
                        "CorpoGameApp - password changed", 
                        "Your password has changed", 
                        new List<string> { user.Email }));
                        
                _logger.LogInformation(3, "User changed their password successfully.");
                return RedirectToAction(nameof(Index), new { Message = ManageMessageType.ChangePasswordSuccess });
            }
            AddErrors(result);
            return View(model);
        }

        #region Helpers

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        private string GetStatusMessageText(ManageMessageType? message)
        {
            return message == ManageMessageType.ChangePasswordSuccess ? "Your password has been changed."
                : message == ManageMessageType.Error ? "An error has occurred."
                : message == ManageMessageType.UpdatePlayerSuccess ? "Player updated successfully."
                : message == ManageMessageType.ErrorUpdatingAvatar ? "Avatar update failed."
                : message == ManageMessageType.FileNotCompatible ? "Not compatible file (file must be max 2MB and in jpg or png format."
                : string.Empty;
        }

        private bool IsStatusMessageAnError(ManageMessageType? message)
        {
            switch(message)
            {
                case ManageMessageType.Error:
                case ManageMessageType.ErrorUpdatingAvatar:
                case ManageMessageType.FileNotCompatible:
                    return true;
                default:
                    return false;
            }
        }

        private bool IsTheFileCompatible(IFormFile file)
        {
            var contentHeader = ContentDispositionHeaderValue
                .Parse(file.ContentDisposition);

            if(file.Length > MAX_AVATAR_FILE_SIZE_IN_BYTES) return false;
            
            var extension = contentHeader.FileName
                .Replace("/", "").Replace("\"", "")
                .Split('.').Last();

            var allowedExtension = ALLOWED_AVATAR_FILE_EXTENSIONS.Contains(extension);
            if(!allowedExtension) return false;

            return true;
        }

        private static string GetFileName(IFormFile file)
        {
            var fileName = ContentDispositionHeaderValue
                .Parse(file.ContentDisposition)
                .FileName
                .Trim('"');

            return $"{Guid.NewGuid()}_{fileName}";
        }
        #endregion
    }
}
