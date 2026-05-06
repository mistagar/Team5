using Microsoft.EntityFrameworkCore;
using Team5Hackathon.API.Configuration;
using Team5Hackathon.API.BackgroundServices;
using Team5Hackathon.Application.Configuration;
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
        services.Configure<WhisperTranscriptionOptions>(configuration.GetSection(WhisperTranscriptionOptions.SectionName));

        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("ConnectionStrings:DefaultConnection is required.");

        services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString, sqlServerOptionsAction => sqlServerOptionsAction.EnableRetryOnFailure()));
        services.AddScoped<IAuditLogRepository, AuditLogRepository>();
        services.AddScoped<IAuditService, AuditService>();
        services.AddScoped<ICallRepository, CallRepository>();
        services.AddScoped<ICallService, CallService>();
        services.AddScoped<IAIService, AIService>();
        services.AddScoped<ITextToSpeechService, AzureTextToSpeechService>();
        services.AddScoped<IAudioStreamIngestionService, AudioStreamIngestionService>();
        services.AddHttpClient<ITranscriptionService, WhisperTranscriptionService>();
        services.AddScoped<ICallRecordingService, CallRecordingService>();
        services.AddSingleton<IAudioStreamQueue, InMemoryAudioStreamQueue>();
        services.AddSingleton(new InMemoryAudioChunkBuffer(maxChunksPerCall: 25));
        services.AddHostedService<AudioStreamProcessingWorker>();
        services.AddMemoryCache();
        services.AddScoped<IPredictionRepository, PredictionRepository>();
        services.AddScoped<IPredictionQueryService, PredictionQueryService>();
        services.AddScoped<IDecisionEngineService, DecisionEngineService>();
        services.AddScoped<IOfferRepository, OfferRepository>();
        services.AddScoped<IOfferManagementService, OfferManagementService>();
        services.AddScoped<IChannelRepository, ChannelRepository>();
        services.AddScoped<IChannelManagementService, ChannelManagementService>();
        services.AddScoped<IExecutionPolicyRepository, ExecutionPolicyRepository>();
        services.AddScoped<IExecutionPolicyService, ExecutionPolicyService>();
        services.AddScoped<IInterventionQueueRepository, InterventionQueueRepository>();
        services.AddScoped<ISupervisorApprovalService, SupervisorApprovalService>();
        services.AddScoped<IPredictionDataRepository, PredictionDataRepository>();
        services.AddScoped<IPredictionDataService, PredictionDataService>();

        return services;
    }
}
