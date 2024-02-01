using AccessManager.Models.Database;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Providers
{
    public class DataContext : DbContext
    {
        private readonly string _connectionString;

        public DbSet<UserDbModel> User { get; set; }

        public DataContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_connectionString);
        }
    }
}
