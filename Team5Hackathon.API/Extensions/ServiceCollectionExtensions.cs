using Microsoft.EntityFrameworkCore;
using Team5Hackathon.API.Configuration;
using Team5Hackathon.API.BackgroundServices;
<<<<<<< HEAD
=======
using Team5Hackathon.Application.Configuration;
>>>>>>> aad58e4df299d06879af545321d6fe668beb3017
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
<<<<<<< HEAD
=======
        services.Configure<WhisperTranscriptionOptions>(configuration.GetSection(WhisperTranscriptionOptions.SectionName));
>>>>>>> aad58e4df299d06879af545321d6fe668beb3017

        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("ConnectionStrings:DefaultConnection is required.");

        services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString, sqlServerOptionsAction => sqlServerOptionsAction.EnableRetryOnFailure()));
        services.AddScoped<IAuditLogRepository, AuditLogRepository>();
        services.AddScoped<IAuditService, AuditService>();
        services.AddScoped<ICallRepository, CallRepository>();
        services.AddScoped<ICallService, CallService>();
        services.AddScoped<IAIService, AIService>();
        services.AddScoped<IAudioStreamIngestionService, AudioStreamIngestionService>();
<<<<<<< HEAD
=======
        services.AddHttpClient<ITranscriptionService, WhisperTranscriptionService>();
>>>>>>> aad58e4df299d06879af545321d6fe668beb3017
        services.AddSingleton<IAudioStreamQueue, InMemoryAudioStreamQueue>();
        services.AddSingleton(new InMemoryAudioChunkBuffer(maxChunksPerCall: 25));
        services.AddHostedService<AudioStreamProcessingWorker>();

        return services;
    }
}
