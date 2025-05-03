using ApexLawFirm.API.Data;
using ApexLawFirm.API.DTOs;
using ApexLawFirm.API.Helpers;
using ApexLawFirm.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ApexLawFirm.API.Controllers{
  [ApiController]
  [Route("api/lawyers")]
  public class LawyerProfilesController : ControllerBase{
    private readonly ApexLawFirmDbContext _context;

    public LawyerProfilesController(ApexLawFirmDbContext context){
      _context = context;
    }

    // GET: /api/lawyers?specializationId=1 (optional filter)
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int? specializationId){
      var query = _context.LawyerProfiles
        .Include(lp => lp.User)
          .ThenInclude(u => u.Role)
        .Include(lp => lp.LawyerSpecializations)
          .ThenInclude(ls => ls.Specialization)
        .AsQueryable();

      if(specializationId.HasValue){
        query = query.Where(lp => lp.LawyerSpecializations
          .Any(ls => ls.SpecializationId == specializationId.Value));
      }

      var lawyers = await query.ToListAsync();
      var dtoList = lawyers.Select(lp => lp.ToLawyerProfileDto());

      return Ok(new { message = "Lawyers retrieved successfully.", data = dtoList });
    }

    // GET: /api/lawyers/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id){
      var lawyer = await _context.LawyerProfiles
        .Include(lp => lp.User)
          .ThenInclude(u => u.Role)
        .Include(lp => lp.LawyerSpecializations)
          .ThenInclude(ls => ls.Specialization)
        .FirstOrDefaultAsync(lp => lp.Id == id);

      if(lawyer == null)
        return NotFound(new { message = "Lawyer not found." });

      return Ok(new { message = "Lawyer retrieved successfully.", data = lawyer.ToLawyerProfileDto() });
    }

    // POST: /api/lawyers
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] LawyerProfile profile){
      var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
      profile.UserId = userId;

      var existingUser = await _context.Users
        .FirstOrDefaultAsync(u => u.Id == userId);

      if(existingUser == null)
        return NotFound(new { message = "User not found." });

      _context.LawyerProfiles.Add(profile);
      await _context.SaveChangesAsync();
      
      var created = await _context.LawyerProfiles
        .Include(lp => lp.User)
          .ThenInclude(u => u.Role)
        .Include(lp => lp.LawyerSpecializations)
          .ThenInclude(ls => ls.Specialization)
        .FirstOrDefaultAsync(lp => lp.Id == profile.Id);

      return Ok(new { message = "Lawyer profile created successfully.", data = created!.ToLawyerProfileDto() });
    }

    // PUT: /api/lawyers/{id}
    [HttpPut("{id}")]
    [Authorize(Roles = "Lawyer")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateLawyerProfileDto updated){
      var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
      var profile = await _context.LawyerProfiles.FirstOrDefaultAsync(lp => lp.Id == id && lp.UserId == userId);

      if(profile == null)
        return Forbid("You do not have permission to update this profile.");

      profile.Biography = updated.Biography;
      profile.YearsExperience = updated.YearsExperience;
      profile.RegistrationBody = updated.RegistrationBody;
      profile.RegistrationNumber = updated.RegistrationNumber;

      await _context.SaveChangesAsync();
      return Ok(new { message = "Profile updated successfully.", data = profile.ToUpdateLawyerProfileDto() });
    }

    // DELETE: /api/lawyers/{id}
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id){
      var profile = await _context.LawyerProfiles.FindAsync(id);
      if(profile == null)
        return NotFound(new { message = "Lawyer profile not found." });

      _context.LawyerProfiles.Remove(profile);
      await _context.SaveChangesAsync();

      return Ok(new { message = "Lawyer profile deleted successfully." });
    }
  }
}
