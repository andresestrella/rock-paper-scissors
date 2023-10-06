namespace rps_server.Data;

using Microsoft.EntityFrameworkCore;
using rps_server.Entities;

public class DataContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Match> Matches { get; set; }

    protected readonly IConfiguration Configuration;

    public DataContext(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        // in memory database used for simplicity, change to a real db for production applications
        options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection"));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasMany(u => u.Matches) // One User has many Matches
            .WithOne(m => m.User)    // Each Match belongs to one User
            .HasForeignKey(m => m.UserId) // Match entity has a foreign key UserId
            .IsRequired(); // UserId is required

        base.OnModelCreating(modelBuilder);
    }

}
