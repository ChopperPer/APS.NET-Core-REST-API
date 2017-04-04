using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityInfo.API
{
    public class CitiesDataStore
    {
        public static CitiesDataStore Current { get; } = new CitiesDataStore(); // Ensure this property is immutal
        public List<Models.CityDto> Cities { get; set; }

        public CitiesDataStore()
        {
            // Init some dummy data
            Cities = new List<Models.CityDto>()
            {
                new Models.CityDto()
                {
                    Id = 1,
                    Name = "Östersund",
                    Description = "The pearl of Jamtland",
                    PointsOfInterests = new List<Models.PointOfInterestDto>()
                    {
                        new Models.PointOfInterestDto()
                        {
                            Id = 1,
                            Name = "Jamtli",
                            Description = "Otudoor museum with a lot of gotes"
                        },
                        new Models.PointOfInterestDto()
                        {
                            Id = 2,
                            Name = "Utsiktstornet",
                            Description = "View Tower with a view of the whole city"
                        }
                    }
                },
                new Models.CityDto()
                {
                    Id = 2,
                    Name = "Brunflo",
                    Description = "South bound",
                    PointsOfInterests = new List<Models.PointOfInterestDto>()
                    {
                        new Models.PointOfInterestDto()
                        {
                            Id = 1,
                            Name = "Ragnarök",
                            Description = "Burned down restaurang"
                        }
                    }
                },
                new Models.CityDto()
                {
                    Id = 3,
                    Name = "Åre",
                    Description = "The ski metropol of Jamtland",
                    PointsOfInterests = new List<Models.PointOfInterestDto>()
                    {
                        new Models.PointOfInterestDto()
                        {
                            Id = 1,
                            Name = "Åreskutan",
                            Description = "The big ski mountain"
                        },
                        new Models.PointOfInterestDto()
                        {
                            Id = 2,
                            Name = "Holiday Club",
                            Description = "Recreation center and hotel"
                        }
                    }
                }
            };
        }
    }
}
