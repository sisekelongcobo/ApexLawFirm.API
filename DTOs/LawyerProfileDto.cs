namespace ApexLawFirm.API.DTOs
{
    public class LawyerProfileDto
    {
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
}


// DTOs/UpdateLawyerProfileDto.cs
namespace ApexLawFirm.API.DTOs{
    public class UpdateLawyerProfileDto {
        public string Biography { get; set; } = null!;
        public string RegistrationBody { get; set; } = null!;
        public string RegistrationNumber { get; set; } = null!;
        public int YearsExperience { get; set; }
    }
}
