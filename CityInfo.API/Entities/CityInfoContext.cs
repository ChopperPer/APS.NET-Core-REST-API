using Microsoft.EntityFrameworkCore;

namespace CityInfo.API.Entities
{
    public sealed class CityInfoContext : DbContext
    {
        /// <summary>
        /// Ctr, Ensures that the database is created and/or updated to the latest version (migration).
        /// </summary>
        /// <param name="options">A <see cref="DbContextOptions{TContext}"/>that e.g. holds the connection string</param>
        public CityInfoContext(DbContextOptions<CityInfoContext> options)
            : base(options)
        {
            // Ensure that the database is created if it doesn't exist
            //Database.EnsureCreated();

            // An alternative to above EnsureCreated() is to use Migrations. 
            // If we have created a migration (Package Manager Console, write: Add-Migration <name-of-the-migration>
            // we can use it here and it will create or update the database to the latest version.
            Database.Migrate();
        }

        public DbSet<City> Cities { get; set; } // DbSet used to query and create entities, e.g. you can use LINQ expression that will be translated to SQL queries

        public DbSet<PointOfInterest> PointsOfInterest { get; set; }
    }
}
