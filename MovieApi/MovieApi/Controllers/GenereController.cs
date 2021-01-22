using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MovieApi.DTOs;
using MovieApi.Entities;
using MovieApi.Filters;
using MovieApi.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
//******************************NEVER USE this KEYWORD in Controller********************************************
namespace MovieApi.Controllers
{
    [Route("api/genere")]
    [ApiController]
    public class GenereController : ControllerBase
    {
        private readonly IRepository repo;
        private readonly ILogger<GenereController> logger;
        private readonly ApplicationDBContext context;
        private readonly IMapper mapper;

        public GenereController(
                                IRepository repository,
                                ILogger<GenereController> logger,
                                ApplicationDBContext _context,
                                IMapper mapper
                                )
        {
            this.repo = repository;
            this.logger = logger;
            context = _context;
            this.mapper = mapper;
        }


        // [HttpGet("getGenereList")]       
        // public ActionResult<List<Genre>> Get()
        // {
        //     return Ok(this.repo.GetListGenre());
        // }


        //[HttpGet("getGenereList/{id=1}")]     //in simple words, after api/genere/ we are expecting an int parameter in this endpoint 
        // public ActionResult<Genre> Get(int? id)
        // {
        //     if(id==null)
        //     {
        //         //see the definition of NotFound() to understand how we are able to return ActionResult<Genre>
        //         return NotFound(); 
        //     }
        //     else
        //     {
        //         return Ok(this.repo.GetGenreById(id.Value));
        //     }

        // }


        //[HttpGet("getGenereList")]
        //[HttpGet]
        //[ResponseCache(Duration =30)] //executed only after authorization, this one lasts for 30 sec, if u make request before it, this method won't be hit.Don't forget to set caching in client eg. switch off from settings in Postman.
        //[ServiceFilter(typeof(MyActionFilter))]
        //public async Task<ActionResult<List<Genre>>> Get()
        //{

        //    this.logger.LogInformation("Getting list of movie genre");
        //    //throw new ApplicationException();
        //    return  await this.repo.GetListGenre();
        //}


        // [HttpGet("{id:int}",Name ="getGenre")] //https://localhost:44365/api/genere/1
        // [HttpGet("getGenereList/{id:int}")] //https://localhost:44365/api/genere/getGenereList/1
        //// [ResponseCache(Duration = 30)]
        // public async Task<ActionResult<Genre>> Get(int id,[FromQuery] string value) //https://localhost:44365/api/genere/getGenereList/1?value=hello
        // {
        //     //this.logger.LogInformation("Getting genre by ID executed-information");
        //     //this.logger.LogWarning("Getting genre by ID executed-warning ");
        //     //this.logger.LogCritical("Getting genre by ID executed-critical ");
        //     if (id == 0)
        //     {
        //         //see the definition of NotFound() to understand how we are able to return ActionResult<Genre>
        //         return   NotFound();
        //     }
        //     else
        //     {
        //         return await this.repo.GetGenreById(id);
        //     }

        // }




        //[HttpGet("getGenereListQuery")] //https://localhost:44365/api/genere/getGenereList/1
        //public async Task<ActionResult<Genre>> GetGenereList([FromQuery] int id, [FromQuery] string value1, [FromQuery] string value2) //https://localhost:44365/api/genere/getGenereList?id=4&value1=hello1&value2=hello2
        //{
        //    if (id == 0)
        //    {
        //        //see the definition of NotFound() to understand how we are able to return ActionResult<Genre>
        //        return NotFound();
        //    }
        //    else
        //    {
        //        return await this.repo.GetGenreById(id);
        //    }

        //}

        //[HttpGet("getGenereListQuery/{id:int}")] //https://localhost:44365/api/genere/getGenereListQuery/1
        //public async Task<ActionResult<Genre>> GetGenereList(int id, string value1, string value2) //https://localhost:44365/api/genere/getGenereListQuery/4?value1=hello1&value2=hello2
        //{
        //    if (id == 0)
        //    {
        //        //see the definition of NotFound() to understand how we are able to return ActionResult<Genre>
        //        return NotFound();
        //    }
        //    else
        //    {
        //        return await this.repo.GetGenreById(id);
        //    }

        //}



        //[HttpPost] 
        //public ActionResult<Genre>  Post([FromBody] Genre genre) 
        //{
        //    this.repo.AddGenre(genre);
        //    return new CreatedAtRouteResult("getGenre",new {Id=genre.Id },genre);

        //}




        //****************MAIN ENTITY FRAMEWORK CORE**********************

        [HttpGet]
        public async Task<ActionResult<List<GenreDTO>>> Get()
        {
            //AsNoTracking() is just for performance enhancement: as we don't want to track changes into the resulting Genres of this query.
            var genre = await context.Genres.AsNoTracking().ToListAsync();
            var genreDTO = mapper.Map<List<GenreDTO>>(genre);
            return genreDTO;

        }


        [HttpGet("{id:int}", Name = "getGenreById")]
        public async Task<ActionResult<GenreDTO>> Get(int id)
        {
            //AsNoTracking() is just for performance enhancement: as we don't want to track changes into the resulting Genres of this query.
            var genre = await context.Genres.AsNoTracking().FirstOrDefaultAsync(genre => genre.Id == id);
            var genreDTO = mapper.Map<GenreDTO>(genre);
            return genreDTO;
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,Roles ="")]
        public async Task<ActionResult<GenreCreationDTO>> Post([FromBody] GenreCreationDTO genre) //[FromBody] is optional
        {
            var genreTemp = mapper.Map<Genre>(genre);
            context.Genres.Add(genreTemp);
            await context.SaveChangesAsync();
            //make GenreDTO object so that you can pass it in the getGenreById returns a GenreDTO object, this is required for HATEOS 
            var genreDTO = mapper.Map<GenreDTO>(genreTemp); //ReverseMap is allowing this to happen
            return new CreatedAtRouteResult("getGenreById", new { id = genreDTO.Id, name = genreDTO.Name }, genreDTO);


        }

        [HttpPut("{id}")] //this should not be "{id:int}" because here we are actually changine the request uri and expecting a value on id
        public async Task<ActionResult> Put(int id,[FromBody] GenreCreationDTO genreCreationDTO)
       {
            //Map To Genre by accepting object of GenreCreationDTO
            var genre = mapper.Map<Genre>(genreCreationDTO);
            genre.Id = id;
            context.Entry(genre).State = EntityState.Modified;
            await context.SaveChangesAsync();
            return NoContent();


      }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var remove = await context.Genres.AnyAsync(x=>x.Id==id);
            if(!remove)
            {
                return NotFound();
            }
            context.Remove(new Genre() { Id = id });
            await context.SaveChangesAsync();
            return NoContent();
        }









































    }
}
