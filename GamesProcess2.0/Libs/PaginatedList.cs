using GamesProcess2.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GamesProcess2.Libs
{
    public class PaginatedList<T> : List<T>
    {
        public int PageIndex { get; private set; }
        public int TotalPages { get; private set; }
        public List<Game> GamesList { get; private set; }
        public List<GamesClass> GamesGroups { get; private set; }
        public int GameSelection { get; private set; }
        public int GroupSelection { get; private set; }



        public PaginatedList(List<T> items, int count, int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);

            this.AddRange(items);
        }

        public PaginatedList(List<T> items, int count, int pageIndex, int pageSize, List<Game> _gamesList, List<GamesClass> _gamesClasses)
        {

            GamesList = _gamesList;
            GamesGroups = _gamesClasses;
            PageIndex = pageIndex;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);

            this.AddRange(items);
        }

        //public int gamesID { get; set; }

        // to know if previous page is available
        public bool HasPreviousPage
        {
            get
            {
                return (PageIndex > 1);
            }
        }

        // to know if next page is available
        public bool HasNextPage
        {
            get
            {
                return (PageIndex < TotalPages);
            }
        }

        // takes page size and number and applies skip and take statement to the list
        public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, int pageIndex, int pageSize, List<Game> _gamesList, List<GamesClass> _gamesClasses)
        {
            var count = await source.CountAsync();
            var items = await source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PaginatedList<T>(items, count, pageIndex, pageSize, _gamesList, _gamesClasses);
        }

        public static PaginatedList<T> Create(IQueryable<T> source, int pageIndex, int pageSize, List<Game> _gamesList, List<GamesClass> _gamesGroups, int _gameSelection, int _groupSelection)
        {
            var count = source.Count();
            var items = source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            return new PaginatedList<T>(items, count, pageIndex, pageSize, _gamesList, _gamesGroups);
        }
    }
}
