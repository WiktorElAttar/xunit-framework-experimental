using Microsoft.EntityFrameworkCore;
using SampleApp.Domain;

namespace SampleApp.Infrastructure;

public class AppDbContext(DbContextOptions<AppDbContext> options)
    : DbContext(options)
{
    public DbSet<User> Users { get; set; }
}
