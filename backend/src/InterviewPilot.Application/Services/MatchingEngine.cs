using InterviewPilot.Application.DTOs;
using InterviewPilot.Application.Interfaces;
using InterviewPilot.Domain.Entities;
using InterviewPilot.Domain.Enums;
using InterviewPilot.Domain.Interfaces;
using InterviewPilot.Domain.Models;

namespace InterviewPilot.Application.Services;

public class MatchingEngine : IMatchingEngine
{
    private const double WeightTechnicalSkills = 0.30;
    private const double WeightSemanticSimilarity = 0.25;
    private const double WeightExperience = 0.15;
    private const double WeightRole = 0.10;
    private const double WeightLocation = 0.10;
    private const double WeightSalary = 0.05;
    private const double WeightPreference = 0.05;

    public MatchingResultDto CalculateMatch(CandidateProfile candidate, Job job)
    {
        var breakdown = new MatchBreakdown();

        // 1. Technical Skills Match (30%)
        var candidateSkillNames = candidate.Skills
            .Where(cs => cs.Skill != null)
            .Select(cs => cs.Skill!.Name.ToLowerInvariant())
            .ToHashSet();

        var jobRequiredSkills = job.Skills
            .Where(js => js.Skill != null)
            .Select(js => js.Skill!.Name)
            .ToList();

        if (jobRequiredSkills.Count > 0)
        {
            var matched = new List<string>();
            var missing = new List<string>();

            foreach (var skill in jobRequiredSkills)
            {
                if (candidateSkillNames.Contains(skill.ToLowerInvariant()))
                    matched.Add(skill);
                else
                    missing.Add(skill);
            }

            breakdown.MatchedSkills = matched;
            breakdown.MissingSkills = missing;
            breakdown.TechnicalSkillMatch = (double)matched.Count / jobRequiredSkills.Count * 100.0;
        }
        else
        {
            breakdown.TechnicalSkillMatch = 80.0;
        }

        // 2. Semantic Similarity (25%)
        var jobText = $"{job.Title} {job.Description}".ToLowerInvariant();
        int matchedInDesc = candidateSkillNames.Count(s => jobText.Contains(s));
        breakdown.SemanticSimilarity = candidateSkillNames.Count > 0
            ? Math.Min(100.0, (double)matchedInDesc / candidateSkillNames.Count * 110.0)
            : 70.0;

        // 3. Experience Match (15%)
        int assumedRequiredExp = 3;
        if (candidate.YearsExperience >= assumedRequiredExp)
            breakdown.ExperienceMatch = 100.0;
        else if (candidate.YearsExperience > 0)
            breakdown.ExperienceMatch = Math.Round((double)candidate.YearsExperience / assumedRequiredExp * 100.0, 1);
        else
            breakdown.ExperienceMatch = 50.0;

        // 4. Role Match (10%)
        var targetRoles = (candidate.TargetRoles ?? "")
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(r => r.ToLowerInvariant())
            .ToList();

        if (targetRoles.Count > 0)
        {
            var jobTitle = job.Title.ToLowerInvariant();
            bool matchesRole = targetRoles.Any(tr => jobTitle.Contains(tr) || tr.Contains(jobTitle));
            breakdown.RoleMatch = matchesRole ? 100.0 : 60.0;
        }
        else
        {
            breakdown.RoleMatch = 80.0;
        }

        // 5. Location Match (10%)
        if (job.IsRemote && candidate.RemotePreference != RemotePreference.OnSite)
        {
            breakdown.LocationMatch = 100.0;
        }
        else
        {
            var preferredLocations = (candidate.PreferredLocations ?? "")
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(l => l.ToLowerInvariant())
                .ToList();

            var jobLoc = job.Location.ToLowerInvariant();
            bool locMatch = preferredLocations.Any(pl => jobLoc.Contains(pl) || pl.Contains(jobLoc));
            breakdown.LocationMatch = locMatch ? 100.0 : 50.0;
        }

        // 6. Salary Match (5%)
        if (candidate.SalaryExpectationMin.HasValue && job.SalaryMax.HasValue)
        {
            if (job.SalaryMax.Value >= candidate.SalaryExpectationMin.Value)
                breakdown.SalaryMatch = 100.0;
            else
                breakdown.SalaryMatch = 60.0;
        }
        else
        {
            breakdown.SalaryMatch = 90.0;
        }

        // 7. Preference Match (5%)
        var excluded = (candidate.ExcludedCompanies ?? "")
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(c => c.ToLowerInvariant())
            .ToList();

        if (excluded.Any(ex => job.CompanyName.ToLowerInvariant().Contains(ex)))
            breakdown.PreferenceMatch = 0.0;
        else
            breakdown.PreferenceMatch = 95.0;

        breakdown.OverallScore = Math.Round(
            (breakdown.TechnicalSkillMatch * WeightTechnicalSkills) +
            (breakdown.SemanticSimilarity * WeightSemanticSimilarity) +
            (breakdown.ExperienceMatch * WeightExperience) +
            (breakdown.RoleMatch * WeightRole) +
            (breakdown.LocationMatch * WeightLocation) +
            (breakdown.SalaryMatch * WeightSalary) +
            (breakdown.PreferenceMatch * WeightPreference),
            1
        );

        breakdown.Summary = breakdown.OverallScore >= 85.0
            ? "Strong match across primary skills, role, and location."
            : breakdown.OverallScore >= 70.0
                ? "Moderate match; some skills or role requirements differ."
                : "Low match with several missing core prerequisites.";

        return new MatchingResultDto
        {
            JobId = job.Id,
            JobTitle = job.Title,
            CompanyName = job.CompanyName,
            Breakdown = breakdown
        };
    }
}
