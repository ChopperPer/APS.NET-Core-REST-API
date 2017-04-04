using System.Collections.Generic;
using System.Linq;
using CityInfo.API.Entities;

namespace CityInfo.API
{
    /// <summary>
    /// Holding extension methods for the db context
    /// Used e.g. for seeding data to the database on Migration
    /// </summary>
    public static class CityInfoContextExtensions
    {
        /// <summary>
        /// Ensures that we have some startup data in the database
        /// </summary>
        /// <param name="context"></param>
        public static void EnsureSeedDataForContext(this CityInfoContext context)
        {
            // the this decoration above tells the compiler that this method extends the CityInfoCotext

            // First check if we already have any data (if so - don't add any more data)
            if (context.Cities.Any())
            {
                return;
            }

            // Create start data
            var cities = new List<City>()
            {
                new City()
                {
                    Name = "A-köping",
                    Description = "Many A-type persons",
                    PointsOfInterests = new List<PointOfInterest>()
                    {
                        new PointOfInterest()
                        {
                            Name = "A-person 1",
                            Description = "A very A-person"
                        },
                        new PointOfInterest()
                        {
                            Name = "A-person 2",
                            Description = "A slighter less A-person"
                        }
                    }
                },
                new City()
                {
                    Name = "B-köping",
                    Description = "Many B-type persons",
                    PointsOfInterests = new List<PointOfInterest>()
                    {
                        new PointOfInterest()
                        {
                            Name = "B-person 1",
                            Description = "A very B-person"
                        },
                        new PointOfInterest()
                        {
                            Name = "B-person 2",
                            Description = "A slighter less B-person"
                        }
                    }
                }
            };

            // Add these cities to the context
            context.AddRange(cities);
            // And save it to the database
            context.SaveChanges();
        }
    }
}
