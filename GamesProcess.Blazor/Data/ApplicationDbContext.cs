using GamesProcess.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GamesProcess.Data
{
    public class ApplicationDbContext :DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<GamesClass> GamesClass { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<TodoItem> TodoItems { get; set; }
        public DbSet<AdvancedSearchParameters> AdvancedSearchParameters { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<GamesClass>().ToTable("GamesClass");
            modelBuilder.Entity<Game>().ToTable("Game");
            modelBuilder.Entity<Event>().ToTable("Event");
            modelBuilder.Entity<TodoItem>().ToTable("TodoItem");
            modelBuilder.Entity<AdvancedSearchParameters>().ToTable("AdvancedSearchParameter");
        }
    }
}
