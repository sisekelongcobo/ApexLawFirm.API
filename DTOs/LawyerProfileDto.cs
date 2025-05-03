using ApexLawFirm.API.Models;

namespace ApexLawFirm.API.DTOs{
  public class LawyerProfileDto{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Biography { get; set; } = null!;
    public string RegistrationBody { get; set; } = null!;
    public string RegistrationNumber { get; set; } = null!;
    public int YearsExperience { get; set; }
    public string ProfilePhoto { get; set; } = null!;
    public UserDto User { get; set; } = null!;
    public List<LawyerSpecializationDto> LawyerSpecializations { get; set; } = new();
  }

  public class LawyerProfileResponseDto{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Biography { get; set; } = string.Empty;
    public string? ProfilePhoto { get; set; }
    public string RegistrationBody { get; set; } = string.Empty;
    public string RegistrationNumber { get; set; } = string.Empty;
    public int YearsExperience { get; set; }
    public List<string> Specializations { get; set; } = new();
    public double? AverageRating { get; set; } 
  }

  public class UpdateLawyerProfileDto{
    public string Biography { get; set; } = null!;
    public string RegistrationBody { get; set; } = null!;
    public string RegistrationNumber { get; set; } = null!;
    public int YearsExperience { get; set; }
  }

  public class AddLawyerProfileDto{
    public int UserId { get; set; }
    public string Biography { get; set; } = null!;
    public string RegistrationBody { get; set; } = null!;
    public string RegistrationNumber { get; set; } = null!;
    public int YearsExperience { get; set; } 
    public string Education { get; set; } = null!;
    public string Languages { get; set; } = null!;
    public RoleDto Role { get; set; } = null!;
  }

  public class LawyerSpecializationDto{
    public int LawyerId { get; set; }
    public int SpecializationId { get; set; }
    public SpecializationDto Specialization { get; set; } = null!;
  }

  public class SpecializationDto{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
  }

  public class AssignSpecializationDto {
    public int SpecializationId { get; set; }
  }
}
