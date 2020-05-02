using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace GamesProcess.Models
{
    public class TotalSearchParameters
    {
        // DBAccess Fields
        public int ID { get; set; }
        public DateTime TimeSearched { get; set; }

        public void Reset()
        {
            NoOfWeeksToDisplay = 5;
            GameSelection = 0;
            GroupSelection = 0;

            TotalValue = 1;
            TotalValueType = 1;
            TotalValueLocation = 0;
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

        [Range(1,450)]
        public int TotalValue { get; set; }
        [Range(1, 2)]
        public int TotalValueType { get; set; } = 1;
        public int TotalValueLocation { get; set; }

        [NotMapped]
        public List<SelectItem> SelectTotalValueType { get; } = new List<SelectItem>
        {
            new SelectItem{ID=1, Name = "5 Values"},
            new SelectItem{ID=2, Name = "2 Values"}
        };
        [NotMapped]
        public List<SelectItem> SelectLocation { get; } = new List<SelectItem>{
            new SelectItem {ID = 0, Name = "Winning and Machine"},
            new SelectItem {ID = 1, Name = "Winning"},
            new SelectItem {ID = 2, Name = "Machine"}
        };
    }

}
