using InterviewPilot.Application.DTOs;
using InterviewPilot.Application.Interfaces;
using InterviewPilot.Domain.Entities;
using InterviewPilot.Domain.Enums;
using InterviewPilot.Domain.Interfaces;
using AppEntity = InterviewPilot.Domain.Entities.Application;

namespace InterviewPilot.Application.Services;

public class ApplicationService : IApplicationService
{
    private readonly IRepository<AppEntity> _appRepo;
    private readonly IRepository<Job> _jobRepo;
    private readonly IUnitOfWork _unitOfWork;

    public ApplicationService(IRepository<AppEntity> appRepo, IRepository<Job> jobRepo, IUnitOfWork unitOfWork)
    {
        _appRepo = appRepo;
        _jobRepo = jobRepo;
        _unitOfWork = unitOfWork;
    }

    public async Task<IReadOnlyList<ApplicationDto>> GetApplicationsByUserIdAsync(Guid userId, ApplicationStatus? status = null, CancellationToken cancellationToken = default)
    {
        var apps = await _appRepo.FindAsync(a => a.UserId == userId && (!status.HasValue || a.Status == status.Value), cancellationToken);
        return apps.Select(MapToDto).ToList();
    }

    public async Task<ApplicationDto?> GetApplicationByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var app = await _appRepo.GetByIdAsync(id, cancellationToken);
        return app != null ? MapToDto(app) : null;
    }

    public async Task<ApplicationDto> CreateApplicationAsync(Guid userId, Guid jobId, ApplicationMethod method, CancellationToken cancellationToken = default)
    {
        var job = await _jobRepo.GetByIdAsync(jobId, cancellationToken);
        var app = new AppEntity
        {
            UserId = userId,
            JobId = jobId,
            Job = job,
            ApplicationMethod = method,
            Status = ApplicationStatus.New,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _appRepo.AddAsync(app, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return MapToDto(app);
    }

    public async Task<ApplicationDto?> UpdateStatusAsync(Guid id, ApplicationStatus newStatus, string? notes = null, CancellationToken cancellationToken = default)
    {
        var app = await _appRepo.GetByIdAsync(id, cancellationToken);
        if (app == null) return null;

        app.Status = newStatus;
        if (!string.IsNullOrEmpty(notes))
            app.Notes = notes;
        if (newStatus == ApplicationStatus.Applied && !app.AppliedAt.HasValue)
            app.AppliedAt = DateTime.UtcNow;

        app.UpdatedAt = DateTime.UtcNow;
        _appRepo.Update(app);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToDto(app);
    }

    private static ApplicationDto MapToDto(AppEntity a)
    {
        return new ApplicationDto
        {
            Id = a.Id,
            UserId = a.UserId,
            JobId = a.JobId,
            JobTitle = a.Job?.Title ?? "Role",
            CompanyName = a.Job?.CompanyName ?? "Company",
            Status = a.Status,
            MatchScore = a.MatchScore,
            ApplicationMethod = a.ApplicationMethod,
            ExternalApplicationId = a.ExternalApplicationId,
            Notes = a.Notes,
            AppliedAt = a.AppliedAt,
            CreatedAt = a.CreatedAt
        };
    }
}
