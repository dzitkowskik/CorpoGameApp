using System;
using CorpoGameApp.Data;
using CorpoGameApp.Logic;
using CorpoGameApp.Models;
using CorpoGameApp.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Microsoft.EntityFrameworkCore;
using CorpoGameApp.Properties;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using CorpoGameApp.Controllers;
using CorpoGameApp.ViewModels.Game;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;

namespace CorpoGameApp.Test
{
    [TestClass]
    public class GameControllerTest : IDisposable
    {
        private ApplicationDbContext Context { get; }

        private UserManager<ApplicationUser> UserManager { get; }

        public GameControllerTest()
        {
            var services = new ServiceCollection();
            services
                .AddEntityFrameworkInMemoryDatabase()
                .AddDbContext<ApplicationDbContext>(c => c.UseInMemoryDatabase("test"))
                .AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            var serviceProvider = services.BuildServiceProvider();
            Context = serviceProvider.GetRequiredService<ApplicationDbContext>();
            UserManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        }       

        [TestMethod]
        public void Finish_EndGameError_ShouldAddModelError()
        {
            var gameLogicMock = new Mock<IGameLogic>();
            gameLogicMock
                .Setup(t => t.EndGame(It.IsAny<int>(), It.IsAny<int?>()))
                .Returns(false);
            var playerServicesMock = new Mock<IPlayerServices>();
            var statisticsServicesMock = new Mock<IStatisticsServices>();
            var optionsMock = new Mock<IOptions<GameSettings>>();
            var loggerMock = new Mock<ILogger<GameController>>();

            var gameController = new GameController(
                gameLogicMock.Object,
                playerServicesMock.Object,
                statisticsServicesMock.Object,
                UserManager,
                optionsMock.Object,
                loggerMock.Object);

            var result = gameController.Finish(new FinishGameViewModel(){GameId = 1234});

            // Assert
            result.Should().BeOfType<PartialViewResult>();
            var viewResult = result as PartialViewResult;
            viewResult.Should().NotBeNull();
            viewResult.ViewData.Model.Should().NotBeNull();
            viewResult.ViewData.Model.Should().BeAssignableTo<FinishGameViewModel>();
            var model = viewResult.ViewData.Model as FinishGameViewModel;
            model.GameId.Should().Be(1234);

            viewResult.ViewData.ModelState.ErrorCount.Should().Be(1);
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    UserManager.Dispose();
                    Context.Dispose();
                }

                disposedValue = true;
            }
        }
        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
