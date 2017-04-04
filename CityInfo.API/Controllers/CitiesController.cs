using System.Collections.Generic;
using AutoMapper;
using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers
{
    // Using the Route attribute on the class level lets you set the common part of the uri on the
    // controller class so you don't need repeat it on each single action below.
    // Use either one of these attribute constructs
    //[Route("api/[controller]")] // adds the controller name, disadvantage could be that you might want to rename the controller but not change the api?
    [Route("api/cities")] // Adds a hard coded name to the uri, advantage: you can refactor/rename controller without breaking the API
    public class CitiesController : Controller
    {
        private readonly ICityInfoRepository _cityInfoRepository;

        public CitiesController(ICityInfoRepository cityInfoRepository)
        {
            _cityInfoRepository = cityInfoRepository;
        }

        /// <summary>
        /// Get all cities
        /// </summary>
        /// <returns>A list of cities in the requested format (text, json or xml)</returns>
        [HttpGet] // GET: api/cities/
        //public JsonResult GetCities() // Modified to IActionResult to easier handle response codes, n.b. JsonResults inherites ActionResult that implements IActionResults
        public IActionResult GetCities()
        {
            // -----------------------------------------
            // Version 1: Dummy return object
            // -----------------------------------------
            //return new JsonResult(new List<object>
            //{
            //    new { id=1, Name="Östersund"},
            //    new { id=2, Name="Sundsvall"}
            //});

            // -----------------------------------
            // Version 2: In memory data store
            // -----------------------------------

            //List<CityDto> cities = CitiesDataStore.Current.Cities;
            ////return new JsonResult(cities);
            //return Ok(cities);

            // -----------------------------------
            // Version 3: entities and repository pattern
            // -----------------------------------

            //var cityEntities = _cityInfoRepository.GetCities();
            // We now need to map these entities to a matching DTO class
            //var results = new List<CityWithoutPointsOfInterestDto>();
            //foreach (var cityEntity in cityEntities)
            //{
            //    results.Add(new CityWithoutPointsOfInterestDto

            //    {
            //        Id = cityEntity.Id,
            //        Description = cityEntity.Description,
            //        Name = cityEntity.Name
            //    });
            //}
            // Using AutoMapper instead of the manual mapping above, AutoMapper is configured in the Startup class
            var cityEntities = _cityInfoRepository.GetCities();
            var results = Mapper.Map<IEnumerable<CityWithoutPointsOfInterestDto>>(cityEntities);
            return Ok(results);
        }

        /// <summary>
        /// Returnes a single city, with or without points of interests
        /// </summary>
        /// <param name="id">The id of the city, part of the URI</param>
        /// <param name="includePointsOfInterest">boolean, true to include points of interests, false if not</param>
        [HttpGet("{id}", Name = "GetCity")] // GET: api/cities/{id}, n.b. the Name is used in the return of the creation (POST)
        public IActionResult GetCity(int id, bool includePointsOfInterest = false)
        {
            // Version 1 - using an in memory data store
            //CityDto city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == id);
            //if (city == null)
            //{
            //    return NotFound();
            //}
            //return Ok(city);

            // ----------------------------------
            // Version 2 - using a repository
            // -----------------------------------
            //var city = _cityInfoRepository.GetCity(id, includePointsOfInterest);
            //if (city == null)
            //{
            //    return NotFound();
            //}
            
            //if (includePointsOfInterest)
            //{
            //    var pointOfInterestsDto = new List<PointOfInterestDto>();
            //    foreach (var pointOfInterest in city.PointsOfInterests)
            //    {
            //        pointOfInterestsDto.Add(new PointOfInterestDto
            //        {
            //            Id = pointOfInterest.Id,
            //            Name = pointOfInterest.Name,
            //            Description = pointOfInterest.Description
            //        });
            //    }
            //    var cityDto = new CityDto { Id = city.Id, Name = city.Name, Description = city.Description, PointsOfInterests = pointOfInterestsDto };
            //    return Ok(cityDto);

            //}

            //var cityWithoutPoiDto = new CityWithoutPointsOfInterestDto {Id = city.Id, Name = city.Name, Description = city.Description};
            //return Ok(cityWithoutPoiDto);

            // -------------------------------------------------
            // Version 2b - using a repository AND AutoMapper
            // -------------------------------------------------
            var city = _cityInfoRepository.GetCity(id, includePointsOfInterest);
            if (city == null)
            {
                return NotFound();
            }

            if (includePointsOfInterest)
            {
                var cityResult = Mapper.Map<CityDto>(city);
                return Ok(cityResult);
            }

            var cityWithoutPoiResult = Mapper.Map<CityWithoutPointsOfInterestDto>(city);
            return Ok(cityWithoutPoiResult);
        }

        [HttpPost]
        public IActionResult AddCity([FromBody] CityForCreationDto cityForCreationDto)
        {
            // Verify the input
            if (cityForCreationDto == null)
            {
                return BadRequest();
            }

            // Verify that we do not have the same value on both name and description
            if (cityForCreationDto.Name == cityForCreationDto.Description)
            {
                ModelState.AddModelError("Description", "Description value must be different from the name value.");
            }

            // Verify if any other rules of the model properties has been broken 
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Mapp DTO to daatabase Entity
            Entities.City cityEntity = Mapper.Map<Entities.City>(cityForCreationDto);

            // Add to DB context
            _cityInfoRepository.AddCity(cityEntity);

            // Try to save it and return the result
            if (!_cityInfoRepository.Save())
            {
                return StatusCode(500, "A problem happened while handling your request.");
            }
            // If all went well, Mapp the created entity back to a DTO and return the URI to it
            CityDto createdCity = Mapper.Map<CityDto>(cityEntity);
            return CreatedAtRoute("GetCity", new { id = createdCity.Id }, createdCity);
        }

    }
}
