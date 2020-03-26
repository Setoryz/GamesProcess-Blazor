using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GamesProcess2.Data;
using GamesProcess2.Libs;
using GamesProcess2.Models;
using GamesProcess2.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace GamesProcess2.Controllers
{
    public class SearchController : Controller
    {
        private readonly GameContext _context;

        public SearchController(GameContext context)
        {
            _context = context;
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }

        // GET: Advanced Search
        public async Task<IActionResult> Search(SearchParameters searchParameters)
        {
            searchParameters.GamesList = (from games in _context.Games select games).ToList();
            searchParameters.GamesList.Insert(0, new Game { ID = 0, Name = "Select Groups" });
            searchParameters.GamesGroups = (from groups in _context.GamesClass select groups).ToList();
            searchParameters.GamesGroups.Insert(0, new GamesClass { ID = 0, Name = "Select All" });

            SearchResultViewModel searchResults = new SearchResultViewModel() { SearchParameters = searchParameters };

            if (searchParameters.NoOfSearchValues == null)
            {
                return await Task.Run(() => View(searchResults));
            }

            List<AdvancedSearchResult> results = new List<AdvancedSearchResult>();

            int[] groupGamesToSearchFrom = (searchParameters.GameSelection == 0) ? (from games in _context.Games.Where(s => s.GamesClassID == searchParameters.GroupSelection) select games.ID).ToArray() : null;

            switch (searchParameters.NoOfSearchValues)
            {
                case 1:
                    results = await Task.Run(() => AdvSearch.FindResults(_context,
                        searchParameters.NoOfWeeksToDisplay,
                        (int)searchParameters.ReferenceValue,
                        searchParameters.ReferenceLocation,
                        searchParameters.ReferencePosition,
                        searchParameters.GameSelection,
                        groupGamesToSearchFrom).ToList());
                    break;
                case 2:
                    results = await Task.Run(() => AdvSearch.FindResults(_context,
                        searchParameters.NoOfWeeksToDisplay,
                        (int)searchParameters.ReferenceValue,
                        searchParameters.ReferenceLocation,
                        searchParameters.ReferencePosition,
                        searchParameters.GameSelection,
                        groupGamesToSearchFrom,
                        (int)searchParameters.Value2,
                        searchParameters.Value2WeekSelect,
                        searchParameters.Value2Week,
                        searchParameters.Value2Location,
                        searchParameters.Value2Position).ToList());
                    break;
                case 3:
                    results = await Task.Run(() => AdvSearch.FindResults(_context,
                        searchParameters.NoOfWeeksToDisplay,
                        (int)searchParameters.ReferenceValue,
                        searchParameters.ReferenceLocation,
                        searchParameters.ReferencePosition,
                        searchParameters.GameSelection,
                        groupGamesToSearchFrom,
                        (int)searchParameters.Value2,
                        searchParameters.Value2WeekSelect,
                        searchParameters.Value2Week,
                        searchParameters.Value2Location,
                        searchParameters.Value2Position,
                        (int)searchParameters.Value3,
                        searchParameters.Value3WeekSelect,
                        searchParameters.Value3Week,
                        searchParameters.Value3Location,
                        searchParameters.Value3Position).ToList());
                    break;
                default:
                    results = null;
                    break;
            }

            searchResults.SearchResults = results;

            return await Task.Run(() => View(searchResults));
        }
    }
}
