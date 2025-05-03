using ApexLawFirm.API.Data;
using ApexLawFirm.API.DTOs;
using ApexLawFirm.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ApexLawFirm.API.Controllers {
  [ApiController]
  [Route("api/specializations")]
  public class SpecializationsController : ControllerBase {
    private readonly ApexLawFirmDbContext _context;

    public SpecializationsController(ApexLawFirmDbContext context){
      _context = context;
    }

    // GET: /api/specializations
    [HttpGet]
    public async Task<IActionResult> GetAll(){
      var specializations = await _context.Specializations
        .Select(s => new SpecializationDto{
          Id = s.Id,
          Name = s.Name
        }).ToListAsync();

      return Ok(new {
      message = "Specializations retrieved successfully.",
      data = specializations
    });
    }
    
    // POST: /api/specializations (Admin only)
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] SpecializationDto dto) {
      var specialization = new Specialization { Name = dto.Name };
      _context.Specializations.Add(specialization);
      await _context.SaveChangesAsync();

      return Ok(new {
      message = "Specialization created successfully.",
      data = new {
        specialization.Id,
        specialization.Name
      }
    });
    }

    // POST: /api/specializations/assign (Lawyer assigns specialization to self)
    [HttpPost("assign")]
    [Authorize(Roles = "Lawyer")]
    public async Task<IActionResult> Assign([FromBody] AssignSpecializationDto dto) {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var lawyer = await _context.LawyerProfiles.FirstOrDefaultAsync(lp => lp.UserId == userId);
        if(lawyer == null)
          return Forbid("You must have a lawyer profile to assign specializations.");

        bool alreadyAssigned = await _context.LawyerSpecializations
          .AnyAsync(ls => ls.LawyerId == lawyer.Id && ls.SpecializationId == dto.SpecializationId);
        if(alreadyAssigned)
          return BadRequest(new { message = "This specialization is already assigned to you." });

        var assignment = new LawyerSpecialization{
          LawyerId = lawyer.Id,
          SpecializationId = dto.SpecializationId
        };

        _context.LawyerSpecializations.Add(assignment);
        await _context.SaveChangesAsync();
        return Ok(new { message = "Specialization assigned successfully." });
    }

    // DELETE: /api/specializations/unassign/{id}
    [HttpDelete("unassign/{id}")]
    [Authorize(Roles = "Lawyer")]
    public async Task<IActionResult> Unassign(int id) {
      var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
      var lawyer = await _context.LawyerProfiles.FirstOrDefaultAsync(lp => lp.UserId == userId);
      if(lawyer == null)
        return Forbid("You must have a lawyer profile to unassign specializations.");

      var specialization = await _context.LawyerSpecializations
        .FirstOrDefaultAsync(ls => ls.LawyerId == lawyer.Id && ls.SpecializationId == id);

      if(specialization == null)
        return NotFound(new { message = "Specialization not found or not assigned." });

      _context.LawyerSpecializations.Remove(specialization);
      await _context.SaveChangesAsync();

      return Ok(new { message = "Specialization unassigned successfully." });
    }
  }
}
