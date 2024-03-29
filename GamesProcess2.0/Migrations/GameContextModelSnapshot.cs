﻿// <auto-generated />
using System;
using GamesProcess2.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace GamesProcess2.Migrations
{
    [DbContext(typeof(GameContext))]
    partial class GameContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.14-servicing-32113")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("GamesProcess2.Models.Event", b =>
                {
                    b.Property<int>("EventID");

                    b.Property<DateTime>("Date");

                    b.Property<int>("EventNumber");

                    b.Property<int>("GameID");

                    b.Property<string>("MachineValues");

                    b.Property<string>("WinningValues");

                    b.HasKey("EventID");

                    b.HasIndex("GameID");

                    b.ToTable("Event");
                });

            modelBuilder.Entity("GamesProcess2.Models.Game", b =>
                {
                    b.Property<int>("ID");

                    b.Property<int>("GamesClassID");

                    b.Property<string>("Name");

                    b.HasKey("ID");

                    b.HasIndex("GamesClassID");

                    b.ToTable("Game");
                });

            modelBuilder.Entity("GamesProcess2.Models.GamesClass", b =>
                {
                    b.Property<int>("ID");

                    b.Property<string>("Name");

                    b.HasKey("ID");

                    b.ToTable("GamesClass");
                });

            modelBuilder.Entity("GamesProcess2.Models.TodoItem", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Details");

                    b.Property<bool>("IsDone");

                    b.Property<DateTime>("TimeAdded");

                    b.Property<string>("Title")
                        .IsRequired();

                    b.HasKey("ID");

                    b.ToTable("TodoItem");
                });

            modelBuilder.Entity("GamesProcess2.Models.Event", b =>
                {
                    b.HasOne("GamesProcess2.Models.Game")
                        .WithMany("Events")
                        .HasForeignKey("GameID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("GamesProcess2.Models.Game", b =>
                {
                    b.HasOne("GamesProcess2.Models.GamesClass")
                        .WithMany("Games")
                        .HasForeignKey("GamesClassID")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
