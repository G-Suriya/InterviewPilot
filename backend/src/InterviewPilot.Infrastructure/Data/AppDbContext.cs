using InterviewPilot.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using AppEntity = InterviewPilot.Domain.Entities.Application;

namespace InterviewPilot.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<CandidateProfile> CandidateProfiles => Set<CandidateProfile>();
    public DbSet<Skill> Skills => Set<Skill>();
    public DbSet<CandidateSkill> CandidateSkills => Set<CandidateSkill>();
    public DbSet<Company> Companies => Set<Company>();
    public DbSet<Job> Jobs => Set<Job>();
    public DbSet<JobSkill> JobSkills => Set<JobSkill>();
    public DbSet<AppEntity> Applications => Set<AppEntity>();
    public DbSet<Resume> Resumes => Set<Resume>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // CandidateSkill relationships
        modelBuilder.Entity<CandidateSkill>()
            .HasOne(cs => cs.CandidateProfile)
            .WithMany(cp => cp.Skills)
            .HasForeignKey(cs => cs.CandidateProfileId);

        modelBuilder.Entity<CandidateSkill>()
            .HasOne(cs => cs.Skill)
            .WithMany(s => s.CandidateSkills)
            .HasForeignKey(cs => cs.SkillId);

        // JobSkill relationships
        modelBuilder.Entity<JobSkill>()
            .HasOne(js => js.Job)
            .WithMany(j => j.Skills)
            .HasForeignKey(js => js.JobId);

        modelBuilder.Entity<JobSkill>()
            .HasOne(js => js.Skill)
            .WithMany(s => s.JobSkills)
            .HasForeignKey(js => js.SkillId);

        // Seed Core Skills
        var netSkillId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var csharpSkillId = Guid.Parse("22222222-2222-2222-2222-222222222222");
        var azureSkillId = Guid.Parse("33333333-3333-3333-3333-333333333333");
        var reactSkillId = Guid.Parse("44444444-4444-4444-4444-444444444444");
        var sqlSkillId = Guid.Parse("55555555-5555-5555-5555-555555555555");

        modelBuilder.Entity<Skill>().HasData(
            new Skill { Id = netSkillId, Name = ".NET", Category = "Backend" },
            new Skill { Id = csharpSkillId, Name = "C#", Category = "Language", ParentSkillId = netSkillId },
            new Skill { Id = azureSkillId, Name = "Azure", Category = "Cloud" },
            new Skill { Id = reactSkillId, Name = "React", Category = "Frontend" },
            new Skill { Id = sqlSkillId, Name = "SQL", Category = "Database" }
        );
    }
}
