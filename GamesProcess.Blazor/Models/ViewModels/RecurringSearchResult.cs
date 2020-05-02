using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GamesProcess.Models.ViewModels
{
    public class RecurringSearchResult
    {
        public int ID { get; set; }
        public List<Event> Events { get; set; }
        public int RecurringValue { get; set; }
        public int ReferenceEventID { get; set; }
        public int Value2EventID { get; set; }
        public int Value3EventID { get; set; }
    }
}
