using InterviewPilot.Application.DTOs;
using InterviewPilot.Domain.Entities;

namespace InterviewPilot.Application.Interfaces;

public interface IMatchingEngine
{
    MatchingResultDto CalculateMatch(CandidateProfile candidate, Job job);
}
