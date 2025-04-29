using System.Text.Json.Serialization;

namespace ApexLawFirm.API.Models{
  public class User{
    public int Id { get; set; }
    public string FullName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public int? RoleId { get; set; }
    public Role? Role { get; set; }
    [JsonIgnore]
    public LawyerProfile? LawyerProfile { get; set; }
    [JsonIgnore]
    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
  }
}
