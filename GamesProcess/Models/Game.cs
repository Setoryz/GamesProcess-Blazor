using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace GamesProcess.Models
{
    public class Game
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ID { get; set; }
        public string Name { get; set; }
        public ICollection<Event> Events { get; set; }

        public int GamesClassID { get; set; }
    }
}
