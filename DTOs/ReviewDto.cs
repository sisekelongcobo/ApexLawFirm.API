namespace ApexLawFirm.API.DTOs{
  public class ReviewCreateDto{
    public int LawyerId { get; set; }
    public int Rating { get; set; } // 1 to 5
    public string? Comment { get; set; }
  }

  public class ReviewResponseDto{
    public int Id { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public DateTime CreatedAt { get; set; }
    public string UserName { get; set; } = string.Empty;
  }
}
