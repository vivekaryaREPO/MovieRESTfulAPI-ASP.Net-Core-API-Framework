using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieApi.DTOs;
using MovieApi.Entities;
using MovieApi.Helpers;
using MoviesAPI.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MovieApi.Controllers
{
    [ApiController]
    [Route("api/people")]
    public class PeopleController:ControllerBase
    {
        private readonly ApplicationDBContext context;
        private readonly IMapper mapper;
        private readonly IFileStorageService fileStorageService;
        private readonly string containerName = "persons";//name of folder inside wwwroot

        public PeopleController(ApplicationDBContext _context,IMapper _mapper,
            IFileStorageService _fileStorageService)
        {
            context = _context;
            mapper = _mapper;
            fileStorageService = _fileStorageService;
        }


        //[HttpGet]
        //public async Task<ActionResult<List<PersonDTO>>> Get()
        //{
        //    var listOfPeople= await context.Persons.AsNoTracking().ToListAsync();
        //    var listPeopleDTO = mapper.Map<List<PersonDTO>>(listOfPeople);
        //    return listPeopleDTO;
        //}


        [HttpGet("{id}", Name ="getPersonById")]
        public async Task<ActionResult<PersonDTO>> Get(int id)
        {
            //AsNoTracking() is just to boost performance
            var exist = await context.Persons.AsNoTracking().AnyAsync(x=>x.Id==id);
            if(!exist)
            {
                return NotFound();
            }
            else
            {
                var person = await context.Persons.FirstOrDefaultAsync(x=>x.Id==id);
                var personDTO= mapper.Map<PersonDTO>(person);
                return (PersonDTO)personDTO;
            }

        }


        [HttpPost]
        public async Task<ActionResult<PersonDTO>> Post([FromForm]PersonCreationDTO personCreationDTO)
        {
            //AsNoTracking() is just to boost performance
            var person = mapper.Map<Person>(personCreationDTO);

            if(personCreationDTO.Picture!=null)
            {
                using (var memorystream=new MemoryStream())
                {
                    await personCreationDTO.Picture.CopyToAsync(memorystream);
                    var content = memorystream.ToArray();
                    var extension = Path.GetExtension(personCreationDTO.Picture.FileName);
                    person.Picture =
                        await fileStorageService.SaveFile(content,extension,containerName,personCreationDTO.Picture.ContentType);
                }
            }
            context.Add(person);
            await context.SaveChangesAsync();

            var personDTO = mapper.Map<PersonDTO>(person);
            return new CreatedAtRouteResult("getPersonById",new { Id= personDTO.Id},personDTO);

        }



        //HttpPost basically needs all the fields in order to update the record in database
        //If you don't send value for some field, those fields will get updated to null
        //and this is why we need patch method.

        [HttpPut]
        public async Task<ActionResult<PersonDTO>> Put(int id, [FromForm] PersonCreationDTO personCreationDTO)
        {
            var personDB = await context.Persons.FirstOrDefaultAsync(x=>x.Id==id);
            if(personDB==null)
            {
                return NoContent();
            }
            personDB = mapper.Map(personCreationDTO,personDB); //So only updated properties in will be updated inside personDB


            if (personCreationDTO.Picture != null)
            {
                using (var memorystream = new MemoryStream())
                {
                    await personCreationDTO.Picture.CopyToAsync(memorystream);
                    var content = memorystream.ToArray();
                    var extension = Path.GetExtension(personCreationDTO.Picture.FileName);
                    personDB.Picture =
                        await fileStorageService.EditFile(content, extension, containerName, personDB.Picture, personCreationDTO.Picture.ContentType);
                }
            }
            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var remove = await context.Persons.AnyAsync(x => x.Id == id);
            if (!remove)
            {
                return NotFound();
            }
            context.Remove(new Person() { Id = id });
            await context.SaveChangesAsync();
            return NoContent();
        }


        //So when only partial values have to be updated on the entity, advantage is, you don't need to
        //send all the values, this would save a lot of bandwidth.
        //we need to install using package manager-newtonsoft json
        [HttpPatch]
        public async Task<ActionResult> Patch(int id,[FromBody] JsonPatchDocument<PersonPatchDTO> patchDocument)
        {
            var entityFromDb = await context.Persons.FirstOrDefaultAsync(p => p.Id == id);
            if (entityFromDb == null)
            {
                return BadRequest();
            }
            //getting personPatchDTO form person(entityFromDb) i.e mapping
            var personDTO = mapper.Map<PersonPatchDTO>(entityFromDb);

            //updating inside PersonPatchDTO the values we've received in patchDocument which is of JsonPatchDocument type-see method parameter
            patchDocument.ApplyTo(personDTO,ModelState);
            //so now we have a PersonPatchDTO with updated values

            //let's check if the values are valid or not.
            var isValid = TryValidateModel(personDTO);
            if(!isValid)
            {
                return BadRequest(ModelState);
            }
            //updating value from personDTO into entityFromDb
            mapper.Map(personDTO,entityFromDb);
            await context.SaveChangesAsync();
            return NoContent();
            
        }

        [HttpGet]
        public async Task<ActionResult<List<PersonDTO>>> Get([FromQuery] PaginationDTO paginationDTO)
        {
            var queryable = context.Persons.AsQueryable();
            await HttpContext.InsertPaginationParametersInResponse(queryable,paginationDTO.RecordsPerPage);
            var people = await queryable.Paginate(paginationDTO).ToListAsync();
            return mapper.Map<List<PersonDTO>>(people);
        }


































    }
}
