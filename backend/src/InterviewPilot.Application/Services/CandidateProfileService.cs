using InterviewPilot.Application.DTOs;
using InterviewPilot.Application.Interfaces;
using InterviewPilot.Domain.Entities;
using InterviewPilot.Domain.Interfaces;

namespace InterviewPilot.Application.Services;

public class CandidateProfileService : ICandidateProfileService
{
    private readonly IRepository<CandidateProfile> _profileRepo;
    private readonly IRepository<User> _userRepo;
    private readonly IUnitOfWork _unitOfWork;

    public CandidateProfileService(
        IRepository<CandidateProfile> profileRepo,
        IRepository<User> userRepo,
        IUnitOfWork unitOfWork)
    {
        _profileRepo = profileRepo;
        _userRepo = userRepo;
        _unitOfWork = unitOfWork;
    }

    public async Task<CandidateProfileDto?> GetProfileByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var profiles = await _profileRepo.FindAsync(p => p.UserId == userId, cancellationToken);
        var profile = profiles.FirstOrDefault();
        return profile != null ? MapToDto(profile) : null;
    }

    public async Task<CandidateProfileDto> UpsertProfileAsync(Guid userId, CandidateProfileDto dto, CancellationToken cancellationToken = default)
    {
        var profiles = await _profileRepo.FindAsync(p => p.UserId == userId, cancellationToken);
        var profile = profiles.FirstOrDefault();

        if (profile == null)
        {
            // Verify or create User
            var user = await _userRepo.GetByIdAsync(userId, cancellationToken);
            if (user == null)
            {
                user = new User { Id = userId, Name = "Candidate", Email = "candidate@interviewpilot.io" };
                await _userRepo.AddAsync(user, cancellationToken);
            }

            profile = new CandidateProfile
            {
                UserId = userId,
                Summary = dto.Summary,
                YearsExperience = dto.YearsExperience,
                NoticePeriodDays = dto.NoticePeriodDays,
                SalaryExpectationMin = dto.SalaryExpectationMin,
                SalaryExpectationMax = dto.SalaryExpectationMax,
                Currency = dto.Currency,
                RemotePreference = dto.RemotePreference,
                PreferredLocations = string.Join(",", dto.PreferredLocations),
                TargetRoles = string.Join(",", dto.TargetRoles),
                ExcludedCompanies = string.Join(",", dto.ExcludedCompanies)
            };
            await _profileRepo.AddAsync(profile, cancellationToken);
        }
        else
        {
            profile.Summary = dto.Summary;
            profile.YearsExperience = dto.YearsExperience;
            profile.NoticePeriodDays = dto.NoticePeriodDays;
            profile.SalaryExpectationMin = dto.SalaryExpectationMin;
            profile.SalaryExpectationMax = dto.SalaryExpectationMax;
            profile.Currency = dto.Currency;
            profile.RemotePreference = dto.RemotePreference;
            profile.PreferredLocations = string.Join(",", dto.PreferredLocations);
            profile.TargetRoles = string.Join(",", dto.TargetRoles);
            profile.ExcludedCompanies = string.Join(",", dto.ExcludedCompanies);
            _profileRepo.Update(profile);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return MapToDto(profile);
    }

    private static CandidateProfileDto MapToDto(CandidateProfile profile)
    {
        return new CandidateProfileDto
        {
            Id = profile.Id,
            UserId = profile.UserId,
            Summary = profile.Summary,
            YearsExperience = profile.YearsExperience,
            NoticePeriodDays = profile.NoticePeriodDays,
            SalaryExpectationMin = profile.SalaryExpectationMin,
            SalaryExpectationMax = profile.SalaryExpectationMax,
            Currency = profile.Currency,
            RemotePreference = profile.RemotePreference,
            PreferredLocations = (profile.PreferredLocations ?? "").Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList(),
            TargetRoles = (profile.TargetRoles ?? "").Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList(),
            ExcludedCompanies = (profile.ExcludedCompanies ?? "").Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList(),
            Skills = profile.Skills.Select(s => new CandidateSkillDto
            {
                Name = s.Skill?.Name ?? "",
                Category = s.Skill?.Category ?? "",
                YearsExperience = s.YearsExperience,
                Proficiency = s.Proficiency
            }).ToList()
        };
    }
}
