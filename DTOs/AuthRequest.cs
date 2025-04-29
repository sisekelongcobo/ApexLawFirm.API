namespace ApexLawFirm.API.DTOs{
  public class AuthRequest{
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string? PhoneNumber { get; set; } = null!;
    public string? FullName { get; set; } = null!;
    public string? LastName { get; set; } = null!;
  }
}
