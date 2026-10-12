using InterviewPilot.Domain.Entities;
using InterviewPilot.Infrastructure;
using InterviewPilot.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add Infrastructure & Application Services
builder.Services.AddInfrastructureServices(builder.Configuration);

// Add CORS for React frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.SetIsOriginAllowed(origin => true) // allows localhost dev ports
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

// Ensure Database & Seed Data
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.EnsureCreatedAsync();

    if (!await db.Jobs.AnyAsync())
    {
        var sampleJobs = new List<Job>
        {
            new Job
            {
                Title = "Senior .NET Core Engineer",
                CompanyName = "Stripe",
                Location = "Remote",
                IsRemote = true,
                SalaryMin = 180000,
                SalaryMax = 220000,
                Source = "Greenhouse",
                Description = "Seeking an experienced .NET/C# engineer with Azure, microservices, and distributed systems experience.",
                ApplicationUrl = "https://boards.greenhouse.io/stripe/jobs/101"
            },
            new Job
            {
                Title = "Full-Stack Engineer (React & .NET)",
                CompanyName = "Vercel",
                Location = "San Francisco, CA",
                IsRemote = true,
                SalaryMin = 160000,
                SalaryMax = 195000,
                Source = "Greenhouse",
                Description = "Build developer platforms using React, TypeScript, and .NET Core.",
                ApplicationUrl = "https://boards.greenhouse.io/vercel/jobs/102"
            },
            new Job
            {
                Title = "Senior Full-Stack Developer",
                CompanyName = "Ramp",
                Location = "New York, NY",
                IsRemote = false,
                SalaryMin = 175000,
                SalaryMax = 210000,
                Source = "Lever",
                Description = "Scale fintech infrastructure using modern cloud architectures, microservices, and React.",
                ApplicationUrl = "https://jobs.lever.co/ramp/201"
            }
        };
        await db.Jobs.AddRangeAsync(sampleJobs);
        await db.SaveChangesAsync();
    }
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors("AllowReactApp");

app.UseAuthorization();

app.MapControllers();

app.Run();
