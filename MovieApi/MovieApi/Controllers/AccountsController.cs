//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Extensions.Configuration;
//using Microsoft.IdentityModel.Tokens;
//using MovieApi.DTOs;
//using System;
//using System.Collections.Generic;
//using System.IdentityModel.Tokens.Jwt;
//using System.Linq;
//using System.Security.Claims;
//using System.Text;
//using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MovieApi.DTOs;
using MovieApi.Helpers;
using MoviesAPI.DTOs;
using MoviesAPI.Helpers;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MovieApi.Controllers
{
    [Route("api/accounts")]
    [ApiController]
    //[EnableCors(PolicyName = "AllowAPIRequestIO")]
    public class AccountsController:ControllerBase
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signinManager;
        private readonly IConfiguration Configuration;
        private readonly ApplicationDBContext context;
        private readonly IMapper mapper;

        public AccountsController(UserManager<IdentityUser> _userManager, SignInManager<IdentityUser> _signinManager,
                                  IConfiguration _configuration, ApplicationDBContext _context,
                                  IMapper _mapper)
        {
            userManager = _userManager;
            signinManager = _signinManager;
            Configuration = _configuration;
            context = _context;
            mapper = _mapper;
        }


        //GETTING ALL USERS FROM DB
        [HttpGet("Users")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<ActionResult<List<UserDTO>>> Get([FromQuery] PaginationDTO paginationDTO)
        {
            //Users is our AspNetUsers table in the database
            var queryable = context.Users.AsQueryable();
            queryable = queryable.OrderBy(x => x.Email);
            await HttpContext.InsertPaginationParametersInResponse(queryable, paginationDTO.RecordsPerPage);
            var users = await queryable.Paginate(paginationDTO).ToListAsync();
            return mapper.Map<List<UserDTO>>(users);
        }

        //[HttpPost("Create")]
        //public async Task<ActionResult<UserToken>> CreateUser([FromBody] UserInfo model)
        //{
        //    var user = new IdentityUser { UserName = model.EmailAddress, Email = model.EmailAddress };
        //    var result = await userManager.CreateAsync(user, model.Password);

        //    if (result.Succeeded)
        //    {
        //        return await BuildToken(model);
        //    }
        //    else
        //    {
        //        return BadRequest(result.Errors);
        //    }
        //}

        //[HttpPost("RenewToken")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        //public async Task<ActionResult<UserToken>> Renew()
        //{
        //    var userInfo = new UserInfo
        //    {
        //        EmailAddress = HttpContext.User.Identity.Name
        //    };

        //    return await BuildToken(userInfo);
        //}

        //[HttpPost("Login")]
        //public async Task<ActionResult<UserToken>> Login([FromBody] UserInfo model)
        //{
        //    var result = await signinManager.PasswordSignInAsync(model.EmailAddress,
        //        model.Password, isPersistent: false, lockoutOnFailure: false);

        //    if (result.Succeeded)
        //    {
        //        return await BuildToken(model);
        //    }
        //    else
        //    {
        //        return BadRequest("Invalid login attempt");
        //    }
        //}


        ////public async Task<ActionResult<UserToken>> BuildToken(UserInfo userInfo)
        ////{
        ////    var claims = new List<Claim>()
        ////    {
        ////        new Claim(ClaimTypes.Name,userInfo.EmailAddress){},
        ////        new Claim(ClaimTypes.Name,userInfo.EmailAddress){},
        ////        new Claim("mykey","whatever value I want"){}
        ////    };
        ////    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["jwt:key"]));
        ////    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        ////    var expiration = DateTime.UtcNow.AddYears(1);
        ////    JwtSecurityToken token = new JwtSecurityToken(issuer: null, audience: null, claims: claims,
        ////        expires: expiration, signingCredentials: creds);
        ////    return new UserToken()
        ////    {
        ////        Token = new JwtSecurityTokenHandler().WriteToken(token),
        ////        Expiration = expiration
        ////    };


        ////}

        //[HttpGet("Roles")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        //public async Task<ActionResult<List<string>>> GetRoles()
        //{
        //    return await context.Roles.Select(x => x.Name).ToListAsync();
        //}

        //[HttpPost("AssignRole")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        //public async Task<ActionResult> AssignRole(EditRoleDTO editRoleDTO)
        //{
        //    var user = await userManager.FindByIdAsync(editRoleDTO.UserId);
        //    if (user == null)
        //    {
        //        return NotFound();
        //    }

        //    await userManager.AddClaimAsync(user, new Claim(ClaimTypes.Role, editRoleDTO.RoleName));
        //    return NoContent();
        //}

        //[HttpPost("RemoveRole")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        //public async Task<ActionResult> RemoveRole(EditRoleDTO editRoleDTO)
        //{
        //    var user = await userManager.FindByIdAsync(editRoleDTO.UserId);
        //    if (user == null)
        //    {
        //        return NotFound();
        //    }

        //    await userManager.RemoveClaimAsync(user, new Claim(ClaimTypes.Role, editRoleDTO.RoleName));
        //    return NoContent();
        //}









    }
}
