using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Dapper;
using LiveSeeder.Reader;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Z.Dapper.Plus;

namespace LiveSeeder.Core
{
    public static class LiveDbContextExtensions
    {
        private static readonly CsvSeedReader Reader;

        static LiveDbContextExtensions()
        {
            Reader = new CsvSeedReader();
        }

        public static Task SeedAdd<T>(this DbContext dbContext) where T : class
        {
            var records = Reader.Read<T>().ToList();
            return Insert(dbContext, records);
        }

        public static Task SeedAdd<T>(this DbContext dbContext, Assembly assembly, string delimiter = ",",
            string @namespace = "Seed", string fileName = "") where T : class
        {
            var records = Reader.Read<T>(assembly, delimiter, @namespace, fileName).ToList();
            return Insert(dbContext, records);
        }

        public static async Task SeedAddOrUpdate<T>(this DbContext dbContext) where T : class
        {
            var records = Reader.Read<T>().ToList();

            await InsertOrUpdate(dbContext, records);
        }

        public static async Task SeedAddOrUpdate<T>(this DbContext dbContext, Assembly assembly,
            string delimiter = ",", string @namespace = "Seed", string fileName = "") where T : class
        {
            var records = Reader.Read<T>(assembly, delimiter, @namespace, fileName).ToList();

            await InsertOrUpdate(dbContext, records);
        }

        public static Task SeedMerge<T>(this DbContext dbContext) where T : class
        {
            var records = Reader.Read<T>().ToList();

            return MergeAll(dbContext, records);
        }

        public static Task SeedMerge<T>(this DbContext dbContext, Assembly assembly,
            string delimiter = ",", string @namespace = "Seed", string fileName = "") where T : class
        {
            var records = Reader.Read<T>(assembly, delimiter, @namespace, fileName).ToList();

            return MergeAll(dbContext, records);
        }

        public static Task SeedClear<T>(this DbContext dbContext) where T : class
        {
            var tableName = GetTableName<T>(dbContext);
            return dbContext.Database.GetDbConnection().ExecuteAsync($"DELETE FROM {tableName}");
        }

        private static Task Insert<T>(DbContext dbContext, List<T> records) where T : class
        {
            var connection = dbContext.Database.GetDbConnection();

            if (records.Any())
            {
                return connection
                    .BulkActionAsync(x =>
                        x.BulkInsert(records));
            }

            return Task.CompletedTask;
        }

        private static Task Update<T>(DbContext dbContext, List<T> records) where T : class
        {
            var connection = dbContext.Database.GetDbConnection();

            if (records.Any())
            {
                return connection
                    .BulkActionAsync(x =>
                        x.BulkInsert(records));
            }

            return Task.CompletedTask;
        }

        private static Task MergeAll<T>(DbContext dbContext, List<T> records) where T : class
        {
            var connection = dbContext.Database.GetDbConnection();

            if (records.Any())
            {
                return connection
                    .BulkActionAsync(x =>
                        x.BulkMerge(records));
            }

            return Task.CompletedTask;
        }

        private static async Task InsertOrUpdate<T>(DbContext dbContext, List<T> records)
            where T : class
        {
            if (!records.Any())
                return;

            var connection = dbContext.Database.GetDbConnection();
            var data = dbContext.Set<T>().AsNoTracking().ToList();
            var updates = records.Where(x => data.Contains(x)).ToList();
            var inserts = records.Where(x => !data.Contains(x)).ToList();

            if (inserts.Any())
                await connection
                    .BulkActionAsync(x =>
                        x.BulkInsert(inserts));

            if (updates.Any())
                await connection
                    .BulkActionAsync(x =>
                        x.BulkUpdate(updates));
        }

        private static string GetTableName<T>(DbContext dbContext) where T : class
        {
            var model = dbContext.Model;
            var entityTypes = model.GetEntityTypes();
            var entityType = entityTypes.First(t => t.ClrType == typeof(T));
            var tableNameAnnotation = entityType.GetAnnotation("Relational:TableName");
            var tableName = tableNameAnnotation.Value.ToString();
            return tableName;
        }
    }
}
