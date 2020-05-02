using System.Collections.Generic;

namespace GamesProcess.Models.ViewModels
{
    public class TurningSearchResult
    {
        public int ID { get; set; }
        public List<Event> Events { get; set; }
        public int ReferenceEventID { get; set; }
        public int TurnedValueEventID { get; set; }
    }
}
