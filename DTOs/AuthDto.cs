namespace ApexLawFirm.API.DTOs{
  public class RegisterAuthRequest{
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public string FullName { get; set; } = null!;
    public string LastName { get; set; } = null!;
  }

  public class LoginAuthRequest{
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
  }

  public class ResetPasswordRequest{
    public string Email { get; set; } = null!;
    public string NewPassword { get; set; } = null!;
  }

  public class LoginAuthResponse{
    public string Token { get; set; } = null!;
    public string Role { get; set; } = null!;
  }
}
