using Microsoft.Extensions.Logging;
using MovieApi.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieApi.Services
{
    public class InMemoryRepository : IRepository
    {
        public List<Genre> listOfGenre;
        public readonly ILogger<InMemoryRepository> logger;
        public InMemoryRepository(ILogger<InMemoryRepository> _logger)
        {
            this.logger = _logger;
            this.listOfGenre = new List<Genre>() {

                new Genre(){Id=1,Name="Action"},
                new Genre(){Id=2,Name="Comedy"},
                new Genre(){Id=3,Name="Thriller"},
                new Genre(){Id=4,Name="Horror"},
                new Genre(){Id=5,Name="Suspense"}

            };

        }

        async Task<Genre> IRepository.GetGenreById(int id)
        {
            await Task.Delay(2000);
            this.logger.LogInformation("Getting genre by id from Inmemoryrepository");
            return   this.listOfGenre.FirstOrDefault(movie=>movie.Id==id);
        }

        async  Task<List<Genre>> IRepository.GetListGenre()
        {
            this.logger.LogInformation("Getting list of movie genre from Inmemoryrepository");
            await Task.Delay(2000);
            return this.listOfGenre;
        }

         void IRepository.AddGenre(Genre genre)
        {
            this.logger.LogInformation("Adding genre from Inmemoryrepository");
            genre.Id = this.listOfGenre.Max(x => x.Id) + 1;
            this.listOfGenre.Add(genre);
            return;
        }





    }
}
