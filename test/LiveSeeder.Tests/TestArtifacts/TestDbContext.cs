using Microsoft.EntityFrameworkCore;

namespace LiveSeeder.Tests.TestArtifacts
{
    public class TestDbContext:DbContext
    {
        public DbSet<TestCar> TestCars { get; set; }
        public DbSet<Car> Cars { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<County> Counties { get; set; }

        public TestDbContext(DbContextOptions<TestDbContext> options) : base(options)
        {

        }
    }
}
