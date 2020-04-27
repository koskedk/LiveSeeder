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

        public static Task Add<T>(this IDbConnection connection) where T : class
        {
            var records = Reader.Read<T>().ToList();
            return Insert(connection,records);
        }

        public static Task Add<T>(this IDbConnection connection, Assembly assembly, string delimiter = ",", string @namespace = "Seed", string fileName = "") where T : class
        {
            var records = Reader.Read<T>(assembly, delimiter,@namespace, fileName).ToList();
            return Insert(connection,records);
        }

        public static async Task AddOrUpdate<T>(this IDbConnection connection, string table = "") where T : class
        {
            var tableName = string.IsNullOrWhiteSpace(table) ? typeof(T).Name : table;
            var records = Reader.Read<T>().ToList();

            await InsertOrUpdate(connection,records, tableName);
        }

        public static async Task AddOrUpdate<T>(this IDbConnection connection, Assembly assembly,
            string delimiter = ",", string @namespace = "Seed", string fileName = "", string table = "") where T : class
        {
            var tableName = string.IsNullOrWhiteSpace(table) ? typeof(T).Name : table;
            var records = Reader.Read<T>(assembly, delimiter, @namespace, fileName).ToList();

            await InsertOrUpdate(connection, records, tableName);
        }

        public static Task Merge<T>(this IDbConnection connection) where T : class
        {
            var records = Reader.Read<T>().ToList();

            return MergeAll(connection, records);
        }

        public static Task Merge<T>(this IDbConnection connection, Assembly assembly,
            string delimiter = ",", string @namespace = "Seed", string fileName = "") where T : class
        {
            var records = Reader.Read<T>(assembly, delimiter, @namespace, fileName).ToList();

            return MergeAll(connection, records);
        }

        public static Task Clear<T>(this IDbConnection connection,string table = "") where T : class
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


            var updates = new List<T>();
            var inserts = new List<T>();

            var data = connection.Query<T>($"SELECT * FROM {tableName}").ToList();

            updates = records.Where(x => data.Contains(x)).ToList();
            inserts = records.Where(x => !data.Contains(x)).ToList();

            if (inserts.Any())
                await connection
                    .BulkActionAsync(x =>
                        x.BulkInsert(inserts));

            if (updates.Any())
                await connection
                    .BulkActionAsync(x =>
                        x.BulkUpdate(updates));
        }
    }
}
