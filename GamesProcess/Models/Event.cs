using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace GamesProcess.Models
{
    public class Event
    {
        public Event()
        {
            // create array values that will be used in getting the array data that will parsed to string format
            arrWinning = new int[5];
            arrMachine = new int[5];
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int EventID { get; set; }

        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Display(Name = "Event Number")]
        public int EventNumber { get; set; }

        //[ForeignKey("Game")]
        public int GameID { get; set; }
        //public Game Game { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }

        [NotMapped]
        private int[] arrWinning;
        [NotMapped]
        public int[] Winning
        {
            get { return arrWinning; }
            set { arrWinning = value; }
        }

        public string WinningValues
        {
            get => String.Join(",", arrWinning.Select(p => p.ToString()));
            set => arrWinning = value.Split(',').Select(int.Parse).ToArray();
        }

        [NotMapped]
        private int[] arrMachine;
        [NotMapped]
        public int[] Machine
        {
            get { return arrMachine; }
            set { arrMachine = value; }
        }
        public string MachineValues
        {
            get => String.Join(",", arrMachine.Select(p => p.ToString()));
            set => arrMachine = value.Split(',').Select(int.Parse).ToArray();
        }
    }
}