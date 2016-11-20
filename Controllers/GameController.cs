using CorpoGameApp.Models;
using CorpoGameApp.Services;
using CorpoGameApp.ViewModels.Game;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace CorpoGameApp.Controllers
{
    [Authorize]
    public class GameController : Controller
    {
        private readonly IGameServices _gameServices;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IPlayerServices _playerServices;

        public GameController(
            IGameServices gameServices, 
            IPlayerServices playerServices,
            UserManager<ApplicationUser> userManager)
        {
            _gameServices = gameServices;
            _userManager = userManager;
            _playerServices = playerServices;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var players = _playerServices.GetAllPlayers();
            var newGameViewModel = new NewGameViewModel(players, 2, 2);

            var currentPlayerId = _userManager.GetUserId(User);
            var currentPlayer = players.FirstOrDefault(t => t.User.Id == currentPlayerId);

            var gameViewModel = new GameViewModel() {
                NewGame = newGameViewModel,
                CurrentPlayer = new PlayerViewModel(currentPlayer)
            };

            return View(gameViewModel);
        }

        [HttpPost]
        public IActionResult UpdatePlayer(GameViewModel model)
        {
            if(ModelState.IsValid)
            {
                var currentPlayerId = _userManager.GetUserId(User);
                var updatedPlayer = new Player()
                {
                    User = new ApplicationUser() { Id = currentPlayerId },
                    Name = model.CurrentPlayer.Name,
                    Surname = model.CurrentPlayer.Surname
                };
                _playerServices.UpdatePlayer(updatedPlayer);
            }
            else return View(model);

            return RedirectToAction("Index");
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