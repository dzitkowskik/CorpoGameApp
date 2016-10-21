using CorpoGameApp.Services;
using CorpoGameApp.ViewModels.Game;
using Microsoft.AspNetCore.Mvc;

namespace CorpoGameApp.Controllers
{
    public class GameController : Controller
    {
        private readonly IGameServices _gameServices;

        public GameController(IGameServices gameServices)
        {
            _gameServices = gameServices;
        }

        public ActionResult Index()
        {
            var players = _gameServices.GetAllPlayers();
            var viewModel = new NewGameViewModel(players, 2, 2);
            return View(viewModel);
        }
    }
}