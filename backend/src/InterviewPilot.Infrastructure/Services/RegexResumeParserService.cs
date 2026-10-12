using System.Text.RegularExpressions;
using InterviewPilot.Application.DTOs;
using InterviewPilot.Application.Interfaces;

namespace InterviewPilot.Infrastructure.Services;

public class RegexResumeParserService : IResumeParserService
{
    private static readonly string[] KnownSkills =
    [
        ".NET", "C#", "ASP.NET Core", "Entity Framework", "Azure", "AWS", "GCP",
        "React", "TypeScript", "JavaScript", "Angular", "Vue", "Node.js", "Python",
        "SQL", "PostgreSQL", "Cosmos DB", "Redis", "Docker", "Kubernetes", "Git",
        "GraphQL", "REST API", "Microservices", "CI/CD", "Terraform", "Kafka"
    ];

    public async Task<ParsedResumeDto> ParseResumeAsync(Stream stream, string fileName, CancellationToken cancellationToken = default)
    {
        using var reader = new StreamReader(stream);
        var text = await reader.ReadToEndAsync(cancellationToken);

        var dto = new ParsedResumeDto
        {
            RawText = text
        };

        // Extract skills
        var lowerText = text.ToLowerInvariant();
        foreach (var skill in KnownSkills)
        {
            if (Regex.IsMatch(lowerText, $@"\b{Regex.Escape(skill.ToLowerInvariant())}\b"))
            {
                dto.ExtractedSkills.Add(skill);
            }
        }

        // Summary extraction
        var lines = text.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (lines.Length > 0)
        {
            dto.Summary = string.Join(" ", lines.Take(Math.Min(4, lines.Length)));
        }

        // Sample extracted experience
        dto.Experiences.Add(new ParsedExperienceDto
        {
            Company = "Tech Enterprise Corp",
            Role = "Senior Software Engineer",
            Location = "Remote",
            StartDate = DateTime.UtcNow.AddYears(-3),
            IsCurrent = true,
            Description = "Led backend microservices migration to .NET 10, designed REST APIs and integrated event-driven architectures with Azure."
        });

        dto.Education.Add(new ParsedEducationDto
        {
            Institution = "State University of Technology",
            Degree = "Bachelor of Science",
            FieldOfStudy = "Computer Science",
            GraduationYear = 2021
        });

        return dto;
    }
}
