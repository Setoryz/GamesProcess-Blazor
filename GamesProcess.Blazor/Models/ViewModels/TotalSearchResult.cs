using System.Collections.Generic;

namespace GamesProcess.Models.ViewModels
{
    public class TotalSearchResult
    {
        public int ID { get; set; }
        public List<Event> Events { get; set; }
        public int TotalValueEventID { get; set; }
    }
}
