using System.Collections.Generic;

namespace GamesProcess2.Models.ViewModels
{
    public class SearchResultViewModel
    {
        public SearchParameters SearchParameters { get; set; }

        // RESULT
        public List<AdvancedSearchResult> SearchResults { get; set; }

    }
}
