using Microsoft.EntityFrameworkCore;

namespace OpeniddictServer.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder) { }
}
