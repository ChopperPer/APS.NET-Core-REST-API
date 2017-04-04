using System;
using System.Collections.Generic;
using AutoMapper;
using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CityInfo.API.Controllers
{
    [Route("api/cities")] // Basic routing for this class
    public class PointsOfInterestController : Controller
    {
        private readonly ILogger<PointsOfInterestController> _logger;
        private readonly IMailService _mailService;
        //private CityInfoContext _ctx; // Used only for the in memory data store version 
        private readonly ICityInfoRepository _cityInfoRepository;

        /// <summary>
        /// Ctr, Contstructs the controller and injects the services needed.
        /// </summary>
        /// <param name="logger">A logger service for logging</param>
        /// <param name="mailService">A mail service</param>
        /// <param name="cityInfoRepository">A repository service</param>
        public PointsOfInterestController(ILogger<PointsOfInterestController> logger, IMailService mailService,
             ICityInfoRepository cityInfoRepository/*, CityInfoContext ctx*/)
        {
            _logger = logger;
            // N.b. If you for any reason don't want or can't use Constructor Dependency Injection like above, 
            // you can ask the container direct for a service with something like this: 
            // HttpContext.RequestServices.GetService(ILogger<PointsOfInterestController>)
            _mailService = mailService;
            _cityInfoRepository = cityInfoRepository;
            //_ctx = ctx;
        }

        /// <summary>
        /// Gets all points of interests for a city
        /// </summary>
        /// <param name="cityId">Id of the city to get points of interests for</param>
        /// <returns>A list of points of interest in the requested format (json or xml)</returns>
        [HttpGet("{cityId}/pointsofinterest")]
        public IActionResult GetPointsOfInterest(int cityId)
        {
            try
            {
                // Just testing exception logging
                //throw new Exception("Testing Exception");

                // ------------------------------------------------------------
                // Version 1 - using the in memory data store
                // ------------------------------------------------------------

                // Do we have the city?
                // Models.CityDto city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
                //City city = _ctx.Cities.FirstOrDefault(c => c.Id == cityId);

                //if (city == null)
                //{
                //    _logger.LogInformation($"********************** A City with id {cityId} was not found ***********************");
                //    return NotFound();
                //}
                //return Ok(city.PointsOfInterests);

                // ------------------------------------------------------------
                // Version 2 - using a Repository pattern
                // ------------------------------------------------------------

                //var city = _cityInfoRepository.GetCity(cityId, true);
                //if (city == null)
                //{
                //    _logger.LogInformation($"********************** A City with id {cityId} was not found ***********************");
                //    return NotFound();
                //}
                //var pointsOfInterestDto = new List<PointOfInterestDto>();
                //foreach (var pointOfInterest in city.PointsOfInterests)
                //{
                //    pointsOfInterestDto.Add(new PointOfInterestDto
                //    {
                //        Id = pointOfInterest.Id,
                //        Name = pointOfInterest.Name,
                //        Description = pointOfInterest.Description
                //    });
                //}
                //return Ok(pointsOfInterestDto);

                // ------------------------------------------------------
                // Version 2.1 - with new CityExists method in repo
                // ------------------------------------------------------
                //if (!_cityInfoRepository.CityExists(cityId))
                //{
                //    _logger.LogInformation($"********************** A City with id {cityId} was not found ***********************");
                //    return NotFound();
                //}
                //var pointsOfInterestForCity = _cityInfoRepository.GetPointsOfInterestForCity(cityId);
                //var pointsOfInterestDto = new List<PointOfInterestDto>();
                //foreach (var pointOfInterest in pointsOfInterestForCity)
                //{
                //    pointsOfInterestDto.Add(new PointOfInterestDto
                //    {
                //        Id = pointOfInterest.Id,
                //        Name = pointOfInterest.Name,
                //        Description = pointOfInterest.Description
                //    });
                //}
                //return Ok(pointsOfInterestDto);

                // -----------------------------------------------------------
                // Version 2.2 Using Repo AND AutoMapper
                // -----------------------------------------------------------
                if (!_cityInfoRepository.CityExists(cityId))
                {
                    _logger.LogInformation($"********************** A City with id {cityId} was not found ***********************");
                    return NotFound();
                }
                var pointsOfInterestForCity = _cityInfoRepository.GetPointsOfInterestForCity(cityId);
                var pointsOfInterestDto = Mapper.Map<IEnumerable<PointOfInterestDto>>(pointsOfInterestForCity);
                return Ok(pointsOfInterestDto);

            }
            catch (Exception e)
            {
                _logger.LogCritical($"Exception while getting points of interest for city with id {cityId}.", e);
                //Console.WriteLine(e);
                return StatusCode(500, "A problem happend while handling your request"); 
                // N.b. do not expose any implementaion details to the consumer, like passing the exception or stacktrace
            }
            
        }

        /// <summary>
        /// Get one point of interests for a city
        /// </summary>
        /// <param name="cityId">Id of the city to get points of interests for</param>
        /// <param name="id">Id of the point of interest to get</param>
        /// <returns>A point of interest in the requested format (json or xml)</returns>
        [HttpGet("{cityId}/pointsofinterest/{id}", Name = "GetPointOfInterest")] // Give the url a name to be easier reused e.g. in the POST below
        public IActionResult GetPointOfInterest(int cityId, int id)
        {
            // Do we have the city?
            // -----------------------------------
            // Version 1 - in memory database
            // -----------------------------------
            //CityDto city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);

            // -----------------------------------
            // Version 2.1 - repository pattern
            // -----------------------------------
            //if (!_cityInfoRepository.CityExists(cityId))
            //{
            //    _logger.LogInformation($"********************** A City with id {cityId} was not found ***********************");
            //    return NotFound();
            //}

            //// Do this city have the looked for point of interest?
            //var pointOfInterestForCity = _cityInfoRepository.GetPointOfInterestForCity(cityId, id);
            //if (pointOfInterestForCity == null)
            //{
            //    _logger.LogInformation($"****** A Point of Interest with id {id} was not found in city with id {cityId} ***********");
            //    return NotFound();
            //}
            //var pointOfInterestDto = new PointOfInterestDto
            //{
            //    Id = pointOfInterestForCity.Id,
            //    Name = pointOfInterestForCity.Name,
            //    Description = pointOfInterestForCity.Description
            //};
            //return Ok(pointOfInterestDto);


            //var pointOfInterest = city.PointsOfInterests.FirstOrDefault(p => p.Id == id);
            //if (pointOfInterest == null)
            //{
            //    _logger.LogInformation($"********************** A Point of Interest with id {id} was not found ***********************");
            //    return NotFound();
            //}

            // Ok - we found a POI with the id passed in
            //return Ok(pointOfInterest); // Version 1 - returning the in memory stored DTO

            // Version 2 - mapp repository entity to dto
            //var pointOfInterestDto = new PointOfInterestDto
            //{
            //    Id = pointOfInterest.Id,
            //    Name = pointOfInterest.Name,
            //    Description = pointOfInterest.Description
            //};
            //return Ok(pointOfInterestDto);

            // --------------------------------------------------
            // Version 2.1 - repository pattern AND AutoMapper
            // --------------------------------------------------
            if (!_cityInfoRepository.CityExists(cityId))
            {
                _logger.LogInformation($"********************** A City with id {cityId} was not found ***********************");
                return NotFound();
            }

            // Do this city have the looked for point of interest?
            var pointOfInterestForCity = _cityInfoRepository.GetPointOfInterestForCity(cityId, id);
            if (pointOfInterestForCity == null)
            {
                _logger.LogInformation($"****** A Point of Interest with id {id} was not found in city with id {cityId} ***********");
                return NotFound();
            }

            var pointOfInterestDto = Mapper.Map<PointOfInterestDto>(pointOfInterestForCity);
            return Ok(pointOfInterestDto);

        }

        /// <summary>
        /// Creates a new resource of type PointOfInterest and adds it to a city.
        /// </summary>
        /// <param name="cityId">The city to add a point of interest to</param>
        /// <param name="pointOfInterest">The <see cref="PointOfInterestForCreationDto"/> to create and add</param>
        /// <returns></returns>
        [HttpPost("{cityId}/pointsofinterest")]
        public IActionResult CreatePointOfInterest(int cityId,
            [FromBody] PointOfInterestForCreationDto pointOfInterest)
        {
            // Verify if the body context is correct
            if (pointOfInterest == null)
            {
                return BadRequest();
            }

            // Verify that we do not have the same value on both name and description
            if (pointOfInterest.Name == pointOfInterest.Description)
            {
                ModelState.AddModelError("Description", "Description value must be different from the name value.");
            }

            // Verify if any rules of the model properties has been broken 
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // -------------------------------------------------
            // Version 1 - using in memory data store
            // -------------------------------------------------
            //// Verify that the city resource to add POI to exists
            //CityDto city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
            //if (city == null)
            //{
            //    return NotFound();
            //}

            //// First approach - for demo purposes. To be improved
            //// Get the last (highest) POI id
            //int maxPointOfInterestId = CitiesDataStore.Current.Cities.SelectMany(
            //   c => c.PointsOfInterests).Max(p => p.Id);
            //// Create a new POI, icluded with a new Id
            //var finalPointOfInterest = new PointOfInterestDto
            //{
            //    Id = ++maxPointOfInterestId, // Add +1 to the existing id
            //    Name = pointOfInterest.Name,
            //    Description = pointOfInterest.Description
            //};
            //city.PointsOfInterests.Add(finalPointOfInterest);

            //// Recommendation is to return an URL to the newly created resource, CreatedAtRoute (return code 201 - Created),
            //// And the Id of the city and the new POI, and the complete createded object (resource)
            //return CreatedAtRoute("GetPointOfInterest", new {cityId, id = finalPointOfInterest.Id},
            //    finalPointOfInterest);

            // ----------------------------------------------------
            // Version 2 - using repository pattern and AutoMapper
            // ----------------------------------------------------
            if (!_cityInfoRepository.CityExists(cityId))
            {
                return NotFound();
            }
            var pointOfInterestToCreate = Mapper.Map<Entities.PointOfInterest>(pointOfInterest);
            _cityInfoRepository.AddPointOfInterestForCity(cityId, pointOfInterestToCreate);
            if (!_cityInfoRepository.Save())
            {
                return StatusCode(500, "A problem happened while handling your request.");
            }
            // Mapp the created entity back to a DTO and return it
            var createdPointOfInterest = Mapper.Map<PointOfInterestDto>(pointOfInterestToCreate);

            return CreatedAtRoute("GetPointOfInterest", new { cityId, id = createdPointOfInterest.Id },
                createdPointOfInterest);
        }

        /// <summary>
        /// Do a FULL update of a Point of Interest resource
        /// </summary>
        /// <param name="cityId">The city holding the point of interest</param>
        /// <param name="id">The id of the point of interest to update</param>
        /// <param name="pointOfInterestDto">The point of interest object holding the updtaded data</param>
        /// <returns>204 - No Content on success</returns>
        [HttpPut("{cityId}/pointsofinterest/{id}")]
        public IActionResult UpdatePointOfInterest(int cityId, int id,
            [FromBody] PointOfInterestForUpdateDto pointOfInterestDto)
        {
            // Verify the input
            if (pointOfInterestDto == null)
            {
                return BadRequest();
            }

            // Verify that we do not have the same value on both name and description
            if (pointOfInterestDto.Name == pointOfInterestDto.Description)
            {
                ModelState.AddModelError("Description", "Description value must be differen from the name value.");
            }

            // Verify if any rules of the model properties has been broken 
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // ---------------------------------------------
            // Version 1 - using in memory data store
            // ---------------------------------------------

            //// Verify that the city resource to update POI in exists in the data store
            //CityDto city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
            //if (city == null)
            //{
            //    return NotFound();
            //}

            //// Verify that the POI exists
            //PointOfInterestDto poi = city.PointsOfInterests.FirstOrDefault(p => p.Id == id);
            //if (poi == null)
            //{
            //    return NotFound();
            //}

            //// All is good - do a FULL update and return 204 - No Content
            //poi.Name = pointOfInterestDto.Name;
            //poi.Description = pointOfInterestDto.Description;

            //return NoContent(); // 204 - No Content (we usually don't need to return anything since the consumer already have all information, he just sent it - including the id)

            // ------------------------------------------------------
            // Version 2 - with Repository pattern and AutoMapper
            // ------------------------------------------------------

            // Verify that we have the city and the POI to update
            if (!_cityInfoRepository.CityExists(cityId))
            {
                return NotFound();
            }
            Entities.PointOfInterest pointOfInterestEntity = _cityInfoRepository.GetPointOfInterestForCity(cityId, id);
            if (pointOfInterestEntity == null)
            {
                return NotFound();
            }

            // Update the destination/target object (the entity) with the values from the source (the DTO)
            Mapper.Map(pointOfInterestDto, pointOfInterestEntity); 
            
            // The entity is tracked by the context, and is now in a modified state, so it will be saved to the repo when we call Save
            if (!_cityInfoRepository.Save())
            {
                return StatusCode(500, "A problem happened while handling your request.");
            }
            return NoContent();
        }

        /// <summary>
        /// Do a PARTIAL update of a Point of Interest resource
        /// </summary>
        /// <param name="cityId">The city holding the point of interest</param>
        /// <param name="id">The id of the point of interest to update</param>
        /// <param name="patchDocument">A <see cref="JsonPatchDocument{TModel}"/> holding a list of operations to be applyed to the point of interest</param>
        /// <returns>204 - No Content on success</returns>
        [HttpPatch("{cityId}/pointsofinterest/{id}")]
        public IActionResult PartiallyUpdatePointOfInterest(int cityId, int id,
            [FromBody] JsonPatchDocument<PointOfInterestForUpdateDto> patchDocument )
        {
            // Verify the input
            if (patchDocument == null)
            {
                return BadRequest();
            }

            // -------------------------------------------
            // Version 1 - using an in memory data store
            // -------------------------------------------

            //CityDto city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
            //if (city == null)
            //{
            //    return NotFound();
            //}

            //// Verify that the POI exists
            //PointOfInterestDto poi = city.PointsOfInterests.FirstOrDefault(p => p.Id == id);
            //if (poi == null)
            //{
            //    return NotFound();
            //}

            //// Create a work copy of the existing poi from the data store
            //var pointOfInterestToPatch = new PointOfInterestForUpdateDto
            //{
            //    Name = poi.Name,
            //    Description = poi.Description
            //};

            //// Apply the patch to the work copy
            //patchDocument.ApplyTo(pointOfInterestToPatch, ModelState); // Pass in ModelState so we can check after the ApplyTo that we didn't get any errors

            //// Verify that all went well when applying the changes - but hey! This validated only the state of the JsonPatchDocument and this is almoust always valid...
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}

            //// Verify that we do not have the same value on both name and description
            //if (pointOfInterestToPatch.Name == pointOfInterestToPatch.Description)
            //{
            //    ModelState.AddModelError("Description", "Description value must be differen from the name value.");
            //}

            //// validate also the the pointOfInterestToPatch object
            //TryValidateModel(pointOfInterestToPatch);

            //// Did the validation pass?
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}

            //// All is good - set the patched values to the object in the store
            //poi.Name = pointOfInterestToPatch.Name;
            //poi.Description = pointOfInterestToPatch.Description;

            //// Return OK (with no content)
            //return NoContent();

            // ------------------------------------------------------
            // Version 2 - with Repository pattern and AutoMapper
            // ------------------------------------------------------

            // Verify that we have the city and the POI to update
            if (!_cityInfoRepository.CityExists(cityId))
            {
                return NotFound();
            }
            Entities.PointOfInterest pointOfInterestEntity = _cityInfoRepository.GetPointOfInterestForCity(cityId, id);
            if (pointOfInterestEntity == null)
            {
                return NotFound();
            }

            // Create a working copy dto and map the Entity values to it
            PointOfInterestForUpdateDto pointOfInterestForUpdateDto =
                Mapper.Map<PointOfInterestForUpdateDto>(pointOfInterestEntity);

            // Apply the patch to the working copy (same coding as before)
            // Pass in ModelState so we can check after the ApplyTo that we didn't get any errors
            patchDocument.ApplyTo(pointOfInterestForUpdateDto, ModelState); 

            // Verify that all went well when applying the changes 
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
                // but hey!This validates only the state of the JsonPatchDocument and this is almoust always valid, 
                // so we'll add some more validation below.
            }

            // Verify that we do not have the same value on both name and description
            if (pointOfInterestForUpdateDto.Name == pointOfInterestForUpdateDto.Description)
            {
                ModelState.AddModelError("Description", "Description value must be differen from the name value.");
            }

            // validate also the the pointOfInterestToPatch object
            TryValidateModel(pointOfInterestForUpdateDto);

            // Did the validation pass?
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Now map back the patched working copy to an entity
            Mapper.Map(pointOfInterestForUpdateDto, pointOfInterestEntity);

            // The entity is tracked by the context, and is now in a modified state, so it will be saved to the repo when we call Save
            if (!_cityInfoRepository.Save())
            {
                return StatusCode(500, "A problem happened while handling your request.");
            }
            return NoContent();
        }

        /// <summary>
        /// Deletes a point of interest resource
        /// </summary>
        /// <param name="cityId">Id of the city holding the resource</param>
        /// <param name="id">Id of the resoruce to delete</param>
        /// <returns>204 - No Content on success</returns>
        [HttpDelete("{cityId}/pointsofinterest/{id}")]
        public IActionResult DeletePointOfInterest(int cityId, int id)
        {
            // ------------------------------------------
            // Version 1 - using an in memory data store
            // ------------------------------------------
            
            // Verify that the resource exists
            //CityDto city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
            //if (city == null)
            //{
            //    return NotFound();
            //}

            //// Verify that the POI exists
            //PointOfInterestDto poi = city.PointsOfInterests.FirstOrDefault(p => p.Id == id);
            //if (poi == null)
            //{
            //    return NotFound();
            //}

            //city.PointsOfInterests.Remove(poi);
            //_mailService.Send("Point of interest was deleted", $"POI: {poi.Name} with id {poi.Id} was deleted");

            //return NoContent();

            // ----------------------------------------------------
            // Version 2 - using repoitory pattern and AutoMapper
            // ----------------------------------------------------
            // Verify that we have the city and the POI to update
            if (!_cityInfoRepository.CityExists(cityId))
            {
                return NotFound();
            }
            Entities.PointOfInterest pointOfInterestEntity = _cityInfoRepository.GetPointOfInterestForCity(cityId, id);
            if (pointOfInterestEntity == null)
            {
                return NotFound();
            }

            // Alternative 1 - remove the poi using the city entity
            //Entities.City cityEntity = _cityInfoRepository.GetCity(cityId, true);
            //cityEntity.PointsOfInterests.Remove(pointOfInterestEntity);

            // Alternative 2 - remove the poi using a new delete method in the repository
            _cityInfoRepository.DeletePointOfInterest(pointOfInterestEntity);

            // The entity is tracked by the context, and is now in a modified state, so it will be saved to the repo when we call Save
            if (!_cityInfoRepository.Save())
            {
                return StatusCode(500, "A problem happened while handling your request.");
            }

            // Send a (fake) mail to inform that a resource where removed
            _mailService.Send("Point of interest was deleted",
                $"POI: {pointOfInterestEntity.Name} with id {pointOfInterestEntity.Id} was deleted");

            return NoContent();
        }
    }
}
