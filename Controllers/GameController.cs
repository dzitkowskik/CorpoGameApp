using CorpoGameApp.Models;
using CorpoGameApp.Services;
using CorpoGameApp.ViewModels.Game;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using System;

namespace CorpoGameApp.Controllers
{
    [Authorize]
    public class GameController : Controller
    {
        private const int TEAM_NUMBER = 2;
        private const int TEAM_SIZE = 2;

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
            var gameViewModel = new GameViewModel() {
                NewGame = GetNewGameViewModel(),
                CurrentPlayer = GetCurrentPlayerViewModel()
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

        [HttpPost]
        public IActionResult Create(CreateGameViewModel model)
        {
            var newGameViewModel = GetNewGameViewModel();

            if(model.SecondTeam.Count != TEAM_SIZE || model.FirstTeam.Count != TEAM_SIZE)
                newGameViewModel.Error = "Teams must have equal size of 2";

            if(ModelState.IsValid && string.IsNullOrEmpty(newGameViewModel.Error))
            {
                try
                {
                   _gameServices.CreateGame(new [] {model.FirstTeam, model.SecondTeam});
                    return PartialView("Partial/NewGame", newGameViewModel);
                }
                catch(Exception ex)
                {
                    newGameViewModel.Error = ex.Message;
                }
            }

            return PartialView("Partial/NewGame", newGameViewModel);
        }

        [HttpPost]
        public IActionResult Finish(CurrentGameViewModel model)
        {
            if(ModelState.IsValid)
            {
               
            }
            else return PartialView("", model);

            return RedirectToAction("Index");
        }

        private NewGameViewModel GetNewGameViewModel()
        {
            var players = _playerServices.GetAllPlayers();
            var newGameViewModel = new NewGameViewModel(players, TEAM_NUMBER, TEAM_SIZE);
            return newGameViewModel;
        }

        private PlayerViewModel GetCurrentPlayerViewModel()
        {
            var players = _playerServices.GetAllPlayers();            
            var currentPlayerId = _userManager.GetUserId(User);
            var currentPlayer = players.FirstOrDefault(t => t.User.Id == currentPlayerId);
            return new PlayerViewModel(currentPlayer);
        }
    }
}