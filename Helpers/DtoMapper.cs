using ApexLawFirm.API.DTOs;
using ApexLawFirm.API.Models;

namespace ApexLawFirm.API.Helpers{
  public static class DtoMapper{
    public static UpdateLawyerProfileDto ToUpdateLawyerProfileDto(this LawyerProfile profile) => new UpdateLawyerProfileDto{
      Biography = profile.Biography,
      RegistrationBody = profile.RegistrationBody,
      RegistrationNumber = profile.RegistrationNumber,
      YearsExperience = profile.YearsExperience,
    };

    public static LawyerProfileDto ToLawyerProfileDto(this LawyerProfile profile) => new LawyerProfileDto{
      Id = profile.Id,
      UserId = profile.UserId,
      Biography = profile.Biography,
      RegistrationBody = profile.RegistrationBody,
      RegistrationNumber = profile.RegistrationNumber,
      YearsExperience = profile.YearsExperience,
      ProfilePhoto = profile.ProfilePhoto,
      User = profile.User != null ? new UserDto{
        // Id = profile.User.Id,
        FullName = profile.User.FullName,
        LastName = profile.User.LastName,
        Email = profile.User.Email,
        PhoneNumber = profile.User.PhoneNumber,
        Role = profile.User.Role != null ? new RoleDto{
          // Id = profile.User.Role.Id,
          Name = profile.User.Role.Name
        } : null
      } : null!,
      LawyerSpecializations = profile.LawyerSpecializations.Select(ls => new LawyerSpecializationDto{
        LawyerId = ls.LawyerId,
        SpecializationId = ls.SpecializationId,
        Specialization = new SpecializationDto{
          Id = ls.Specialization.Id,
          Name = ls.Specialization.Name
        }
      }).ToList()
    };
  }
}
