using ApexLawFirm.API.Data;
using ApexLawFirm.API.DTOs;
using ApexLawFirm.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ApexLawFirm.API.Controllers{
  [ApiController]
  [Route("api/auth")]
  public class AuthController : ControllerBase{
    private readonly ApexLawFirmDbContext _context;
    private readonly IConfiguration _config;
    private readonly PasswordHasher<User> _passwordHasher;

    public AuthController(ApexLawFirmDbContext context, IConfiguration config){
      _context = context;
      _config = config;
      _passwordHasher = new PasswordHasher<User>();
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(AuthRequest request){
      if(await _context.Users.AnyAsync(u => u.Email == request.Email))
        return BadRequest("Email already registered.");

      var userRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "User");
      if(userRole == null)
        return StatusCode(500, "Default role 'User' not found in database.");

      var user = new User{
        Email = request.Email,
        FullName = request.FullName ?? "",
        LastName = request.LastName ?? "",
        RoleId = userRole.Id,
        PhoneNumber = request.PhoneNumber ?? "",
      };
      user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);

      _context.Users.Add(user);
      await _context.SaveChangesAsync();

      return Ok("User registered.");
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(AuthRequest request){
      var user = await _context.Users
        .Include(u => u.Role)
        .FirstOrDefaultAsync(u => u.Email == request.Email);

      if(user == null)
        return Unauthorized("Invalid credentials.");

      var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
      if(result == PasswordVerificationResult.Failed)
        return Unauthorized("Invalid credentials.");

      var claims = new[]{
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Email, user.Email),
        new Claim(ClaimTypes.Role, user.Role?.Name ?? "User")
      };
      
      var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT_KEY"]!));
      var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
      var token = new JwtSecurityToken(
        issuer: _config["JWT_ISSUER"],
        audience: _config["JWT_AUDIENCE"],
        claims: claims,
        expires: DateTime.UtcNow.AddHours(2),
        signingCredentials: creds
      );

      return Ok(new AuthResponse{
        Token = new JwtSecurityTokenHandler().WriteToken(token),
        Role = user.Role?.Name ?? "User"
      });
    }
  }
}
