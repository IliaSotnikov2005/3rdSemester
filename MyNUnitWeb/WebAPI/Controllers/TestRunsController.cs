// <copyright file="TestRunsController.cs" company="IlyaSotnikov">
// Copyright (c) IlyaSotnikov. All rights reserved.
// </copyright>

namespace WebAPI.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyNUnit;
using WebAPI.Data;
using WebAPI.Models;

/// <summary>
/// A controller to work with test runs.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="TestRunsController"/> class.
/// </remarks>
/// <param name="context">Db context.</param>
[Route("api/[controller]")]
[ApiController]
public class TestRunsController(NUnitWebDbContext context) : ControllerBase
{
    private readonly int maxUploadSize = 25 * 1024 * 1024;

    /// <summary>
    /// Gets test runs.
    /// </summary>
    /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
    /// GET: api/TestRuns
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TestRun>>> GetTestRuns()
    {
        return await context.TestRuns.Include(t => t.Result).ThenInclude(r => r.TestAssemblyResults)
           .ThenInclude(a => a.TestClassResults)
           .ThenInclude(c => c.TestResults).ToListAsync();
    }

    /// <summary>
    /// Gets test run result with given id.
    /// </summary>
    /// <param name="id">Id.</param>
    /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
    /// GET: api/TestRuns/5
    [HttpGet("{id}")]
    public async Task<ActionResult<TestRun>> GetTestRun(int id)
    {
        var testRun = await context.TestRuns
           .Include(t => t.Result)
           .ThenInclude(r => r.TestAssemblyResults)
           .ThenInclude(a => a.TestClassResults)
           .ThenInclude(c => c.TestResults)
           .FirstOrDefaultAsync(t => t.Id == id);

        if (testRun == null)
        {
            return this.NotFound();
        }

        return testRun;
    }

    /// <summary>
    /// Uploads given files, runs and saves test results in the database.
    /// </summary>
    /// <param name="files">Assembly files.</param>
    /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
    /// POST: api/TestRuns
    [HttpPost("upload")]
    public async Task<ActionResult<TestRun>> UploadAndRunTests([FromForm] List<IFormFile> files)
    {
        var validationResult = this.ValidateFiles(files);
        if (validationResult is not null)
        {
            return validationResult;
        }

        var tempDir = Path.Combine("./Uploaded", Path.GetRandomFileName());
        Directory.CreateDirectory(tempDir);

        try
        {
            foreach (var file in files)
            {
                var filePath = Path.Combine(tempDir, file.FileName);
                using var stream = new FileStream(filePath, FileMode.Create);
                await file.CopyToAsync(stream);
            }

            TestRunResult testsResult = await MyTester.RunTestsFromDirectory(Path.GetFullPath(tempDir));

            var testRun = new TestRun
            {
                LaunchTime = DateTime.UtcNow,
                Result = testsResult,
            };

            context.TestRuns.Add(testRun);
            await context.SaveChangesAsync();

            return this.CreatedAtAction(nameof(this.GetTestRun), new { id = testRun.Id }, testRun);
        }
        finally
        {
            // Directory.Delete(tempDir, recursive: true);
        }
    }

    private BadRequestObjectResult? ValidateFiles(List<IFormFile> files)
    {
        if (files == null || files.Count == 0)
        {
            return this.BadRequest("No files uploaded.");
        }

        if (files.Sum(file => file.Length) > this.maxUploadSize)
        {
            return this.BadRequest($"Files exceeds the maximum allowed size of 25 MB.");
        }

        foreach (var file in files)
        {
            if (!Path.GetExtension(file.FileName).Equals(".dll", StringComparison.CurrentCultureIgnoreCase))
            {
                return this.BadRequest($"File {file.FileName} is not a .dll file.");
            }
        }

        return null;
    }
}
