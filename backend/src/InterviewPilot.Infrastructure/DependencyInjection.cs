using InterviewPilot.Application.Interfaces;
using InterviewPilot.Application.Services;
using InterviewPilot.Domain.Interfaces;
using InterviewPilot.Infrastructure.Adapters;
using InterviewPilot.Infrastructure.Data;
using InterviewPilot.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace InterviewPilot.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection") ?? "Data Source=interviewpilot.db";

        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite(connectionString));

        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Adapters
        services.AddScoped<IJobSourceAdapter, MockGreenhouseAdapter>();

        // Application Services
        services.AddScoped<IJobService, JobService>();
        services.AddScoped<ICandidateProfileService, CandidateProfileService>();
        services.AddScoped<IApplicationService, ApplicationService>();
        services.AddScoped<IMatchingEngine, MatchingEngine>();

        return services;
    }
}
