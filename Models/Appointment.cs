using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ApexLawFirm.API.Models{
	public class Appointment{
		public int Id { get; set; }
		public int UserId { get; set; }
		public int LawyerId { get; set; }
		public string CaseDescription { get; set; } = null!;
		public DateTime AppointmentDate { get; set; }
		public string Status { get; set; } = "Pending"; 
		[JsonIgnore]
		public User User { get; set; } = null!;
		[ForeignKey("LawyerId")]
		[JsonIgnore]
		public LawyerProfile LawyerProfile { get; set; } = null!;
	}
}
