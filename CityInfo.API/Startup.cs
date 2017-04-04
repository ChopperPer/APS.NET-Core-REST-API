using CityInfo.API.Entities;
using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NLog.Extensions.Logging;

namespace CityInfo.API
{
    public class Startup
    {
        public static IConfigurationRoot Configuration; // Needed to read the configuration file(s)

        public Startup(IHostingEnvironment env)
        {
            // Create a configuration builder to build the configuration from our appSettings.json file
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appSettings.json", optional: false, reloadOnChange: true)
                // Add the settings file in the right order (order of hierarchy)
                .AddJsonFile($"appSettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
                // Values in this file, if it exists, will overide any previous values.
                .AddEnvironmentVariables(); // To be able to use environment variables in our code, this will override above setting if it exists (is last in the chain here)

            Configuration = builder.Build(); // Now we have the values from our config file and environment variable in this variable
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        // You can also configure the added services here.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            // Services added here are available for dependency injection in other part of the solution

            // Add MVC Services
            services.AddMvc();

            // Alternative to add more output formatters (json formatter is already added by default)
            services.AddMvc()
                .AddMvcOptions(o => o.OutputFormatters.Add(
                    new XmlDataContractSerializerOutputFormatter())); // Now we in the Accept header can use both application/json and application/xml.

            // Add our own mail service, it's lightweight and stateless so lets use AddTransient. We have 2 concrete classes implementing this, so you can use either one.
            //services.AddTransient<IMailService, LocalMailService>();
            //services.AddTransient<IMailService, CloudMailService>();

            // Lets choose the mail service based on the compile settings Debug och Release
            // Note which one is high lighted based on the selection ob build (Debug or Release)
#if DEBUG
            services.AddTransient<IMailService, LocalMailService>();
#else
            services.AddTransient<IMailService, CloudMailService>();
#endif
            // Add a database context to be able to talk to the database
            //var connectionString = @"Server=(localdb)\mssqllocaldb;Database=CityInfoDB;Trusted_Connection=True;";
            // Use connection string from appSettings.config (or from an environment variable if we add one) instead:
            var connectionString = Configuration.GetConnectionString("cityInfoDBConnectionString");
            services.AddDbContext<CityInfoContext>(o => o.UseSqlServer(connectionString));

            services.AddScoped<ICityInfoRepository, CityInfoRepository>(); // Lets have the repo created upon each request
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        // Here we specify how the application will respond to individual HTTP requests by adding midleware 
        // to the pipeline.
        // We also configures services that are bulit in (like the Logger) or added above to the IOC container
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory,
            CityInfoContext cityInfoContext)
        {
            loggerFactory.AddConsole();
            loggerFactory.AddDebug(); // Default logging level is information and above.

            // Add the 3:rd party provider we shoose to use - NLog (see file nlog.config)
            //loggerFactory.AddProvider(new NLogLoggerProvider());
            // Above AddProvicer can also be done with an extension method:
            loggerFactory.AddNLog();

            // Add different midleware to different environments
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage(); // An example of a middleware added to the pipeline only when indeveloper environment
            }
            else
            {
                app.UseExceptionHandler(); // This middleware is used for the other environments (Staging and Production)
            }

            // Use the extension method on CityInfoContext to ensure we have startup data
            cityInfoContext.EnsureSeedDataForContext();

            // Add midleware to show status codes
            app.UseStatusCodePages();

            // Configure AutoMapper
            AutoMapper.Mapper.Initialize(cfg =>
            {
                // Add the decired mappings here, format: cfg.CreateMap<TSource, TTarget>();
                cfg.CreateMap<City, CityWithoutPointsOfInterestDto>(); // Mapps the database entity City to the Dto without any POI
                cfg.CreateMap<City, CityDto>(); // Mapps a City entity to the City DTO with POIs
                cfg.CreateMap<CityForCreationDto, City>();
                cfg.CreateMap<PointOfInterest, PointOfInterestDto>(); // Mapping a POI entity to its DTO
                cfg.CreateMap<PointOfInterestForCreationDto, Entities.PointOfInterest>(); // Mapping for creating a repository entity from a DTO
                cfg.CreateMap<PointOfInterestForUpdateDto, Entities.PointOfInterest>(); // Mapping Update DTO => Entity
                cfg.CreateMap<Entities.PointOfInterest, PointOfInterestForUpdateDto>(); // Mapping Entity => Update DTO
            });

            // Add the midleware MVC Core
            app.UseMvc();

            // Use to test throwing an execption to test above midlewares different exception handling. (set the environment variable in project setting)
            // N.b. you will need to restart VS in order for it to nofity the change of environment variable
            //app.Run((context) =>
            //{
            //    throw new Exception("Testing exeption");
            //});

            // First test to only send a text response back.
            //app.Run(async (context) =>
            //{
            //    await context.Response.WriteAsync("Hello World!");
            //});
        }
    }
}
