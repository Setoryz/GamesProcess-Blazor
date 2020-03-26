using System;
using System.ComponentModel.DataAnnotations;

namespace GamesProcess2.Models
{
    public class TodoItem
    {
        [Key]
        public int ID { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "Time Added")]
        public DateTime TimeAdded { get; set; }

        [Display(Name = "Is Done")]
        public bool IsDone { get; set; }

        [Required]
        [DataType(DataType.Text)]
        public string Title { get; set; }

        [DataType(DataType.MultilineText)]
        public string Details { get; set; }
    }
}
