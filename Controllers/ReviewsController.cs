using ApexLawFirm.API.Data;
using ApexLawFirm.API.DTOs;
using ApexLawFirm.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ApexLawFirm.API.Controllers{
  [ApiController]
  [Route("api/reviews")]
  public class ReviewsController : ControllerBase{
    private readonly ApexLawFirmDbContext _context;

    public ReviewsController(ApexLawFirmDbContext context){
      _context = context;
    }

    // POST: /api/reviews
    [HttpPost]
    [Authorize(Roles = "User")]
    public async Task<IActionResult> Create(ReviewCreateDto dto){
      var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

      var lawyerExists = await _context.LawyerProfiles.AnyAsync(lp => lp.Id == dto.LawyerId);
      if(!lawyerExists) 
        return NotFound(new { message = "Lawyer not found." });

      var review = new Review{
        UserId = userId,
        LawyerId = dto.LawyerId,
        Rating = dto.Rating,
        Comment = dto.Comment
      };

      _context.Reviews.Add(review);
      await _context.SaveChangesAsync();

      return Ok(new {
        message = "Review created successfully.",
        data = new {
          review.Id,
          review.Rating,
          review.Comment,
          review.CreatedAt
        }
      });
    }

    // GET: /api/reviews/lawyer/{lawyerId}
    [HttpGet("lawyer/{lawyerId}")]
    public async Task<IActionResult> GetByLawyer(int lawyerId){
      var lawyerExists = await _context.LawyerProfiles.AnyAsync(lp => lp.Id == lawyerId);
      if (!lawyerExists)
        return NotFound(new { message = "Lawyer not found." });
      
      var reviews = await _context.Reviews
        .Include(r => r.User)
        .Where(r => r.LawyerId == lawyerId)
        .OrderByDescending(r => r.CreatedAt)
        .Select(r => new ReviewResponseDto{
          Id = r.Id,
          Rating = r.Rating,
          Comment = r.Comment,
          CreatedAt = r.CreatedAt,
          UserName = $"{r.User.FullName} " + $"{r.User.LastName}"
        }).ToListAsync();

      return Ok(new {
        message = "Reviews retrieved successfully.",
        data = reviews
      });
    }

    // PATCH: /api/reviews/{id}
    [HttpPatch("{id}")]
    [Authorize(Roles = "User")]
    public async Task<IActionResult> Update(int id, ReviewCreateDto dto){
      var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
      var review = await _context.Reviews.FirstOrDefaultAsync(r => r.Id == id);

      if(review == null)
        return NotFound(new { message = "Review not found." });
      if(review.UserId != userId) 
        return Forbid("You are not authorized to edit this review.");

      review.Rating = dto.Rating;
      review.Comment = dto.Comment;

      await _context.SaveChangesAsync();
      return Ok(new {
        message = "Review updated successfully.",
        data = new {
          review.Id,
          review.Rating,
          review.Comment
        }
      });
    }

    // DELETE: /api/reviews/{id}
    [HttpDelete("{id}")]
    [Authorize(Roles = "User")]
    public async Task<IActionResult> Delete(int id){
      var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
      var review = await _context.Reviews.FirstOrDefaultAsync(r => r.Id == id);

      if(review == null)
         return NotFound(new { message = "Review not found." });
      if(review.UserId != userId) 
        return Forbid("You are not authorized to delete this review.");

      _context.Reviews.Remove(review);
      await _context.SaveChangesAsync();
      return Ok(new { message = "Review deleted successfully." });
    }
  }
}
