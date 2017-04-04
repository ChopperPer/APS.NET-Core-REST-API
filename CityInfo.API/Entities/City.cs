using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CityInfo.API.Entities
{
    /// <summary>
    /// Entity class representing a city in the repository.
    /// </summary>
    public class City
    {
        [Key] // Key annotation not needed for a field named Id, but makes the entity class more clear to read.
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Identity = A new key will be generated when a city is added
        public int Id { get; set; }
        // Id is by convention automatic regarded as the Primary Key, 
        // but it can be a good idea to annotate it anyway (for clearity)

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [MaxLength(200)]
        public string Description { get; set; }

        public ICollection<PointOfInterest> PointsOfInterests { get; set; }
        = new List<PointOfInterest>(); // Initialize the list to avoid null reference problems later on
    }
}
