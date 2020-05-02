using System.Collections.Generic;

namespace GamesProcess.Models.ViewModels
{
    public class AdvancedSearchResultViewModel
    {
        public AdvancedSearchParameters SearchParameters { get; set; }

        // RESULT
        public List<AdvancedSearchResult> SearchResults { get; set; }
    }
}
