using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyNUnit;
using WebAPI.Data;
using WebAPI.Models;

namespace WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TestRunsController : ControllerBase
{
    private readonly NUnitWebDbContext _context;

    private readonly int MaxUploadSize = 25 * 1024 * 1024;

    public TestRunsController(NUnitWebDbContext context)
    {
        _context = context;
    }

    // GET: api/TestRuns
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TestRun>>> GetTestRuns()
    {
        return await _context.TestRuns.Include(t => t.Result).ThenInclude(r => r.TestAssemblyResults)
           .ThenInclude(a => a.TestClassResults)
           .ThenInclude(c => c.TestResults).ToListAsync();
    }

    // GET: api/TestRuns/5
    [HttpGet("{id}")]
    public async Task<ActionResult<TestRun>> GetTestRun(int id)
    {
        var testRun = await _context.TestRuns
           .Include(t => t.Result)
           .ThenInclude(r => r.TestAssemblyResults)
           .ThenInclude(a => a.TestClassResults)
           .ThenInclude(c => c.TestResults)
           .FirstOrDefaultAsync(t => t.Id == id);

        if (testRun == null)
        {
            return NotFound();
        }

        return testRun;
    }

    // POST: api/TestRuns
    [HttpPost("upload")]
    public async Task<ActionResult<TestRun>> UploadAndRunTests([FromForm] List<IFormFile> files)
    {
        var validationResult = ValidateFiles(files);
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

            _context.TestRuns.Add(testRun);
            await _context.SaveChangesAsync();

            testsResult = null;

            return CreatedAtAction(nameof(GetTestRun), new { id = testRun.Id }, testRun);
        }
        finally
        {
            //Directory.Delete(tempDir, recursive: true);
        }
    }

    private ActionResult ValidateFiles(List<IFormFile> files)
    {
        if (files == null || files.Count == 0)
        {
            return BadRequest("No files uploaded.");
        }

        if (files.Sum(file => file.Length) > MaxUploadSize)
        {
            return BadRequest($"Files exceeds the maximum allowed size of 25 MB.");
        }

        foreach (var file in files)
        {
            if (!Path.GetExtension(file.FileName).Equals(".dll", StringComparison.CurrentCultureIgnoreCase))
            {
                return BadRequest($"File {file.FileName} is not a .dll file.");
            }
        }

        return null;
    }
}
