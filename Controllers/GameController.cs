using CorpoGameApp.Models;
using CorpoGameApp.Services;
using CorpoGameApp.ViewModels.Game;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using System;
using Microsoft.Extensions.Options;
using CorpoGameApp.Properties;
using System.Collections.Generic;

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
        private readonly IOptions<GameSettings> _options;
        private readonly IStatisticsServices _statiticsServices;

        public GameController(
            IGameServices gameServices, 
            IPlayerServices playerServices,
            IStatisticsServices statisticsServices,
            UserManager<ApplicationUser> userManager,
            IOptions<GameSettings> options)
        {
            _gameServices = gameServices;
            _userManager = userManager;
            _playerServices = playerServices;
            _statiticsServices = statisticsServices;
            _options = options;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var gameViewModel = new GameViewModel() {
                NewGame = GetNewGameViewModel(),
                CurrentPlayer = GetCurrentPlayerViewModel(),
                CurrentGame = GetCurrentGameViewModel(),
                Statistics = new List<StatisticsViewModel>()
                {
                    _statiticsServices.GetTopPlayersStatistic(),
                    _statiticsServices.GetLastGamesStatistic()
                }
            };
            
            return View(gameViewModel);
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
               _gameServices.EndGame(model.GameId, model.WinningTeam);
            }
            else return PartialView("Partial/CurrentGame", model);

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

        private CurrentGameViewModel GetCurrentGameViewModel()
        {
            var currentUserId = _userManager.GetUserId(User);
            var player = _playerServices.GetUserPlayer(currentUserId);

            var currentGame = _gameServices.GetCurrentGame();
            if(currentGame != null && !currentGame.Players.Any(t => t.PlayerId == player.Id))
            {
                var timeLeft = currentGame.StartTime.AddMinutes(_options.Value.GameDuration) - DateTime.Now;

                // if time ledt is negative, finish the game
                // players must fill the result later (at least one)                
                if(timeLeft.TotalMilliseconds < 0) 
                    _gameServices.EndGame(currentGame.Id, null);
                else
                    return new CurrentGameViewModel()
                    {
                        LastGameStart = currentGame.StartTime,
                        TimeLeft = timeLeft,
                        CurrentGameLasts = true,
                        GameId = currentGame.Id
                    };
            }

            // get last game of a player and check if it has a result
            var lastGame = _gameServices.GetPlayerLastGame(player.Id);
            if(lastGame != null && lastGame.WinnersTeam == null)
            {
                var teams = lastGame.Players
                    .Where(p => p.Player != null)
                    .GroupBy(t => t.Team, v => v.Player.ToString())
                    .ToDictionary(x => x.Key, y => (IList<string>)y.ToList());
                return new CurrentGameViewModel()
                {
                    GameId = lastGame.Id,
                    CurrentGameLasts = false,
                    Teams = teams
                };
            }

            return null;
        }
    }
}