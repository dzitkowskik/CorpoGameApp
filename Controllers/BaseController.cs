using System.Threading.Tasks;
using CorpoGameApp.Models;
using CorpoGameApp.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CorpoGameApp.Controllers
{
    public class BaseController : Controller
    {
        protected readonly UserManager<ApplicationUser> _userManager;
        protected readonly IPlayerServices _playerServices;

        protected BaseController(
            UserManager<ApplicationUser> userManager, 
            IPlayerServices playerServices)
        {
            _playerServices = playerServices;
            _userManager = userManager;
        }

        protected async Task<Player> GetCurrentPlayer()
        {
            var user = await GetCurrentUserAsync();
            if (user == null)
                return null;
            var player = _playerServices.GetUserPlayer(user.Id);
            return player;
        }

        private Task<ApplicationUser> GetCurrentUserAsync()
        {
            return _userManager.GetUserAsync(HttpContext.User);
        }
    }
}