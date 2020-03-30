using GamesProcess.Models;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace GamesProcess.Data.Services
{
    public class EventDbService
    {
        private ApplicationDbContext _applicationDbContext;

        public EventDbService(ApplicationDbContext applicationDb) => _applicationDbContext = applicationDb;

        public async Task<List<Event>> GetEventsAsync() => await _applicationDbContext.Events.ToListAsync();

        public List<Game> GetGames() => _applicationDbContext.Games.ToList();
        public async Task<List<Game>> GetGamesAsync() => await _applicationDbContext.Games.ToListAsync();

        public List<GamesClass> GetGameGroups() => _applicationDbContext.GamesClass.ToList();
        public async Task<List<GamesClass>> GetGameGroupsAsync() => await _applicationDbContext.GamesClass.ToListAsync();
    }
}
