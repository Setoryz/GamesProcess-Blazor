using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GamesProcess.Models.ViewModels
{
    public class SearchParameters
    {
        // SEARCH PARAMETERS
        [Display(Name = "Number of Search Values")]
        public int NoOfSearchValues { get; set; } = 2;
        public int[] SelectNoOfSearchValues { get; set; } = new int[] { 1, 2, 3 };

        [Display(Name = "Number of Weeks to Display")]
        public int NoOfWeeksToDisplay { get; set; } = 5;

        public List<Game> GamesList { get; set; }
        public List<GamesClass> GamesGroups { get; set; }

        [Display(Name = "Game Selection")]
        public int GameSelection { get; set; } = 0;
        [Display(Name = "Group Selection")]
        public int GroupSelection { get; set; } = 0;

        // VALUE 1 PARAMETERS
        [Display(Name = "Reference Value")]
        [Range(1, 90)]
        public int ReferenceValue { get; set; }

        [Display(Name = "Where to Search Reference Value")]
        public int ReferenceLocation { get; set; }

        [Display(Name = "Reference Value Position")]
        public int ReferencePosition { get; set; } = 0;

        // VALUE 2 PARAMETERS
        [Display(Name = "2nd Search Value")]
        [Range(1, 90)]
        public int Value2 { get; set; }
        [Display(Name = "Use Specified Week or Range of Weeks")]
        public int Value2WeekSelect { get; set; }
        [Display(Name = "Weeks Apart for 2nd Search Value")]
        public int Value2Week { get; set; } // Week Apart
        [Display(Name = "Where to Search")]
        public int Value2Location { get; set; }
        [Display(Name = "Second Value Position")]
        public int Value2Position { get; set; } = 0;

        // VALUE 3 PARAMETERS
        [Display(Name = "3rd Search Value")]
        [Range(1, 90)]
        public int Value3 { get; set; }
        [Display(Name = "Use Specified Week or Range of Weeks")]
        public int Value3WeekSelect { get; set; }
        [Display(Name = "Weeks Apart for 3rd Search Value")]
        public int Value3Week { get; set; } // Week Apart
        [Display(Name = "Where to Search")]
        public int Value3Location { get; set; }
        [Display(Name = "Third Value Position")]
        public int Value3Position { get; set; } = 0;

        public List<SelectItem> SelectLocation { get; set; } = new List<SelectItem>{
            new SelectItem {ID = 0, Name = "Winning and Machine"},
            new SelectItem {ID = 1, Name = "Winning"},
            new SelectItem {ID = 2, Name = "Machine"}
            };

        public List<SelectItem> SelectPosition { get; set; } = new List<SelectItem>{
            new SelectItem {ID = 0, Name = "Search Entire Row"},
            new SelectItem {ID = 1, Name = "1st Position"},
            new SelectItem {ID = 2, Name = "2nd Position"},
            new SelectItem {ID = 3, Name = "3rd Position"},
            new SelectItem {ID = 4, Name = "4th Position"},
            new SelectItem {ID = 5, Name = "5th Position"}
            };

        public List<SelectItem> SelectAltValWeekSel { get; set; } = new List<SelectItem>
        {
            new SelectItem{ID=1, Name = "Use Specified Week"},
            new SelectItem{ID=2, Name = "Use Range of Weeks"}
        };
    }

    public class SelectItem
    {
        public int ID { get; set; }
        public string Name { get; set; }
    }
}
