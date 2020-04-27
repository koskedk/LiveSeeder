using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Dapper;
using LiveSeeder.Reader;
using Z.Dapper.Plus;

namespace LiveSeeder.Core
{
    public static class LiveIDbConnectionExtensions
    {
        private static readonly CsvSeedReader Reader;

        static LiveIDbConnectionExtensions()
        {
            Reader = new CsvSeedReader();
        }

        public static Task SeedAdd<T>(this IDbConnection connection) where T : class
        {
            var records = Reader.Read<T>().ToList();
            return Insert(connection,records);
        }

        public static Task SeedAdd<T>(this IDbConnection connection, Assembly assembly, string delimiter = ",", string @namespace = "Seed", string fileName = "") where T : class
        {
            var records = Reader.Read<T>(assembly, delimiter,@namespace, fileName).ToList();
            return Insert(connection,records);
        }

        public static async Task SeedAddOrUpdate<T>(this IDbConnection connection, string table = "") where T : class
        {
            var tableName = string.IsNullOrWhiteSpace(table) ? typeof(T).Name : table;
            var records = Reader.Read<T>().ToList();

            await InsertOrUpdate(connection,records, tableName);
        }

        public static async Task SeedAddOrUpdate<T>(this IDbConnection connection, Assembly assembly,
            string delimiter = ",", string @namespace = "Seed", string fileName = "", string table = "") where T : class
        {
            var tableName = string.IsNullOrWhiteSpace(table) ? typeof(T).Name : table;
            var records = Reader.Read<T>(assembly, delimiter, @namespace, fileName).ToList();

            await InsertOrUpdate(connection, records, tableName);
        }

        public static async Task SeedNewOnly<T>(this IDbConnection connection, string table = "") where T : class
        {
            var tableName = string.IsNullOrWhiteSpace(table) ? typeof(T).Name : table;
            var records = Reader.Read<T>().ToList();

            await InsertNewOnly(connection,records, tableName);
        }

        public static async Task SeedNewOnly<T>(this IDbConnection connection, Assembly assembly,
            string delimiter = ",", string @namespace = "Seed", string fileName = "", string table = "") where T : class
        {
            var tableName = string.IsNullOrWhiteSpace(table) ? typeof(T).Name : table;
            var records = Reader.Read<T>(assembly, delimiter, @namespace, fileName).ToList();

            await InsertNewOnly(connection, records, tableName);
        }

        public static Task SeedMerge<T>(this IDbConnection connection) where T : class
        {
            var records = Reader.Read<T>().ToList();

            return MergeAll(connection, records);
        }

        public static Task SeedMerge<T>(this IDbConnection connection, Assembly assembly,
            string delimiter = ",", string @namespace = "Seed", string fileName = "") where T : class
        {
            var records = Reader.Read<T>(assembly, delimiter, @namespace, fileName).ToList();

            return MergeAll(connection, records);
        }

        public static Task SeedClear<T>(this IDbConnection connection,string table = "") where T : class
        {
            var tableName = string.IsNullOrWhiteSpace(table) ? typeof(T).Name : table;
            return connection.ExecuteAsync($"DELETE FROM {tableName}");
        }

        private static Task Insert<T>(IDbConnection connection, List<T> records) where T : class
        {
            if (records.Any())
            {
                return connection
                    .BulkActionAsync(x =>
                        x.BulkInsert(records));
            }

            return Task.CompletedTask;
        }

        private static Task Update<T>(IDbConnection connection, List<T> records) where T : class
        {
            if (records.Any())
            {
                return connection
                    .BulkActionAsync(x =>
                        x.BulkInsert(records));
            }

            return Task.CompletedTask;
        }

        private static Task MergeAll<T>(IDbConnection connection, List<T> records) where T : class
        {
            if (records.Any())
            {
                return connection
                    .BulkActionAsync(x =>
                        x.BulkMerge(records));
            }

            return Task.CompletedTask;
        }

        private static async Task InsertOrUpdate<T>(IDbConnection connection, List<T> records,string tableName) where T : class
        {
            if (!records.Any())
                return;

            var data = connection.Query<T>($"SELECT * FROM {tableName}").ToList();
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

        private static async Task InsertNewOnly<T>(IDbConnection connection, List<T> records, string tableName)
            where T : class
        {
            if (!records.Any())
                return;

            var data = connection.Query<T>($"SELECT * FROM {tableName}").ToList();
            var newRecords = records.Where(x => !data.Contains(x)).ToList();

            if (newRecords.Any())
                await connection
                    .BulkActionAsync(x =>
                        x.BulkInsert(newRecords));

        }
    }
}
