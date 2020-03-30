using GamesProcess.Data;
using GamesProcess.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GamesProcess.Data.Services
{
    public class TodoDbService
    {
        private ApplicationDbContext _applicationDbContext;

        public TodoDbService(ApplicationDbContext applicationDb)
        {
            _applicationDbContext = applicationDb;
        }

        public async Task<List<TodoItem>> GetTodoItemsAsync()
        {
            return await _applicationDbContext.TodoItems.OrderByDescending(e => e.TimeAdded).ToListAsync();
        }
    }
}
