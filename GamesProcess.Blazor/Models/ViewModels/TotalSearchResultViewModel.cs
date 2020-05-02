using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GamesProcess.Models.ViewModels
{
    public class TotalSearchResultViewModel
    {
        public TotalSearchParameters SearchParameters { get; set; }
        public List<TotalSearchResult> SearchResults { get; set; }
    }
}
