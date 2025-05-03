using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApexLawFirm.API.Controllers{
  [ApiController]
  [Route("api/admin")]
  [Authorize(Roles = "Admin")]
  public class AdminController : ControllerBase{

    [HttpGet("dashboard")]
    public IActionResult GetDashboard(){
      return Ok(new {
        Message = "Welcome to the admin dashboard!",
        Timestamp = DateTime.UtcNow
      });
    }

    [HttpGet("users")]
    public IActionResult GetUsers(){
      return Ok(new[] {
        new { Id = 1, Name = "Admin User" },
        new { Id = 2, Name = "Regular User" }
      });
    }
  }
}
