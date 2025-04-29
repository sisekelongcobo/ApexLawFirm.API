using System.Text.Json.Serialization;

namespace ApexLawFirm.API.Models{
  public class Specialization{
    public int Id { get; set; }
    public string Name { get; set; } = null!; // e.g., Criminal Law, Corporate Law, Family Law
    [JsonIgnore]
    public ICollection<LawyerSpecialization> LawyerSpecializations { get; set; } = new List<LawyerSpecialization>();
  }
}
