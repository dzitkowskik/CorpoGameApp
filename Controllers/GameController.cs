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
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace CorpoGameApp.Controllers
{
    public sealed class GameMessageType
    {
        public enum Enum
        {
            CreateGameAlreadyInProgressError,
            CreateGameUnknownError,
            CreateGameSuccess,
            None
        }

        public static readonly SortedList<Enum, GameMessageType> Values = new SortedList<Enum, GameMessageType>();

        public static readonly GameMessageType None = new GameMessageType(
            Enum.None,
            string.Empty,
            true);
        public static readonly GameMessageType CreateGameAlreadyInProgressError = new GameMessageType(
            Enum.CreateGameAlreadyInProgressError,
            "Cannot create new game - game already in progress", 
            false);
        public static readonly GameMessageType CreateGameUnknownError = new GameMessageType(
            Enum.CreateGameUnknownError,
            "Cannot create new game - unknown error", 
            false);
        public static readonly GameMessageType CreateGameSuccess = new GameMessageType(
            Enum.CreateGameSuccess,
            "New game created successfully", 
            true);

        public readonly Enum Value;
        public readonly string Text;
        public readonly bool Success;

        private GameMessageType(Enum value, string text, bool success)
        {
            Value = value;
            Text = text;
            Success = success;
            Values.Add(value, this);
        }

        public static implicit operator GameMessageType(Enum value)
        {
            return Values[value];
        }

        public static implicit operator Enum(GameMessageType value)
        {
            return value.Value;
        }
    }

    [Authorize]
    public class GameController : BaseController
    {
        private const int TEAM_NUMBER = 2;
        private const int TEAM_SIZE = 2;

        private readonly IGameServices _gameServices;
        private readonly IOptions<GameSettings> _options;
        private readonly IStatisticsServices _statiticsServices;
        private readonly ILogger _logger;
        private readonly IEmailServices _emailServices;

        public GameController(
            IGameServices gameServices, 
            IPlayerServices playerServices,
            IStatisticsServices statisticsServices,
            UserManager<ApplicationUser> userManager,
            IOptions<GameSettings> options,
            ILoggerFactory loggerFactory) : base(userManager, playerServices)
        {
            _gameServices = gameServices;
            _statiticsServices = statisticsServices;
            _options = options;
            _logger = loggerFactory.CreateLogger<GameController>();
        }

        [HttpGet]
        public async Task<IActionResult> Index(
            GameMessageType.Enum messageType = GameMessageType.Enum.None)
        {
            var message = (GameMessageType)messageType;
            ViewData["StatusMessage"] = message.Text;
            ViewData["IsStatusError"] = !message.Success;

            var gameViewModel = new GameViewModel() {
                NewGame = GetNewGameViewModel(),
                CurrentPlayer = await GetCurrentPlayerViewModel(),
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
                ModelState.AddModelError(nameof(model.SecondTeam), $"Teams must have equal size of {TEAM_SIZE}");

            if(ModelState.IsValid)
            {
                var resultMessage = GameMessageType.CreateGameSuccess;
                try
                {
                    var newGame = _gameServices.CreateGame(new [] {model.FirstTeam, model.SecondTeam});
                    if(newGame != null)
                        _logger.LogInformation($"Created new game {newGame.Id}");
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
                if(_gameServices.EndGame(model.GameId, model.WinningTeam))
                    return RedirectToAction("Index");
                else
                    ModelState.AddModelError(string.Empty, "Cannot finish the game");
            }
            return PartialView("Partial/CurrentGame", model);
        }

        private NewGameViewModel GetNewGameViewModel()
        {
            var players = _playerServices.GetAllPlayers();
            var newGameViewModel = new NewGameViewModel(players, TEAM_NUMBER, TEAM_SIZE);
            return newGameViewModel;
        }

        private async Task<PlayerViewModel> GetCurrentPlayerViewModel()
        {
            var currentPlayer = await GetCurrentPlayer();
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