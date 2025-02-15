using Microsoft.EntityFrameworkCore;
using WebAPI.Models;

namespace WebAPI.Data;

public class NUnitWebDbContext : DbContext
{
    public NUnitWebDbContext(DbContextOptions<NUnitWebDbContext> options) : base(options)
    {
    }

    public DbSet<TestRun> TestRuns => Set<TestRun>();
}
