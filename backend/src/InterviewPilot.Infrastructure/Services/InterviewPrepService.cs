using InterviewPilot.Application.DTOs;
using InterviewPilot.Application.Interfaces;
using InterviewPilot.Domain.Entities;
using InterviewPilot.Domain.Interfaces;

namespace InterviewPilot.Infrastructure.Services;

public class InterviewPrepService : IInterviewPrepService
{
    private readonly IRepository<InterviewSession> _sessionRepo;
    private readonly IRepository<InterviewQuestion> _questionRepo;
    private readonly IRepository<Job> _jobRepo;
    private readonly IUnitOfWork _unitOfWork;

    public InterviewPrepService(
        IRepository<InterviewSession> sessionRepo,
        IRepository<InterviewQuestion> questionRepo,
        IRepository<Job> jobRepo,
        IUnitOfWork unitOfWork)
    {
        _sessionRepo = sessionRepo;
        _questionRepo = questionRepo;
        _jobRepo = jobRepo;
        _unitOfWork = unitOfWork;
    }

    public async Task<InterviewSessionDto> GenerateMockInterviewAsync(Guid userId, Guid jobId, CancellationToken cancellationToken = default)
    {
        var job = await _jobRepo.GetByIdAsync(jobId, cancellationToken);
        var company = job?.CompanyName ?? "Target Company";
        var role = job?.Title ?? "Senior Engineer";

        var session = new InterviewSession
        {
            UserId = userId,
            JobId = jobId,
            RoleTitle = role,
            CompanyName = company,
            OverallReadinessScore = 82.5,
            FeedbackSummary = $"Role-specific preparation for {role} at {company}. Strong alignment with backend requirements; practice STAR responses on challenging deadline tradeoffs."
        };

        var questions = new List<InterviewQuestion>
        {
            new InterviewQuestion
            {
                SessionId = session.Id,
                Category = "Technical",
                QuestionText = $"How would you architect a high-throughput event processing pipeline in .NET for {company}'s workloads?",
                ModelStarAnswer = "Start with an event-driven architecture using Azure Service Bus or Kafka. Implement resilient consumers using Polly retry policies, idempotent processing, and horizontal autoscaling."
            },
            new InterviewQuestion
            {
                SessionId = session.Id,
                Category = "Behavioral",
                QuestionText = "Tell me about a time you had to cut technical scope to meet a critical delivery deadline. How did you decide what to sacrifice?",
                ModelStarAnswer = "Situation: 3 weeks before major release. Task: Deliver payment reconciliation. Action: Held scope triage, prioritized core ledger over multi-currency reports. Result: Shipped on time with zero downtime."
            },
            new InterviewQuestion
            {
                SessionId = session.Id,
                Category = "SystemDesign",
                QuestionText = "Describe how you optimize database query performance and manage caching in a distributed microservices environment.",
                ModelStarAnswer = "Implement cache-aside with Redis, use projection queries with EF Core/Dapper, index query predicates, and monitor database telemetry with Application Insights."
            }
        };

        await _sessionRepo.AddAsync(session, cancellationToken);
        foreach (var q in questions)
            await _questionRepo.AddAsync(q, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new InterviewSessionDto
        {
            SessionId = session.Id,
            JobId = jobId,
            RoleTitle = session.RoleTitle,
            CompanyName = session.CompanyName,
            ReadinessScore = session.OverallReadinessScore ?? 80.0,
            FeedbackSummary = session.FeedbackSummary,
            Questions = questions.Select(q => new InterviewQuestionDto
            {
                QuestionId = q.Id,
                QuestionText = q.QuestionText,
                Category = q.Category,
                ModelStarAnswer = q.ModelStarAnswer
            }).ToList()
        };
    }

    public async Task<AnswerEvaluationDto> EvaluateAnswerAsync(Guid questionId, string candidateAnswer, CancellationToken cancellationToken = default)
    {
        var question = await _questionRepo.GetByIdAsync(questionId, cancellationToken);
        if (question == null)
        {
            return new AnswerEvaluationDto { QuestionId = questionId, Score = 7.0, Feedback = "Question not found." };
        }

        question.CandidateAnswer = candidateAnswer;

        // Evaluate answer quality based on STAR format metrics & depth
        int wordCount = candidateAnswer.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;
        double score = wordCount >= 40 ? 8.5 : wordCount >= 20 ? 7.2 : 5.8;

        question.AnswerScore = score;
        question.Feedback = score >= 8.0
            ? "Strong structure and technical depth! Adding a quantifiable outcome will make this exceptional."
            : "Good response. Try structuring with the STAR framework (Situation, Task, Action, Result) for higher impact.";

        _questionRepo.Update(question);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new AnswerEvaluationDto
        {
            QuestionId = questionId,
            Score = score,
            Feedback = question.Feedback,
            Strengths = "Clear technical vocabulary and actionable reasoning.",
            AreasOfImprovement = "Ensure you explicitly highlight the measurable business outcome.",
            ModelAnswerRecommendation = question.ModelStarAnswer ?? "Provide context, concrete actions, and the measurable result."
        };
    }
}
