using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using Dapper;
using LiveSeeder.Reader;
using LiveSeeder.Tests.TestArtifacts;
using LiveSeeder.Utils;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Z.Dapper.Plus;

namespace LiveSeeder.Tests
{
    [SetUpFixture]
    public class TestInitializer
    {
        public static IServiceProvider ServiceProvider;
        public static IDbConnection Connection;
        public static IDbConnection ConnectionB;
        public static IConfigurationRoot Configuration;

        [OneTimeSetUp]
        public void Init()
        {
            SqlMapper.AddTypeHandler<Guid>(new GuidTypeHandler());
            RemoveTestsFilesDbs();

            var dir = $"{TestContext.CurrentContext.TestDirectory.HasToEndWith(@"/")}";

            var config = Configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile("appsettings.License.json", optional: true, reloadOnChange: true)
                .Build();

            RegisterLicence();

            var connectionString = config.GetConnectionString("liveConnection")
                .Replace("#dir#", dir)
                .ToOsStyle();
            Connection = new SqliteConnection(connectionString);

            var connectionStringB = config.GetConnectionString("liveConnectionB")
                .Replace("#dir#", dir)
                .ToOsStyle();
            var connection = new SqliteConnection(connectionStringB);
            ConnectionB = connection;
            connection.Open();

            var services = new ServiceCollection();

            services.AddDbContext<TestDbContext>(x => x.UseSqlite(connection));
            services.AddTransient<ISeedReader, CsvSeedReader>();
            ServiceProvider = services.BuildServiceProvider();
        }

        public static void InitIDbConnection()
        {
            DapperPlusManager.Entity<Car>()
                .Key(x => x.Id)
                .Table(nameof(Car));

            DapperPlusManager.Entity<TestCar>()
                .Key(x=>x.Id)
                .Table(nameof(TestCar));

            DapperPlusManager.Entity<Company>()
                .Key(x=>x.Id)
                .Table(nameof(Company));

            DapperPlusManager.Entity<County>()
                .Key(x=>x.Id)
                .Table(nameof(County));
        }

        public static void InitDbContext()
        {
            DapperPlusManager.Entity<Car>()
                .Key(x => x.Id)
                .Table(nameof(TestDbContext.Cars));

            DapperPlusManager.Entity<TestCar>()
                .Key(x=>x.Id)
                .Table($"{nameof(TestDbContext.TestCars)}");

            DapperPlusManager.Entity<Company>()
                .Key(x=>x.Id)
                .Table(nameof(TestDbContext.Companies));

            DapperPlusManager.Entity<County>()
                .Key(x=>x.Id)
                .Table(nameof(TestDbContext.Counties));

        }

        private void RegisterLicence()
        {
            if (!string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("DAPPER_LICENSENAME")))
            {
                DapperPlusManager.AddLicense(Environment.GetEnvironmentVariable("DAPPER_LICENSENAME"),
                    Environment.GetEnvironmentVariable("DAPPER_LICENSEKEY"));
                if (!Z.Dapper.Plus.DapperPlusManager.ValidateLicense(out var licenseErrorMessage))
                    throw new Exception(licenseErrorMessage);
                return;
            }

            if (!string.IsNullOrWhiteSpace(Configuration["DapperPlusManager:licenseName"]))
            {
                DapperPlusManager.AddLicense(Configuration["DapperPlusManager:licenseName"],
                    Configuration["DapperPlusManager:licenseKey"]);
                if (!Z.Dapper.Plus.DapperPlusManager.ValidateLicense(out var licenseErrorMessage))
                    throw new Exception(licenseErrorMessage);
            }
        }

        private void RemoveTestsFilesDbs()
        {
            string[] keyFiles =
                { "seed.db","seedB.db"};
            string[] keyDirs = { @"TestArtifacts/Database".ToOsStyle()};

            foreach (var keyDir in keyDirs)
            {
                DirectoryInfo di = new DirectoryInfo(keyDir);
                foreach (FileInfo file in di.GetFiles())
                {
                    if (!keyFiles.Contains(file.Name))
                        file.Delete();
                }
            }
        }
    }
}
