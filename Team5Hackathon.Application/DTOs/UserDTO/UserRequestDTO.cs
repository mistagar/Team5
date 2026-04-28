using System;

namespace Team5Hackathon.Application.DTOs.UserDTO
{
    public class UserRequestDTO
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string? Type { get; set; }
        public string? Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? Status { get; set; }
        public string? Summary { get; set; }
        public string? Response { get; set; }
    }

    public class CreateUserRequestDTO
    {
        public Guid UserId { get; set; }
        public string? Type { get; set; }
        public string? Content { get; set; }
    }
}