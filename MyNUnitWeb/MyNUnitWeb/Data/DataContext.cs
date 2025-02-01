namespace MyNUnitWeb.Data;

using Microsoft.EntityFrameworkCore;
using MyNUnitWeb.Models;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {
    }

    public DbSet<TestClassResults> TestsResults => Set<TestClassResults>();
}

