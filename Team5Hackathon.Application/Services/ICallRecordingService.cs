using System;
using System.Threading.Tasks;
using Team5Hackathon.Application.DTOs;

namespace Team5Hackathon.Application.Services
{
    public interface ICallRecordingService
    {
        Task<bool> StartRecording(StartRecordingDTO startRecordingDTO);
        Task<byte[]> StopRecording(Guid callId);
        void AddAudioChunk(Guid callId, byte[] chunk);
    }
}
