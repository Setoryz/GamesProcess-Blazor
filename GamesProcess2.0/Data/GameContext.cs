﻿using GamesProcess2.Models;
using Microsoft.EntityFrameworkCore;

namespace GamesProcess2.Data
{
    public class GameContext : DbContext
    {
        public GameContext(DbContextOptions<GameContext> options) : base(options)
        {
        }
        public DbSet<GamesClass> GamesClass { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<TodoItem> TodoItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<GamesClass>().ToTable("GamesClass");
            modelBuilder.Entity<Game>().ToTable("Game");
            modelBuilder.Entity<Event>().ToTable("Event");
            modelBuilder.Entity<TodoItem>().ToTable("TodoItem");
        }
    }
}
