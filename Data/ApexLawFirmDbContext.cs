using ApexLawFirm.API.Models;
using Microsoft.EntityFrameworkCore;

namespace ApexLawFirm.API.Data{
  public class ApexLawFirmDbContext : DbContext{
    public ApexLawFirmDbContext(DbContextOptions<ApexLawFirmDbContext> options)
    : base(options){ }

    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<LawyerProfile> LawyerProfiles { get; set; }
    public DbSet<Specialization> Specializations { get; set; }
    public DbSet<LawyerSpecialization> LawyerSpecializations { get; set; }
    public DbSet<Appointment> Appointments { get; set; }
    public DbSet<Review> Reviews { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder){
      base.OnModelCreating(modelBuilder);

      modelBuilder.Entity<LawyerProfile>()
        .ToTable("lawyerprofiles");

      modelBuilder.Entity<LawyerSpecialization>()
        .HasKey(ls => new { ls.LawyerId, ls.SpecializationId });

      modelBuilder.Entity<LawyerSpecialization>()
        .HasOne(ls => ls.LawyerProfile)
        .WithMany(lp => lp.LawyerSpecializations)
        .HasForeignKey(ls => ls.LawyerId);

      modelBuilder.Entity<LawyerSpecialization>()
        .HasOne(ls => ls.Specialization)
        .WithMany(s => s.LawyerSpecializations)
        .HasForeignKey(ls => ls.SpecializationId);

      modelBuilder.Entity<Review>()
        .HasOne(r => r.User)
        .WithMany()
        .HasForeignKey(r => r.UserId)
        .OnDelete(DeleteBehavior.Restrict);
      
      modelBuilder.Entity<Review>()
        .HasOne(r => r.LawyerProfile)
        .WithMany()
        .HasForeignKey(r => r.LawyerId)
        .OnDelete(DeleteBehavior.Restrict); 
      
      modelBuilder.Entity<Role>().HasData(
      new Role { Id = 1, Name = "User" },
      new Role { Id = 2, Name = "Lawyer" },
      new Role { Id = 3, Name = "Admin" });
    }
  }
}
