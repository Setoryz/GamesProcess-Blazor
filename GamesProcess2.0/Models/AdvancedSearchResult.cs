using System.Collections.Generic;

namespace GamesProcess2.Models
{
    public class AdvancedSearchResult
    {
        public int ID { get; set; }
        public List<Event> Events { get; set; }
        public int ReferenceEventID { get; set; }
        public int Value2EventID { get; set; }
        public int Value3EventID { get; set; }
    }
}
