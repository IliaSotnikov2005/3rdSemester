using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MyNUnitWeb.Pages;

[ValidateAntiForgeryToken]
public class UploadAssemblyModel : PageModel
{
    public static int MaxUploadSize = 10 * 1024 * 1024;

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var files = Request.Form.Files;

        if (files == null || files.Count == 0)
        {
            ViewData["ErrorMessage"] = "No files selected.";
            return Page();
        }

        foreach (var file in files)
        {
            if (file.Length > 0)
            {
                var extension = Path.GetExtension(file.FileName).ToLower();
                if (extension != ".dll")
                {
                    ViewData["ErrorMessage"] = "Only .dll files are allowed.";
                    return Page();
                }

                if (file.Length > MaxUploadSize)
                {
                    ViewData["ErrorMessage"] = $"File size must be less than {MaxUploadSize / 1024 / 1024} MB.";
                    return Page();
                }

                var filePath = Path.Combine("Uploads", file.FileName);
                Directory.CreateDirectory("Uploads");

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
            }
        }

        return Page();
    }
}