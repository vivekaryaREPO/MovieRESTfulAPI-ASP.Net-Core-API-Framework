﻿using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MovieApi.DTOs;
using MovieApi.Entities;
using MovieApi.Helpers;
using MoviesAPI.DTOs;
using MoviesAPI.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MovieApi.Controllers
{
    [ApiController]
    [Route("api/movies")]
    public class MoviesController : ControllerBase
    {
        private readonly ApplicationDBContext context;
        private readonly IMapper mapper;
        private readonly IFileStorageService fileStorageService;
        private readonly ILogger<MoviesController> logger;
        private readonly string containerName = "movies";
        public MoviesController
            (ApplicationDBContext _context, IMapper _mapper, IFileStorageService _fileStorageService,ILogger<MoviesController> _logger)
        {
            this.context = _context;
            this.mapper = _mapper;
            this.fileStorageService = _fileStorageService;
            logger = _logger;
        }

        [HttpGet]
        public async Task<ActionResult<List<MoviesDTO>>> Get()
        {
            var movies= await context.Movies.ToListAsync();
            var moviesDTO = mapper.Map<List<MoviesDTO>>(movies);
            return moviesDTO;
        }

        //[HttpGet("getFilteredEmployees")]
        //public async Task<ActionResult<IndexMoviePageDTO>> getFilteredEmployees()
        //{
        //    var top = 6;
        //    var today = DateTime.Today;
        //    var upCompingReleases = await context.Movies.
        //                                               Where(x => x.ReleaseDate > today).
        //                                               OrderBy(x => x.ReleaseDate).Take(top).ToListAsync();

        //    var inTheatres= await context.Movies.
        //                                               Where(x => x.InTheaters).
        //                                               Take(top).ToListAsync();

        //    var result = new IndexMoviePageDTO();
        //    result.InTheaters = mapper.Map<List<MoviesDTO>>(inTheatres);
        //    result.UpcomingReleases = mapper.Map<List<MoviesDTO>>(upCompingReleases);
        //    return result;
        //}


        [HttpGet("filter")]
        public async Task<ActionResult<List<MoviesDTO>>> Filter([FromQuery] FilterMoviesDTO filterMoviesDTO)
        {
            var moviesQueryable = context.Movies.AsQueryable();

            if (!string.IsNullOrWhiteSpace(filterMoviesDTO.Title))
            {
                moviesQueryable = moviesQueryable.Where(x => x.Title.Contains(filterMoviesDTO.Title));
            }

            if (filterMoviesDTO.InTheaters)
            {
                moviesQueryable = moviesQueryable.Where(x => x.InTheaters);
            }

            if (filterMoviesDTO.UpcomingReleases)
            {
                var today = DateTime.Today;
                moviesQueryable = moviesQueryable.Where(x => x.ReleaseDate > today);
            }

            if (filterMoviesDTO.GenreId != 0)
            {
                moviesQueryable = moviesQueryable
                    .Where(x => x.MoviesGenres.Select(y => y.GenreId)
                    .Contains(filterMoviesDTO.GenreId));
            }

            if (!string.IsNullOrWhiteSpace(filterMoviesDTO.OrderingField))
            {
                //try
                //{
                //    moviesQueryable = moviesQueryable.OrderBy($"{filterMoviesDTO.OrderingField} {(filterMoviesDTO.AscendingOrder ? "ascending" : "descending")}");
                //}
                //catch
                //{
                //    // log this
                //    logger.LogWarning("Could not order by field: " + filterMoviesDTO.OrderingField);
                //}
            }

            await HttpContext.InsertPaginationParametersInResponse(moviesQueryable,
                filterMoviesDTO.RecordsPerPage);

            var movies = await moviesQueryable.Paginate(filterMoviesDTO.Pagination).ToListAsync();

            return mapper.Map<List<MoviesDTO>>(movies);
        }







        //[HttpGet("{id}",Name = "getMovie")]
        //public async Task<ActionResult<MoviesDTO>> Get(int id)
        //{
        //    var movie = await context.Movies.FirstOrDefaultAsync(x=>x.Id==id);
        //    if(movie==null)
        //    {
        //        return NoContent();
        //    }
        //    var movieDTO = mapper.Map<MoviesDTO>(movie);
        //    return movieDTO;
        //}

        [HttpPost]
        public async Task<ActionResult<MoviesDTO>> Post([FromForm]MovieCreationDTO movieCreationDTO)
        {
            //to movie from movieCreationDTO
            var movie = mapper.Map<Movie>(movieCreationDTO);
            
            if (movieCreationDTO.Poster != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await movieCreationDTO.Poster.CopyToAsync(memoryStream);
                    var content = memoryStream.ToArray();
                    var extension = Path.GetExtension(movieCreationDTO.Poster.FileName);
                    movie.Poster =
                        await fileStorageService.SaveFile(content, extension, containerName,
                                                            movieCreationDTO.Poster.ContentType);
                }
            }

            AnnotateActorsOrder(movie);

            context.Add(movie);
            await context.SaveChangesAsync();
            var movieDTO = mapper.Map<MoviesDTO>(movie);
            return new CreatedAtRouteResult("getMovie", new { id = movie.Id }, movieDTO);

        }


        [HttpGet("{id}", Name = "getMovie")]
        public async Task<ActionResult<MovieDetailsDTO>> Get(int id)
        {
            
            var movie = await context.Movies
                .Include(x => x.MoviesActors).ThenInclude(x => x.Person)//gives list actors i.e with properties character, movie,movieid,order,person,personId i.e object of MoviesActors
                .Include(x => x.MoviesGenres).ThenInclude(x => x.Genre)//gets list of MoviesGenres with each object has genre id, genre name
                .FirstOrDefaultAsync(x => x.Id == id);

            if (movie == null)
            {
                return NotFound();
            }
            //we are doing this because we don't want to return movie, we want to return MovieDetailsDTO
            return mapper.Map<MovieDetailsDTO>(movie);
        }




















        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromForm] MovieCreationDTO movieCreationDTO)
        {
            var movieDB = await context.Movies.FirstOrDefaultAsync(x => x.Id == id);

            if (movieDB == null)
            {
                return NotFound();
            }

            movieDB = mapper.Map(movieCreationDTO, movieDB);

            if (movieCreationDTO.Poster != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await movieCreationDTO.Poster.CopyToAsync(memoryStream);
                    var content = memoryStream.ToArray();
                    var extension = Path.GetExtension(movieCreationDTO.Poster.FileName);
                    movieDB.Poster =
                        await fileStorageService.EditFile(content, extension, containerName,
                                                            movieDB.Poster,
                                                            movieCreationDTO.Poster.ContentType);
                }
            }

            //This query=> delete the movie detail of Id movieDB.Id, from MoviesActors and MoviesGenres tables, so that our update can lead to re insert the values.
            await context.Database.ExecuteSqlInterpolatedAsync($"delete from MoviesActor where MovieId = {movieDB.Id}; delete from MoviesGenres where MovieId = {movieDB.Id}");

            AnnotateActorsOrder(movieDB);

            await context.SaveChangesAsync();
            return NoContent();
        }

        private static void AnnotateActorsOrder(Movie movie)
        {
            if (movie.MoviesActors != null)
            {
                for (int i = 0; i < movie.MoviesActors.Count; i++)
                {
                    movie.MoviesActors[i].Order = i;
                }
            }
        }









        [HttpPatch("{id}")]
        public async Task<ActionResult> Patch(int id, [FromBody] JsonPatchDocument<MoviePatchDTO> patchDocument)
        {
            if (patchDocument == null)
            {
                return BadRequest();
            }

            var entityFromDB = await context.Movies.FirstOrDefaultAsync(x => x.Id == id);

            if (entityFromDB == null)
            {
                return NotFound();
            }

            var entityDTO = mapper.Map<MoviePatchDTO>(entityFromDB);

            patchDocument.ApplyTo(entityDTO, ModelState);

            var isValid = TryValidateModel(entityDTO);

            if (!isValid)
            {
                return BadRequest(ModelState);
            }

            mapper.Map(entityDTO, entityFromDB);

            await context.SaveChangesAsync();

            return NoContent();
        }


        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var exists = await context.Movies.AnyAsync(x => x.Id == id);

            if (!exists)
            {
                return NotFound();
            }

            context.Remove(new Movie() { Id = id });
            await context.SaveChangesAsync();
            return NoContent();
        }
    }
}
