using System.Collections.Generic;

namespace CorpoGameApp.ViewModels.Game
{
    public class StatisticsViewModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public IEnumerable<string> Headers { get; set; }
        public IList<IEnumerable<string>> Values { get; set; }
    }
}