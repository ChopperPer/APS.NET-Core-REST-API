using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CityInfo.API.Entities
{
    public class PointOfInterest
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [MaxLength(200)]
        public string Description { get; set; } // We will add this later on when talking about migration in the course

        // We also want some "navigation property" to show the relation to the parent class City,
        // A property with the name City of type City will by convention be regarded as such.
        [ForeignKey("CityId")] // This attribute is not needed since we follow the convention naming, but can make the code more readable
        public City City { get; set; } // Will automagic set the City.Id as the foreign key constraint
        public int CityId { get; set; } // But it is recommended to add a foreign key property anyway. This name follows the convention: <Class-name>Id
    }
}
