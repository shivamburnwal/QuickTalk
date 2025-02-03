using Microsoft.EntityFrameworkCore;
using QuickTalk.Api.Data;

namespace QuickTalk.Api.Services
{
    public class DatabaseService
    {
        private readonly ChatDbContext _context;

        public DatabaseService(ChatDbContext context)
        {
            _context = context;
        }

        public void DeleteDataFromDatabase()
        {
            // Disable foreign key checks (SQL Server)
            _context.Database.ExecuteSqlRaw("EXEC sp_MSforeachtable 'ALTER TABLE ? NOCHECK CONSTRAINT all'");

            // Delete data from tables
            var tableNames = _context.Model.GetEntityTypes()
                .Select(t => t.GetTableName())
                .ToList();

            foreach (var tableName in tableNames)
            {
                _context.Database.ExecuteSqlRaw($"DELETE FROM {tableName}");
            }

            // Re-enable foreign key checks
            _context.Database.ExecuteSqlRaw("EXEC sp_MSforeachtable 'ALTER TABLE ? CHECK CONSTRAINT all'");
        }
    }
}
