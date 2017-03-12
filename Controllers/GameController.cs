using CorpoGameApp.Models;
using CorpoGameApp.Services;
using CorpoGameApp.ViewModels.Game;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System;
using Microsoft.Extensions.Options;
using CorpoGameApp.Properties;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using CorpoGameApp.Logic;

namespace CorpoGameApp.Controllers
{
    [Authorize]
    public class GameController : BaseController
    {
        private const int TEAM_NUMBER = 2;
        private const int TEAM_SIZE = 2;

        private readonly IStatisticsServices _statiticsServices;
        private readonly ILogger _logger;
        private readonly IEmailServices _emailServices;
        private readonly IGameLogic _gameLogic;

        public GameController(
            IGameLogic gameLogic,
            IPlayerServices playerServices,
            IStatisticsServices statisticsServices,
            UserManager<ApplicationUser> userManager,
            IOptions<GameSettings> options,
            ILoggerFactory loggerFactory) : base(userManager, playerServices)
        {
            _gameLogic = gameLogic;
            _statiticsServices = statisticsServices;
            _logger = loggerFactory.CreateLogger<GameController>();
        }

        [HttpGet]
        public async Task<IActionResult> Index(
            GameMessageType.Enum messageType = GameMessageType.Enum.None)
        {
            var message = (GameMessageType)messageType;
            ViewData["StatusMessage"] = message.Text;
            ViewData["IsStatusError"] = !message.Success;

            var player = await this.GetCurrentPlayer();

            var gameViewModel = new GameViewModel() {
                NewGame = _gameLogic.GetNewGameViewModel(),
                CurrentPlayer = await GetCurrentPlayerViewModel(),
                CurrentGame = _gameLogic.GetCurrentGameViewModel(player),
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
            var newGameViewModel = _gameLogic.GetNewGameViewModel();

            if(model.SecondTeam.Count != TEAM_SIZE || model.FirstTeam.Count != TEAM_SIZE)
                ModelState.AddModelError(nameof(model.SecondTeam), $"Teams must have equal size of {TEAM_SIZE}");

            if(ModelState.IsValid)
            {
                var resultMessage = GameMessageType.CreateGameSuccess;
                try
                {
                    var newGame = _gameLogic.CreateGame(new [] {model.FirstTeam, model.SecondTeam});
                    if(newGame != null)
                    {
                        _logger.LogInformation($"Created new game {newGame.Id}");
                    }
                    else
                    {
                        resultMessage = GameMessageType.CreateGameUnknownError;
                        _logger.LogError("Cannot create a game - reason unknown");
                    }
                }
                catch(GameAlreadyInProgressException)
                {
                    resultMessage = GameMessageType.CreateGameAlreadyInProgressError;
                    _logger.LogWarning("Cannot create game - game already in progress");
                }
                catch(Exception ex)
                {
                    _logger.LogCritical($"Error occurred while creating new game: {ex.Message} - {ex.StackTrace}");
                    throw;
                }
                return RedirectToAction("Index", resultMessage);
            }

            return PartialView("Partial/NewGame", newGameViewModel);
        }

        [HttpPost]
        public IActionResult Finish(CurrentGameViewModel model)
        {
            if(ModelState.IsValid)
            {
                if(_gameLogic.EndGame(model.GameId, model.WinningTeam))
                    return RedirectToAction("Index");
                else
                    ModelState.AddModelError(string.Empty, "Cannot finish the game");
            }
            return PartialView("Partial/CurrentGame", model);
        }

        private async Task<PlayerViewModel> GetCurrentPlayerViewModel()
        {
            var currentPlayer = await GetCurrentPlayer();
            return new PlayerViewModel(currentPlayer);
        }
    }
}