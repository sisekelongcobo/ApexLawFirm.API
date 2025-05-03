namespace ApexLawFirm.API.DTOs{
  public class AppointmentCreateDto {
    public int LawyerId { get; set; }
    public string CaseDescription { get; set; } = null!;
    public DateTime AppointmentDate { get; set; }
  }

  public class AppointmentStatusUpdateDto {
    public string Status { get; set; } = null!;
  }

  public class AppointmentResponseDto {
    public int Id { get; set; }
    public DateTime AppointmentDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public string CaseDescription { get; set; } = string.Empty;
    public string LawyerName { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
  }
}