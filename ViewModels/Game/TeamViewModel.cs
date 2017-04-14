using System.Collections.Generic;

namespace CorpoGameApp.ViewModels.Game
{
    public class TeamViewModel 
    {
        public int Id { get; set; }
        public int Size { get; set; }
        public IList<string> Players { get; set; }
    }
}