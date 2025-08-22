using Microsoft.AspNetCore.Mvc;
using MysteryBox.Api.DTOs;

namespace MysteryBox.Api.Controllers.Admin;

[ApiController]
[Route("api/init")]
public class InitController : ControllerBase
{
    [HttpGet("refresh")]
    public ActionResult<ResRefresh> Refresh()
        => Ok(new ResRefresh(DateTime.UtcNow.ToString("o"), "1.0.0", "1.0.0"));
}
