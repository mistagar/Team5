using Microsoft.EntityFrameworkCore;
using Team5Hackathon.API.Configuration;
using Team5Hackathon.API.BackgroundServices;
using Team5Hackathon.Application.Services;
using Team5Hackathon.Application.Streaming;
using Team5Hackathon.Domain.RepositoriesContract;
using Team5Hackathon.Infrastructure.Persistence;
using Team5Hackathon.Infrastructure.Repositories;

namespace Team5Hackathon.API.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationModules(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<ApiSecurityOptions>(configuration.GetSection(ApiSecurityOptions.SectionName));

        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("ConnectionStrings:DefaultConnection is required.");

        services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));
        services.AddScoped<IAuditLogRepository, AuditLogRepository>();
        services.AddScoped<IAuditService, AuditService>();
        services.AddScoped<IAudioStreamIngestionService, AudioStreamIngestionService>();
        services.AddSingleton<IAudioStreamQueue, InMemoryAudioStreamQueue>();
        services.AddSingleton(new InMemoryAudioChunkBuffer(maxChunksPerCall: 25));
        services.AddHostedService<AudioStreamProcessingWorker>();

        return services;
    }
}
