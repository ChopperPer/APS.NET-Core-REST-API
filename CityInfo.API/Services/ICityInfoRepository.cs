using System.Collections.Generic;
using CityInfo.API.Entities;

namespace CityInfo.API.Services
{
    public interface ICityInfoRepository
    {
        // To GET data use either IEnumerable<> or IQueryable<> as return
        // IQueryable might leek persistence logic out of this abstraction, but is easier to keep building on for the cunsumer, e.g. adding OrderBy etc.

        /// <summary>
        /// Returns all cities from the repository
        /// </summary>
        IEnumerable<City> GetCities();

        /// <summary>
        /// Returns one city from the repository
        /// </summary>
        /// <param name="cityId">Id of the city to return</param>
        /// <param name="includePointsOfInterest">true if we want to also return the points of interest for the city</param>
        City GetCity(int cityId, bool includePointsOfInterest);

        /// <summary>
        /// Returns all points of interests for a city
        /// </summary>
        /// <param name="cityId">Id for the city to get points of interest for</param>
        IEnumerable<PointOfInterest> GetPointsOfInterestForCity(int cityId);

        /// <summary>
        /// Returns one specific point of interest for a city
        /// </summary>
        /// <param name="cityId">Id of the city to get the point of interest from</param>
        /// <param name="pointOfInterestId">Id of the point of interest</param>
        PointOfInterest GetPointOfInterestForCity(int cityId, int pointOfInterestId);

        /// <summary>
        /// Verifies if a city exists in the reposotory
        /// </summary>
        /// <param name="cityId">The id of the city to verify</param>
        /// <returns>true if the city exists, false if not</returns>
        bool CityExists(int cityId);

        /// <summary>
        /// Adds a <see cref="City"/> to the repository
        /// </summary>
        /// <param name="city">The <see cref="City"/> to add</param>
        void AddCity(City city);

        /// <summary>
        /// Adds a <see cref="PointOfInterest"/> to the repository
        /// </summary>
        /// <param name="cityId">Id of the city to add the point of interest to</param>
        /// <param name="pointOfInterest">The <see cref="PointOfInterest"/> to add</param>
        void AddPointOfInterestForCity(int cityId, PointOfInterest pointOfInterest);

        /// <summary>
        /// Deletes a point of interest
        /// </summary>
        /// <param name="pointOfInterest">The <see cref="PointOfInterest"/> to delete</param>
        void DeletePointOfInterest(PointOfInterest pointOfInterest);

        /// <summary>
        /// Saves the in memory representation to the database
        /// </summary>
        /// <returns>true if the save operation went well</returns>
        bool Save();
    }
}
