using Microsoft.EntityFrameworkCore;

namespace SuperUU.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
}
