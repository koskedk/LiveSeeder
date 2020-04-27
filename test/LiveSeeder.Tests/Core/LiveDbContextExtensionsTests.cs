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
        private TestDbContext TestDbContext;
        [OneTimeSetUp]
        public void Init()
        {
            TestInitializer.InitDbContext();
        }

        [SetUp]
        public void SetUp()
        {
            TestDbContext = TestInitializer.ServiceProvider.GetService<TestDbContext>();
        }

        [Test]
        public void should_Add()
        {
            TestDbContext.SeedAdd<TestCar>(typeof(TestCar).Assembly).Wait();
            var testCars = TestDbContext.TestCars.ToList();
            Assert.True(testCars.Any());
        }

        [Test]
        public void should_AddOrUpdate()
        {
            TestDbContext.SeedAddOrUpdate<Company>(typeof(Company).Assembly).Wait();
            var companies = TestDbContext.Companies.ToList();
            Assert.AreEqual("Microsoft Azure", companies.First(x => x.Id == 1).Name);
            Assert.AreEqual("Netflix Stream", companies.First(x => x.Id == 3).Name);
            Assert.AreEqual("Facebook", companies.First(x => x.Id == 4).Name);
        }

        [Test]
        public void should_Merge()
        {
            TestDbContext.SeedMerge<County>(typeof(County).Assembly).Wait();
            var companies = TestDbContext.Counties.ToList();
            Assert.AreEqual("Nairobi", companies.First(x => x.Id == 47).Name);
            Assert.AreEqual("Bomet", companies.First(x => x.Id == 22).Name);
        }

        [Test]
        public void should_Clear()
        {
            TestDbContext.SeedClear<Car>().Wait();
            var cars = TestDbContext.Cars.ToList();
            Assert.False(cars.Any());
        }
    }
}
