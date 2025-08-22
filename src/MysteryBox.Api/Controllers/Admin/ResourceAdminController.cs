using Microsoft.AspNetCore.Mvc;
using MysteryBox.Api.Services;

namespace MysteryBox.Api.Controllers.Admin;

[ApiController]
[Route("api/admin/resources")]
public class ResourceAdminController : ControllerBase
{
    private readonly ResourceLoader _loader;
    public ResourceAdminController(ResourceLoader loader) => _loader = loader;

    [HttpPost("rebuild")]
    public IActionResult Rebuild()
    {
        // Placeholder: would load CSV files into memory/DB.
        // Example: _loader.LoadCsv("tb_country.csv", p => new { Code = p[0], Name = p[1] });
        return Ok(new { message = "Resource rebuild triggered. Place CSV files under ResourceDB/CSV." });
    }
}
