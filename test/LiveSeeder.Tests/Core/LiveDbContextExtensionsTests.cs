using System.Linq;
using Dapper;
using LiveSeeder.Core;
using LiveSeeder.Tests.TestArtifacts;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace LiveSeeder.Tests.Core
{
    [TestFixture]
    public class LiveDbContextExtensionsTests
    {
        private TestDbContext _testDbContext;
        [OneTimeSetUp]
        public void Init()
        {
            TestInitializer.InitDbContext();
        }

        [SetUp]
        public void SetUp()
        {
            _testDbContext = TestInitializer.ServiceProvider.GetService<TestDbContext>();
        }

        [Test]
        public void should_Add()
        {
            _testDbContext.SeedAdd<TestCar>(typeof(TestCar).Assembly).Wait();
            var testCars = _testDbContext.TestCars.ToList();
            Assert.True(testCars.Any());
        }

        [Test]
        public void should_AddOrUpdate()
        {
            _testDbContext.SeedAddOrUpdate<Company>(typeof(Company).Assembly).Wait();
            var companies = _testDbContext.Companies.ToList();
            Assert.AreEqual("Microsoft Azure", companies.First(x => x.Id == 1).Name);
            Assert.AreEqual("Netflix Stream", companies.First(x => x.Id == 3).Name);
            Assert.AreEqual("Facebook", companies.First(x => x.Id == 4).Name);
        }

        [Test]
        public void should_Merge()
        {
            _testDbContext.SeedMerge<County>(typeof(County).Assembly).Wait();
            var companies = _testDbContext.Counties.ToList();
            Assert.AreEqual("Nairobi", companies.First(x => x.Id == 47).Name);
            Assert.AreEqual("Bomet", companies.First(x => x.Id == 22).Name);
        }

        [Test]
        public void should_Clear()
        {
            _testDbContext.SeedClear<Car>().Wait();
            var cars = _testDbContext.Cars.ToList();
            Assert.False(cars.Any());
        }

        [Test]
        public void should_Clear_By_Predicate()
        {
            _testDbContext.SeedMerge<Car>(typeof(Car).Assembly,"|",@"Seed","Car.csv").Wait();

            _testDbContext.SeedClear<Car>(x=>x.Name.Contains("2")).Wait();
            var cars = _testDbContext.Cars.ToList();
            Assert.False(cars.Any(x=>x.Name.Contains("2")));
        }

        [Test]
        public void should_New_Only()
        {
            _testDbContext.SeedMerge<Company>(typeof(Company).Assembly).Wait();
            _testDbContext.SeedNewOnly<Company>(typeof(Company).Assembly,",",@"Seed\Other","newcompany.csv").Wait();
            var companies = _testDbContext.Companies.ToList();
            Assert.True(companies.Any(x=>x.Name.ToLower()=="Tesla Corp".ToLower()));
            Assert.True(companies.Any(x=>x.Name.ToLower()=="Suzuki Corp".ToLower()));
        }
    }
}
