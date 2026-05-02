using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading.Tasks;
using Team5Hackathon.Application.DTOs;

namespace Team5Hackathon.Application.Services
{
    public class CallRecordingService : ICallRecordingService
    {
        private static readonly ConcurrentDictionary<Guid, MemoryStream> _recordingStreams = new ConcurrentDictionary<Guid, MemoryStream>();

        public Task<bool> StartRecording(StartRecordingDTO startRecordingDTO)
        {
            _recordingStreams.TryAdd(startRecordingDTO.CallId, new MemoryStream());
            return Task.FromResult(true);
        }

        public Task<byte[]> StopRecording(Guid callId)
        {
            if (_recordingStreams.TryRemove(callId, out var stream))
            {
                var recordingBytes = stream.ToArray();
                stream.Dispose();
                return Task.FromResult(recordingBytes);
            }
            return Task.FromResult(Array.Empty<byte>());
        }

        public void AddAudioChunk(Guid callId, byte[] chunk)
        {
            if (_recordingStreams.TryGetValue(callId, out var stream))
            {
                stream.Write(chunk, 0, chunk.Length);
            }
        }
    }
}
