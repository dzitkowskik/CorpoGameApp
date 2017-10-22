using CorpoGameApp.ViewModels.Game;

namespace CorpoGameApp.Services
{
    public interface IStatisticsServices 
    {
        StatisticsViewModel GetTopPlayersStatistic();
        StatisticsViewModel GetLastGamesStatistic();
    }
}