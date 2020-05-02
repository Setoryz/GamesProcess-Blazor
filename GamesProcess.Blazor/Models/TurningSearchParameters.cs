using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GamesProcess.Models
{
    public class TurningSearchParameters
    {
        // DBAccess Fields
        public int ID { get; set; }
        public DateTime TimeSearched { get; set; }

        /// <summary>
        /// Method to Reset Search Parameter Value
        /// </summary>
        public void Reset()
        {
            NoOfWeeksToDisplay = 5;
            GameSelection = 0;
            GroupSelection = 0;

            ReferenceValue = 1;
            ReferenceLocation = 0;
            ReferencePosition = 0;

            TurnedValueWeekSelect = 1;
            TurnedValueWeek = 0;
            TurnedValueLocation = 0;
            TurnedValuePosition = 0;
        }

        [Display(Name = "Number of Weeks to Display")]
        public int NoOfWeeksToDisplay { get; set; } = 5;
        [Display(Name = "Game Selection")]
        public int GameSelection { get; set; } = 0;
        [Display(Name = "Group Selection")]
        public int GroupSelection { get; set; } = 0;

        [NotMapped]
        public List<Game> GamesList { get; set; }
        [NotMapped]
        public List<GamesClass> GamesGroups { get; set; }

        [Display(Name = "Reference Value")]
        [Range(1,90)]
        public int ReferenceValue { get; set; }
        [Display(Name = "Where to Search Reference Value")]
        public int ReferenceLocation { get; set; } = 0;
        [Display(Name = "Reference Value Position")]
        public int ReferencePosition { get; set; } = 0;

        [Display(Name = "Turned Value")]
        public int TurnedValue
        {
            get => ((ReferenceValue % 10) * 10) + ((ReferenceValue - (ReferenceValue % 10))/10);
        }
        [Display(Name = "Use Specified Week or Range of Weeks")]
        public int TurnedValueWeekSelect { get; set; } = 1;
        [Display(Name = "Weeks Apart for Turned Value")]
        [Range(-20, 20)]
        public int TurnedValueWeek { get; set; }
        [Display(Name = "Turned Value Location")]
        public int TurnedValueLocation { get; set; }
        [Display(Name = "Turned Value Posiiton")]
        public int TurnedValuePosition { get; set; }

        [NotMapped]
        public List<SelectItem> SelectLocation { get; } = new List<SelectItem>{
            new SelectItem {ID = 0, Name = "Winning and Machine"},
            new SelectItem {ID = 1, Name = "Winning"},
            new SelectItem {ID = 2, Name = "Machine"}
        };
        [NotMapped]
        public List<SelectItem> SelectPosition { get; } = new List<SelectItem>
        {
            new SelectItem {ID = 0, Name = "Search Entire Row"},
            new SelectItem {ID = 1, Name = "1st Position"},
            new SelectItem {ID = 2, Name = "2nd Position"},
            new SelectItem {ID = 3, Name = "3rd Position"},
            new SelectItem {ID = 4, Name = "4th Position"},
            new SelectItem {ID = 5, Name = "5th Position"}
        };
        [NotMapped]
        public List<SelectItem> SelectAltValWeekSel { get; } = new List<SelectItem>
        {
            new SelectItem{ID=1, Name = "Use Specified Week"},
            new SelectItem{ID=2, Name = "Use Range of Weeks"}
        };
    }
}
