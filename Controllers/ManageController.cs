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

namespace CorpoGameApp.Controllers
{
    [Authorize]
    public class ManageController : BaseController
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly ISmsSender _smsSender;
        private readonly ILogger _logger;
        private readonly IHostingEnvironment _hostingEnvironment;

        private const string AVATAR_FOLDER_PATH = "images/avatars";
        private const long MAX_AVATAR_FILE_SIZE_IN_BYTES = 2097152; // 2MB
        private static readonly string[] ALLOWED_AVATAR_FILE_EXTENSIONS = { "jpg", "png" };

        public ManageController(
            IHostingEnvironment hostingEnvironment,
            IPlayerServices playerServices,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IEmailSender emailSender,
            ISmsSender smsSender,
            ILoggerFactory loggerFactory) : base(userManager, playerServices)
        {
            _signInManager = signInManager;
            _emailSender = emailSender;
            _smsSender = smsSender;
            _logger = loggerFactory.CreateLogger<ManageController>();
            _hostingEnvironment = hostingEnvironment;
        }

        [HttpGet]
        public async Task<IActionResult> Index(ManageMessageId? message = null)
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
                    return RedirectToAction(nameof(Index), new { Message = ManageMessageId.FileNotCompatible });

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
                return RedirectToAction(nameof(Index), new { Message = ManageMessageId.UpdatePlayerSuccess });
            }
            catch(Exception ex)
            {
                var errorMessage = "Cannot update avatar for player";
                _logger.LogError(null, ex, errorMessage);
                ModelState.AddModelError(string.Empty, errorMessage);
                return RedirectToAction(nameof(Index), new { Message = ManageMessageId.ErrorUpdatingAvatar });
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
                return RedirectToAction(nameof(Index), new { Message = ManageMessageId.UpdatePlayerSuccess });
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
                _logger.LogInformation(3, "User changed their password successfully.");
                return RedirectToAction(nameof(Index), new { Message = ManageMessageId.ChangePasswordSuccess });
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

        public enum ManageMessageId
        {
            UpdatePlayerSuccess,
            ChangePasswordSuccess,
            Error,
            ErrorUpdatingAvatar,
            FileNotCompatible
        }

        private string GetStatusMessageText(ManageMessageId? message)
        {
            return message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
                : message == ManageMessageId.Error ? "An error has occurred."
                : message == ManageMessageId.UpdatePlayerSuccess ? "Player updated successfully."
                : message == ManageMessageId.ErrorUpdatingAvatar ? "Avatar update failed."
                : message == ManageMessageId.FileNotCompatible ? "Not compatible file (file must be max 2MB and in jpg or png format."
                : string.Empty;
        }

        private bool IsStatusMessageAnError(ManageMessageId? message)
        {
            switch(message)
            {
                case ManageMessageId.Error:
                case ManageMessageId.ErrorUpdatingAvatar:
                case ManageMessageId.FileNotCompatible:
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
