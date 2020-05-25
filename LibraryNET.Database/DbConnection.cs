using Microsoft.EntityFrameworkCore;
using LibraryNET.Model;

namespace LibraryNET.Database
{
    /// <summary>
    /// My database connection used in my API
    /// </summary>
    /// <seealso cref="Microsoft.EntityFrameworkCore.DbContext" />
    public class DbConnection : DbContext
    {
        public DbSet<User> fpUser { get; set; }
        public DbSet<Book> fpBook { get; set; }
        public DbSet<Collection> fpCollection { get; set; }
        public DbConnection(DbContextOptions<DbConnection> options) : base(options) { }
    }
}
