using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using MovieApi.Filters;
using MovieApi.Services;
using MoviesAPI.Services;

namespace MovieApi
{
    public class Startup
    {
        private  ILogger<Startup> logger;
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddResponseCaching();
            services.AddAutoMapper(typeof(Startup)); //AddAutoMapper() is an extension method
            services.AddDbContext<ApplicationDBContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"))); //UseSqlServer() is an extension method
            services.AddControllers(options =>
            {
                options.Filters.Add(typeof(MyExceptionFilter));
            }).AddNewtonsoftJson().AddXmlDataContractSerializerFormatters();

            services.AddSingleton<IRepository, InMemoryRepository>();
            services.AddIdentity<IdentityUser, IdentityRole>().
                AddEntityFrameworkStores<ApplicationDBContext>().
                AddDefaultTokenProviders();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).
                     AddJwtBearer(options =>
                     options.TokenValidationParameters = new TokenValidationParameters()
                     {
                         ValidateIssuer = false,
                         ValidateAudience = false,
                         ValidateLifetime = true,
                         ValidateIssuerSigningKey = true,
                         IssuerSigningKey = new SymmetricSecurityKey(
                             Encoding.UTF8.GetBytes(Configuration["jwt:key"])),
                         ClockSkew = TimeSpan.Zero
                     }
                     );
            services.AddTransient<IFileStorageService, InAppStorageService>();
            services.AddHttpContextAccessor();

            services.AddTransient<MyActionFilter>();

            // services.AddTransient<IHostedService, WriteToFileHostedService>();
            services.AddTransient<IHostedService, MovieInTheatersService>();
            //services.AddCors();
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAPIRequestIO",
                builder => builder.WithOrigins("www.apirequest.io.").WithMethods("Get", "Post").AllowAnyHeader());
            });
        }
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> _logger)
        {
            this.logger = _logger;
            app.Use(async (context, next) =>
            {
                using (var swapStream = new MemoryStream())
                {
                    //Just like a swapping logic

                    //Response is an abstract property of HttpResponse type 
                    //in abstract Class HttpContext. It Gets the Microsoft.AspNetCore.Http.HttpResponse object for this request.

                    //saving context.Response.Body in originalResponseBody
                    //logger.LogInformation("1. Hi My message is:context.Response.Body is:" + context.Response.Body);

                    //we had to do this cos we are not allowed to read Body of response directly
                    var originalResponseBody = context.Response.Body; //get reference of original context.Response.Body in the originalResponseBody
                    //logger.LogInformation("2. Hi My message is:originalResponseBody is:" + originalResponseBody);

                    //making context.Response.Body point to object of MemoryStream
                    //as Body is Stream type, and MemoryStream class  extends Stream class
                    //So base class reference is pointing child class object.
                    //so Body and swapStream are both pointing to same object
                    //logger.LogInformation("3. Hi My message is swapStream: " + swapStream);

                    context.Response.Body = swapStream; //make reference of context.Response.Body to point to our MemoryStream

                    /*logger.LogInformation("4. Hi My message is  context.Response.Body is: " + context.Response.Body)*/;
                    
                    await next.Invoke();// this will get data into context.Response.Body which is pointing to object of MemoryStream and this object is also pointed by swapStream

                    swapStream.Seek(0, SeekOrigin.Begin); //now,swapStream is pointing to object that has response, so get pointer to beginning before reading

                    //logger.LogInformation("5. Hi My message is swapStream: " + swapStream);

                    string responseBody = new StreamReader(swapStream).ReadToEnd(); //reading into responseBody till the end of swapStream 

                    //logger.LogInformation("6. Hi My message is responseBody: " + responseBody);

                    swapStream.Seek(0, SeekOrigin.Begin); //get pointer to beginning after reading

                    //logger.LogInformation("7. Hi My message is swapStream: " + swapStream);
                    //logger.LogInformation("8. Hi My message is originalResponseBody: " + originalResponseBody);

                    //if you remove this below line, the output will be empty
                    //we had to do this cos we are not allowed to read Body of response directly
                    await swapStream.CopyToAsync(originalResponseBody); //type of originalResponseBody was made to be same as context.Response.Body and so they are now pointing to same object, so swapStream data is copied into originalResponseBody

                    //logger.LogInformation("9. Hi My message is swapStream: " + swapStream);
                    //logger.LogInformation("10. Hi My message is originalResponseBody: " + originalResponseBody);

                    //though they were pointing to the same object, we do this as we are going
                    //to return context only
                    //logger.LogInformation("11. Hi My message is originalResponseBody: " + originalResponseBody);
                    //logger.LogInformation("12. Hi My message is context.Response.Body:" + context.Response.Body);
                    
                    
                    context.Response.Body = originalResponseBody; //as they are of same type, they can point to same object now
                    
                    //logger.LogInformation("13. Hi My message is originalResponseBody: " + originalResponseBody);
                    //logger.LogInformation("14. Hi My message is context.Response.Body:" + context.Response.Body);
                }
            });



            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            //app.UseResponseCaching();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseCors(builder => builder.WithOrigins("www.apirequest.io.").WithMethods("Get","Post").AllowAnyHeader());
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
