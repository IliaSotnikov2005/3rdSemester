// <copyright file="NUnitWebDbContext.cs" company="IlyaSotnikov">
// Copyright (c) IlyaSotnikov. All rights reserved.
// </copyright>

namespace WebAPI.Data;

using Microsoft.EntityFrameworkCore;
using WebAPI.Models;

/// <summary>
/// Db context for test runs in MyNUnitWeb.
/// </summary>
public class NUnitWebDbContext : DbContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NUnitWebDbContext"/> class.
    /// </summary>
    /// <param name="options">DbContextOptions.</param>
    public NUnitWebDbContext(DbContextOptions<NUnitWebDbContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// Gets DbSet of test runs.
    /// </summary>
    public DbSet<TestRun> TestRuns => this.Set<TestRun>();
}
