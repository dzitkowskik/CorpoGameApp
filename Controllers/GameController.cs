using CorpoGameApp.Models;
using CorpoGameApp.Services;
using CorpoGameApp.ViewModels.Game;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CorpoGameApp.Controllers
{
    public class GameController : Controller
    {
        private readonly IGameServices _gameServices;
        private readonly UserManager<ApplicationUser> _userManager;

        public GameController(IGameServices gameServices, UserManager<ApplicationUser> userManager)
        {
            _gameServices = gameServices;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var players = _gameServices.GetAllPlayers();
            var viewModel = new NewGameViewModel(players, 2, 2);

            return View(viewModel);
        }

        // [HttpPost]
        // public IActionResult Create(NewGameViewModel game)
        // {
        //     if(ModelState.IsValid)
        //     {
               
        //     }

        //     return View();
        // }
    }
}