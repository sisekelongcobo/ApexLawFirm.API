namespace ApexLawFirm.API.DTOs
{
    public class LawyerSpecializationDto
    {
        public int LawyerId { get; set; }
        public int SpecializationId { get; set; }
        public SpecializationDto Specialization { get; set; } = null!;
    }
}
