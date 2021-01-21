using MovieApi.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieApi.Services
{
    public interface IRepository
    {
         Task<List<Genre>> GetListGenre();
         Task<Genre> GetGenreById(int id);
        void AddGenre(Genre genre);
    }
}
