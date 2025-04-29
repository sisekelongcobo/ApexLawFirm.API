using System.Text.Json.Serialization;

namespace ApexLawFirm.API.Models{
  public class LawyerSpecialization{
    public int LawyerId { get; set; }
    public int SpecializationId { get; set; }
    [JsonIgnore]
    public LawyerProfile LawyerProfile { get; set; } = null!;
    public Specialization Specialization { get; set; } = null!;
  }
}
