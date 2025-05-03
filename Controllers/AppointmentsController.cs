using ApexLawFirm.API.Data;
using ApexLawFirm.API.DTOs;
using ApexLawFirm.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ApexLawFirm.API.Controllers {
  [ApiController]
  [Route("api/appointments")]
  public class AppointmentsController : ControllerBase{
    private readonly ApexLawFirmDbContext _context;

    public AppointmentsController(ApexLawFirmDbContext context){
      _context = context;
    }

    // GET: /api/appointments
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAll(){
      var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
      var userRole = User.FindFirstValue(ClaimTypes.Role);

      var appointmentsQuery = _context.Appointments
        .Include(a => a.User)
        .Include(a => a.LawyerProfile).ThenInclude(lp => lp.User)
        .AsQueryable();

      if(userRole == "User"){
        appointmentsQuery = appointmentsQuery.Where(a => a.UserId == userId);
      } else if(userRole == "Lawyer"){
        var lawyerProfile = await _context.LawyerProfiles.FirstOrDefaultAsync(lp => lp.UserId == userId);
        if (lawyerProfile == null)
          return Forbid("Access denied. No associated lawyer profile found.");
        appointmentsQuery = appointmentsQuery.Where(a => a.LawyerId == lawyerProfile.Id);
      }

      var appointments = await appointmentsQuery.Select(a => new AppointmentResponseDto{
        Id = a.Id,
        AppointmentDate = a.AppointmentDate,
        Status = a.Status,
        LawyerName = a.LawyerProfile.User.FullName,
        UserName = a.User.FullName
      }).ToListAsync();

      return Ok(new { message = "Appointments retrieved successfully.", data = appointments });
    }

    // GET: /api/appointments/{id}
    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> GetById(int id){
      var appointment = await _context.Appointments
        .Include(a => a.User)
        .Include(a => a.LawyerProfile)
            .ThenInclude(lp => lp.User)
        .FirstOrDefaultAsync(a => a.Id == id);

      if(appointment == null)
        return NotFound(new { message = "Appointment not found." });

      var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
      var role = User.FindFirstValue(ClaimTypes.Role);

      if(role == "User" && appointment.UserId != userId) 
        return Forbid("Access denied: This appointment does not belong to you.");
      if(role == "Lawyer" && appointment.LawyerProfile.UserId != userId) 
        return Forbid("Access denied: This appointment does not belong to you.");

      var dto = new AppointmentResponseDto{
        Id = appointment.Id,
        AppointmentDate = appointment.AppointmentDate,
        Status = appointment.Status,
        CaseDescription = appointment.CaseDescription,
        LawyerName = appointment.LawyerProfile.User.FullName,
        UserName = appointment.User.FullName
      };

      return Ok(new { message = "Appointment details retrieved successfully.", data = dto });
    }


    // POST: /api/appointments
    [HttpPost]
    [Authorize(Roles = "User,Admin")]
    public async Task<IActionResult> Create(AppointmentCreateDto dto){
      var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

      var lawyerProfile = await _context.LawyerProfiles.FindAsync(dto.LawyerId);
      if(lawyerProfile == null) 
        return NotFound(new { message = "Lawyer not found." });

      var appointment = new Appointment{
        UserId = userId,
        LawyerId = dto.LawyerId,
        CaseDescription = dto.CaseDescription,
        AppointmentDate = dto.AppointmentDate,
        Status = "Pending"
      };

      _context.Appointments.Add(appointment);
      await _context.SaveChangesAsync();

      return Ok( new {
        message = "Appointment successfully created.", 
        data = new{ 
          appointment.Id, 
          appointment.Status, 
          appointment.AppointmentDate 
        }
      });
    }

    // PATCH: /api/appointments/{id}/status
    [HttpPatch("{id}/status")]
    [Authorize(Roles = "Lawyer,Admin")]
    public async Task<IActionResult> UpdateStatus(int id, AppointmentStatusUpdateDto dto){
      var appointment = await _context.Appointments
        .Include(a => a.LawyerProfile)
        .FirstOrDefaultAsync(a => a.Id == id);

      if(appointment == null) 
        return NotFound(new { message = "Appointment not found." });

      var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
      var userRole = User.FindFirstValue(ClaimTypes.Role);

      if(userRole == "Lawyer" && appointment.LawyerProfile.UserId != userId)
        return Forbid("You do not have permission to update this appointment.");

      var validStatuses = new[] { "Pending", "Confirmed", "Cancelled", "Rejected", "Completed" };
      
      if(!validStatuses.Contains(dto.Status))
        return BadRequest(new { message = "Invalid status. Valid statuses are: Pending, Confirmed, Cancelled, Completed, Rejected." });
      
      appointment.Status = dto.Status;
      await _context.SaveChangesAsync();

      return Ok(new {
        message = "Appointment status updated.", 
        data = new{ 
          appointment.Id, 
          appointment.Status
        }
      });
    }

    // DELETE: /api/appointments/{id}
    [HttpDelete("{id}")]
    [Authorize(Roles = "User,Lawyer,Admin")]
    public async Task<IActionResult> Delete(int id){
      var appointment = await _context.Appointments
        .Include(a => a.LawyerProfile)
        .FirstOrDefaultAsync(a => a.Id == id);

      if(appointment == null) 
        return NotFound(new { message = "Appointment not found." });

      var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
      var role = User.FindFirstValue(ClaimTypes.Role);

      if(role == "User" && appointment.UserId != userId) 
        return Forbid("You do not have permission to delete this appointment.");
      if(role == "Lawyer" && appointment.LawyerProfile.UserId != userId)
        return Forbid("You do not have permission to delete this appointment.");

      _context.Appointments.Remove(appointment);
      await _context.SaveChangesAsync();

      return Ok(new { message = "Appointment deleted successfully." });
    }
  }
}
