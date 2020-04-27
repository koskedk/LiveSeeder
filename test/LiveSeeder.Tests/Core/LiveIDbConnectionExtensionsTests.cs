using System.Linq;
using Dapper;
using LiveSeeder.Core;
using LiveSeeder.Tests.TestArtifacts;
using NUnit.Framework;

namespace LiveSeeder.Tests.Core
{
    [TestFixture]
    public class LiveIDbConnectionExtensionsTests
    {
        [OneTimeSetUp]
        public void Init()
        {
            TestInitializer.InitIDbConnection();
        }
        [Test]
        public void should_Add()
        {
            TestInitializer.Connection.SeedAdd<TestCar>(typeof(TestCar).Assembly).Wait();
            var testCars = TestInitializer.Connection.Query<TestCar>($"SELECT * FROM {nameof(TestCar)}");
            Assert.True(testCars.Any());
        }

        [Test]
        public void should_AddOrUpdate()
        {
            TestInitializer.Connection.SeedAddOrUpdate<Company>(typeof(Company).Assembly).Wait();
            var companies = TestInitializer.Connection.Query<Company>($"SELECT * FROM {nameof(Company)}").ToList();
            Assert.AreEqual("Microsoft Azure", companies.First(x => x.Id == 1).Name);
            Assert.AreEqual("Netflix Stream", companies.First(x => x.Id == 3).Name);
            Assert.AreEqual("Facebook", companies.First(x => x.Id == 4).Name);
        }

        [Test]
        public void should_Merge()
        {
            TestInitializer.Connection.SeedMerge<County>(typeof(County).Assembly).Wait();
            var counties = TestInitializer.Connection.Query<County>($"SELECT * FROM {nameof(County)}").ToList();
            Assert.AreEqual("Nairobi", counties.First(x => x.Id == 47).Name);
            Assert.AreEqual("Bomet", counties.First(x => x.Id == 22).Name);
        }

        [Test]
        public void should_Clear()
        {
            TestInitializer.Connection.SeedClear<Car>().Wait();
            var cars = TestInitializer.Connection.Query<Car>($"SELECT * FROM {nameof(Car)}");
            Assert.False(cars.Any());
        }
    }
}
