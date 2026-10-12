using InterviewPilot.Application.DTOs;
using InterviewPilot.Application.Interfaces;
using InterviewPilot.Domain.Entities;
using InterviewPilot.Domain.Enums;
using InterviewPilot.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace InterviewPilot.Api.Controllers;

[ApiController]
[Route("api/companies")]
public class CompanyIntelligenceController : ControllerBase
{
    private readonly IAtsDetectionService _atsService;
    private readonly IRepository<Company> _companyRepo;
    private readonly IRepository<Job> _jobRepo;

    public CompanyIntelligenceController(
        IAtsDetectionService atsService,
        IRepository<Company> companyRepo,
        IRepository<Job> jobRepo)
    {
        _atsService = atsService;
        _companyRepo = companyRepo;
        _jobRepo = jobRepo;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<CompanyDto>>> GetCompanies(CancellationToken cancellationToken)
    {
        var companies = await _companyRepo.GetAllAsync(cancellationToken);
        var jobs = await _jobRepo.GetAllAsync(cancellationToken);

        var dtos = companies.Select(c => new CompanyDto
        {
            Id = c.Id,
            Name = c.Name,
            Website = c.Website,
            CareerUrl = c.CareerUrl,
            ATSProvider = c.ATSProvider,
            Industry = c.Industry,
            Locations = c.Locations,
            ActiveJobsCount = jobs.Count(j => j.CompanyName.Equals(c.Name, StringComparison.OrdinalIgnoreCase))
        }).ToList();

        if (dtos.Count == 0)
        {
            // Initial seed companies
            dtos.AddRange([
                new CompanyDto { Id = Guid.NewGuid(), Name = "Stripe", CareerUrl = "https://boards.greenhouse.io/stripe", ATSProvider = ATSProvider.Greenhouse, Industry = "Fintech", Locations = "Remote, San Francisco", ActiveJobsCount = 14 },
                new CompanyDto { Id = Guid.NewGuid(), Name = "Vercel", CareerUrl = "https://boards.greenhouse.io/vercel", ATSProvider = ATSProvider.Greenhouse, Industry = "Cloud & DevTools", Locations = "Remote", ActiveJobsCount = 8 },
                new CompanyDto { Id = Guid.NewGuid(), Name = "Spotify", CareerUrl = "https://jobs.lever.co/spotify", ATSProvider = ATSProvider.Lever, Industry = "Streaming & Media", Locations = "New York, Stockholm", ActiveJobsCount = 19 },
                new CompanyDto { Id = Guid.NewGuid(), Name = "Palantir", CareerUrl = "https://api.lever.co/v0/postings/palantir", ATSProvider = ATSProvider.Lever, Industry = "Enterprise AI", Locations = "New York, London", ActiveJobsCount = 22 }
            ]);
        }

        return Ok(dtos);
    }

    [HttpPost("detect-ats")]
    public async Task<ActionResult<object>> DetectAts([FromQuery] string careerUrl, CancellationToken cancellationToken)
    {
        var provider = await _atsService.DetectAtsProviderAsync(careerUrl, cancellationToken);
        return Ok(new { careerUrl, atsProvider = provider.ToString() });
    }
}
