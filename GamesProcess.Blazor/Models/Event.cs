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
            //arrWinning = new int[5];
            Winning = new int[5];
            //arrMachine = new int[5];
            Machine = new int[5];
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

        //[NotMapped]
        //private int[] arrWinning;
        [NotMapped]
        public int[] Winning
        {
            get { return WinningValues.Split(',').Select(int.Parse).ToArray(); }
            set { WinningValues = String.Join(",", value.Select(p => p.ToString())); }
        }

        public string WinningValues
        {
            get;
            //get => String.Join(",", arrWinning.Select(p => p.ToString()));
            set;
            //set => arrWinning = value.Split(',').Select(int.Parse).ToArray();
        }

        //[NotMapped]
        //private int[] arrMachine;
        [NotMapped]
        public int[] Machine
        {
            //get { return arrMachine; }
            get { return MachineValues.Split(',').Select(int.Parse).ToArray(); }
            //set { arrMachine = value; }
            set { MachineValues = String.Join(",", value.Select(p => p.ToString())); }
        }
        public string MachineValues
        {
            //get => String.Join(",", arrMachine.Select(p => p.ToString()));
            get;
            //set => arrMachine = value.Split(',').Select(int.Parse).ToArray();
            set;
        }
    }
}