using HealthNetDb.Entities;
using Microsoft.EntityFrameworkCore;
namespace HealthNetDb.Data;

public class HealthNetContext : DbContext
{
    public HealthNetContext() { }

    public HealthNetContext(DbContextOptions<HealthNetContext> options) : base(options) { }

    public virtual DbSet<HealthNetDb.Entities.Action> Actions { get; set; }
    public virtual DbSet<Audit> Audits { get; set; }
    public virtual DbSet<AuditLog> AuditLogs { get; set; }
    public virtual DbSet<Cases> Casess { get; set; }
    public virtual DbSet<ComplianceRecord> ComplianceRecords { get; set; }
    public virtual DbSet<Epidemiology> Epidemiologies { get; set; }
    public virtual DbSet<LabReport> LabReports { get; set; }
    public virtual DbSet<LabTest> LabTests { get; set; }
    public virtual DbSet<MedicalRecord> MedicalRecords { get; set; }
    public virtual DbSet<Outbreak> Outbreaks { get; set; }
    public virtual DbSet<Patient> Patients { get; set; }
    public virtual DbSet<Role> Roles { get; set; }
    public virtual DbSet<SymptomReport> SymptomReports { get; set; }
    public virtual DbSet<Users> Userss { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer("");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Users>().HasQueryFilter(u => u.Status == true);
        // Configure Cases -> Users (Citizen)
        // apply cascade onDelete to Citizen
        modelBuilder.Entity<Cases>()
            .HasOne(c => c.Citizen)
            .WithMany()
            .HasForeignKey(c => c.CitizenId)
            .OnDelete(DeleteBehavior.Cascade); // OK to Delete

        // Configure Cases -> Users (Doctor)
        // Removing cascase to Doctor
        modelBuilder.Entity<Cases>()
            .HasOne(c => c.Doctor)
            .WithMany()
            .HasForeignKey(c => c.DoctorId)
            .OnDelete(DeleteBehavior.NoAction); // Keep

        // Configure Audit -> Users (Officer)
        // apply cascade onDelete to Officer
        modelBuilder.Entity<Audit>()
            .HasOne(a => a.Officer)
            .WithMany()
            .HasForeignKey(a => a.OfficerId)
            .OnDelete(DeleteBehavior.Cascade);  //Ok to Delete

        // Configure AuditLog -> Users (user)
        // apply NoAction OnDelete to user
        modelBuilder.Entity<AuditLog>()
            .HasOne(al => al.UsersNavigation)
            .WithMany()
            .HasForeignKey(al => al.UserId)
            .OnDelete(DeleteBehavior.Restrict);  //Keep

        // Configure AuditLog -> Action
        // Apply Cascade onDelete to Action
        modelBuilder.Entity<AuditLog>()
            .HasOne(al => al.ActionNavigation)
            .WithMany()
            .HasForeignKey(al => al.ActionId)
            .OnDelete(DeleteBehavior.Cascade);  //Ok to Delete

        // Configure Epidemiology -> Outbreak
        // apply cascade OnDelete to outbreak
        modelBuilder.Entity<Epidemiology>()
            .HasOne(e => e.OutbreakNavigation)
            .WithMany()
            .HasForeignKey(e => e.OutbreakId)
            .OnDelete(DeleteBehavior.Cascade);  //Ok to Delete


        // Configure LabTest -> Patient
        // apply cascade OnDelete to Patient
        modelBuilder.Entity<LabTest>()
            .HasOne(lt => lt.PatientNavigation)
            .WithMany()
            .HasForeignKey(lt => lt.PatientId)
            .OnDelete(DeleteBehavior.Cascade);  //Ok to Delete

        // Configure LabTest -> Users(Technician)
        // apply cascade OnDelete to Techincian
        modelBuilder.Entity<LabTest>()
            .HasOne(lt => lt.Technician)
            .WithMany()
            .HasForeignKey(lt => lt.TechnicianId)
            .OnDelete(DeleteBehavior.NoAction);  //keep

        // Build one-to-one relation b/w labtest and labreport
        // apply cascade Restrict to lebreport
        modelBuilder.Entity<LabTest>()
            .HasOne(lt => lt.LabReportNavigation)     // LabTest has ONE LabReport
            .WithOne(lr => lr.LabTestNavigation)      // LabReport has ONE LabTest
            .HasForeignKey<LabReport>(lr => lr.TestId) // TestId is the FK on LabReport
            .OnDelete(DeleteBehavior.Restrict);       // Prevent cascade delete loops

        // Configure MedicalRecord -> Patient
        // apply cascade OnDelete to Patient
        modelBuilder.Entity<MedicalRecord>()
            .HasOne(mr => mr.PatientNavigation)
            .WithMany()
            .HasForeignKey(mr => mr.PatientId)
            .OnDelete(DeleteBehavior.Cascade); //Ok to Delete
    }

}
