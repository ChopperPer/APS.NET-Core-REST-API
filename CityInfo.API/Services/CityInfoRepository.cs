using System.Collections.Generic;
using System.Linq;
using CityInfo.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace CityInfo.API.Services
{
    public class CityInfoRepository : ICityInfoRepository
    {
        private CityInfoContext _context;
        public CityInfoRepository(CityInfoContext context)
        {
            _context = context;
        }
        public IEnumerable<City> GetCities()
        {
            return _context.Cities.OrderBy(c => c.Name).ToList(); 
            // The toList() will force the query to be executed right here.
        }

        public City GetCity(int cityId, bool includePointsOfInterest)
        {
            if (includePointsOfInterest)
            {
                return _context.Cities.Include(c => c.PointsOfInterests).FirstOrDefault(c => c.Id == cityId); 
                // FirstOrDefault will force the q to be executed
            }
            return _context.Cities.FirstOrDefault(c => c.Id == cityId);
        }

        public IEnumerable<PointOfInterest> GetPointsOfInterestForCity(int cityId)
        {
            return _context.PointsOfInterest.Where(p => p.CityId == cityId).ToList();
        }

        public PointOfInterest GetPointOfInterestForCity(int cityId, int pointOfInterestId)
        {
            return
                _context.PointsOfInterest.FirstOrDefault(p => p.CityId == cityId && p.Id == pointOfInterestId);

        }

        public bool CityExists(int cityId)
        {
            return _context.Cities.FirstOrDefault(c => c.Id == cityId) != null;
        }

        public void AddPointOfInterestForCity(int cityId, PointOfInterest pointOfInterest)
        {
            var city = GetCity(cityId, false);
            city.PointsOfInterests.Add(pointOfInterest); 
            // This only adds the poi to the in memory representation of the object, nothing is added to the DB yet
        }

        public bool Save()
        {
            return (_context.SaveChanges() >= 0);
        }

        public void AddCity(City city)
        {
            _context.Add(city);  // Begins tracking this entity. It will be saved upon the next SaveChanges           
        }

        public void DeletePointOfInterest(PointOfInterest pointOfInterest)
        {
            _context.Remove(pointOfInterest); // Begins tracking this entity. Changes be saved upon the next SaveChanges 
        }
    }
}
