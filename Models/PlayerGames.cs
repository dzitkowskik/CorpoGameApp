namespace CorpoGameApp.Models
{
    public class PlayerGames
    {
        public int PlayerId { get; set; }
        public Player Player { get; set;}
        public int GameId { get; set; }
        public Game Game { get; set; }
        public int Team { get; set; }
        
    }
}