using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CityInfo.API.Models
{
    public class PointOfInterestForUpdateDto
    {
        [Required(ErrorMessage = "You must provide a name value")]
        [MaxLength(50, ErrorMessage = "Name value is longer then max length of 50 characters")]
        public string Name { get; set; }

        [MaxLength(200)] // N.b. this rule has a default error message so we don't accually need to add one here.
        public string Description { get; set; }
    }
}
