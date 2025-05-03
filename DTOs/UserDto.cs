namespace ApexLawFirm.API.DTOs{
  public class UserDto{
    public int Id { get; set; }
    public string FullName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public RoleDto? Role { get; set; }
  }
}
