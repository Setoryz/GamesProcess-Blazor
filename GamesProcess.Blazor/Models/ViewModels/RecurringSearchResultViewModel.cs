using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GamesProcess.Models.ViewModels
{
    public class RecurringSearchResultViewModel
    {
        public RecurringSearchParameters SearchParameters { get; set; }
        public List<RecurringSearchResult> SearchResults { get; set; }
    }
}
