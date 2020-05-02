using GamesProcess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GamesProcess.Models.ViewModels
{
    public class TurningSearchResultViewModel
    {
        public TurningSearchParameters SearchParameters { get; set; }
        public List<TurningSearchResult> SearchResults { get; set; }
    }
}
