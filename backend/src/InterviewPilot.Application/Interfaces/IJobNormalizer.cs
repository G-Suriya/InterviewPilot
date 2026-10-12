using InterviewPilot.Domain.Entities;
using InterviewPilot.Domain.Models;

namespace InterviewPilot.Application.Interfaces;

public interface IJobNormalizer
{
    Job Normalize(RawJob rawJob);
}
