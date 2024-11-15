using Microsoft.EntityFrameworkCore;

namespace aurga.Data
{
    public class DataContext : DbContext
    {
        private readonly IConfiguration _configuration;

        public DataContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Device> Devices { get; set; }
        public DbSet<DeletedUser> DeletedUsers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite(_configuration.GetConnectionString("DefaultConnection"))
            .UseLoggerFactory(LoggerFactory.Create(builder => builder.AddConsole()));

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().Property<long>("Visited");
            modelBuilder.Entity<User>().Property<long>("Created");

            modelBuilder.Entity<DeletedUser>().Property<long>("Created");

            modelBuilder.Entity<Device>().Property<long>("Registered");

            modelBuilder.Entity<User>().ToTable("Users");
            modelBuilder.Entity<Device>().ToTable("Devices");
            modelBuilder.Entity<DeletedUser>().ToTable("DeletedUsers");
        }
    }
}
