using System.Text.Json.Serialization;

namespace ApexLawFirm.API.Models{
  public class LawyerProfile{
    public int Id { get; set; }
    public int UserId { get; set; } 
    public string Biography { get; set; } = null!;
    public string RegistrationBody { get; set; } = null!;
    public string RegistrationNumber { get; set; } = null!;
    public int YearsExperience { get; set; }
    public string ProfilePhoto { get; set; } = null!;

    public User User { get; set; } = null!;
    public ICollection<LawyerSpecialization> LawyerSpecializations { get; set; } = new List<LawyerSpecialization>();
    [JsonIgnore]
    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
  }
}
