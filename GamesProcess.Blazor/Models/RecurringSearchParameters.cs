using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace GamesProcess.Models
{
    public class RecurringSearchParameters
    {
        public int ID { get; set; }
        public DateTime TimeSearched { get; set; }

        public void Reset()
        {

        }

        [Display(Name = "Number of Weeks to Display")]
        public int NoOfWeeksToDisplay { get; set; } = 5;
        [Display(Name = "Game Selection")]
        public int GameSelection { get; set; } = 0;
        [Display(Name = "Game Selection")]
        public int GroupSelection { get; set; } = 0;

        [NotMapped]
        public List<Game> GamesList { get; set; }
        [NotMapped]
        public List<GamesClass> GamesGroups { get; set; }

        [Display(Name = "Recurring Value")]
        public int RecurringValue { get; set; }
        [Display(Name = "Recurring Type")]
        public int RecurringType { get; set; } = 2;
        [Display(Name = "Recurring Value Location")]
        public int RecurringValueLocation { get; set; }

        [NotMapped]
        public List<SelectItem> SelectRecurringValueType { get; } = new List<SelectItem>
        {
            new SelectItem{ID=1, Name = "2 Weeks"},
            new SelectItem{ID=2, Name = "3 Weeks"}
        };
        [NotMapped]
        public List<SelectItem> SelectLocation { get; } = new List<SelectItem>{
            new SelectItem {ID = 0, Name = "Winning and Machine"},
            new SelectItem {ID = 1, Name = "Winning"},
            new SelectItem {ID = 2, Name = "Machine"}
        };
    }
}
