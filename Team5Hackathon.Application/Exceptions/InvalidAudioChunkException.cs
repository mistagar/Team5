namespace Team5Hackathon.Application.Exceptions;

public sealed class InvalidAudioChunkException : Exception
{
    public InvalidAudioChunkException(string message) : base(message)
    {
    }
}
