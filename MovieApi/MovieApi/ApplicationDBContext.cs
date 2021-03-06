﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MovieApi.Entities;
using MoviesAPI.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace MovieApi
{
    public class ApplicationDBContext : IdentityDbContext
    {
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Person> Persons { get; set; }

        public DbSet<Movie> Movies { get; set; }
        public DbSet<MoviesActors> MoviesActor { get; set; }

        public DbSet<MoviesGenres> MoviesGenres { get; set; }
        public ApplicationDBContext([NotNullAttribute] DbContextOptions options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //SeedData(modelBuilder);
            //configuring composite primary keys
            modelBuilder.Entity<MoviesGenres>().HasKey(x => new { x.GenreId, x.MovieId });
            modelBuilder.Entity<MoviesActors>().HasKey(x => new { x.PersonId, x.MovieId });

           SeedData(modelBuilder);

            base.OnModelCreating(modelBuilder);
        }


        private void SeedData(ModelBuilder modelBuilder)
        {
            var comedy = new Genre() { Id = 5, Name = "Comedy" };
            var action_comedy = new Genre() { Id = 6, Name = "Action Comedy" };
            var horror = new Genre() { Id = 7, Name = "Horror" };
            var thriller = new Genre() { Id = 8, Name = "Thriler" };

            modelBuilder.Entity<Genre>()
                .HasData(new List<Genre>
                {
                    comedy, action_comedy, horror, thriller
                });

            //var jimCarrey = new Person() { Id = 5, Name = "Jim Carrey", DateOfBirth = new DateTime(1962, 01, 17) };
            //var robertDowney = new Person() { Id = 6, Name = "Robert Downey Jr.", DateOfBirth = new DateTime(1965, 4, 4) };
            //var chrisEvans = new Person() { Id = 7, Name = "Chris Evans", DateOfBirth = new DateTime(1981, 06, 13) };

            //modelBuilder.Entity<Person>()
            //    .HasData(new List<Person>
            //    {
            //        jimCarrey, robertDowney, chrisEvans
            //    });

            //var endgame = new Movie()
            //{
            //    Id = 2,
            //    Title = "Avengers: Endgame",
            //    InTheaters = true,
            //    ReleaseDate = new DateTime(2019, 04, 26)
            //};

            //var iw = new Movie()
            //{
            //    Id = 3,
            //    Title = "Avengers: Infinity Wars",
            //    InTheaters = false,
            //    ReleaseDate = new DateTime(2019, 04, 26)
            //};

            //var sonic = new Movie()
            //{
            //    Id = 4,
            //    Title = "Sonic the Hedgehog",
            //    InTheaters = false,
            //    ReleaseDate = new DateTime(2020, 02, 28)
            //};
            //var emma = new Movie()
            //{
            //    Id = 5,
            //    Title = "Emma",
            //    InTheaters = false,
            //    ReleaseDate = new DateTime(2020, 02, 21)
            //};
            //var greed = new Movie()
            //{
            //    Id = 6,
            //    Title = "Greed",
            //    InTheaters = false,
            //    ReleaseDate = new DateTime(2020, 02, 21)
            //};

            //modelBuilder.Entity<Movie>()
            //    .HasData(new List<Movie>
            //    {
            //        endgame, iw, sonic, emma, greed
            //    });

            //modelBuilder.Entity<MoviesGenres>().HasData(
            //    new List<MoviesGenres>()
            //    {
            //        new MoviesGenres(){MovieId = endgame.Id, GenreId = drama.Id},
            //        new MoviesGenres(){MovieId = endgame.Id, GenreId = adventure.Id},
            //        new MoviesGenres(){MovieId = iw.Id, GenreId = drama.Id},
            //        new MoviesGenres(){MovieId = iw.Id, GenreId = adventure.Id},
            //        new MoviesGenres(){MovieId = sonic.Id, GenreId = adventure.Id},
            //        new MoviesGenres(){MovieId = emma.Id, GenreId = drama.Id},
            //        new MoviesGenres(){MovieId = emma.Id, GenreId = romance.Id},
            //        new MoviesGenres(){MovieId = greed.Id, GenreId = drama.Id},
            //        new MoviesGenres(){MovieId = greed.Id, GenreId = romance.Id},
            //    });

            //modelBuilder.Entity<MoviesActors>().HasData(
            //    new List<MoviesActors>()
            //    {
            //        new MoviesActors(){MovieId = endgame.Id, PersonId = robertDowney.Id, Character = "Tony Stark", Order = 1},
            //        new MoviesActors(){MovieId = endgame.Id, PersonId = chrisEvans.Id, Character = "Steve Rogers", Order = 2},
            //        new MoviesActors(){MovieId = iw.Id, PersonId = robertDowney.Id, Character = "Tony Stark", Order = 1},
            //        new MoviesActors(){MovieId = iw.Id, PersonId = chrisEvans.Id, Character = "Steve Rogers", Order = 2},
            //        new MoviesActors(){MovieId = sonic.Id, PersonId = jimCarrey.Id, Character = "Dr. Ivo Robotnik", Order = 1}
            //    });
        }






















    }
}
