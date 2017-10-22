namespace CorpoGameApp.Properties
{
    public class GameSettings
    {
        /// <summary>
        /// Number of teams in the game
        /// </summary>
        public int TeamNumber { get; set; }

        /// <summary>
        /// Team size of the game
        /// </summary>
        public int TeamSize { get; set; }
        
        public int PointsForDraw { get; set; }
        public int PointsForWin { get; set; }
        public int PointsForLose { get; set; }

        /// <summary>
        /// Single game duration in minuter
        /// </summary>
        public int GameDuration { get; set; }
        public int NumberOfTopPlayersInStatistics { get; set; }
        public int NumberOfLastGamesInStatistics { get; set; }
    }
}