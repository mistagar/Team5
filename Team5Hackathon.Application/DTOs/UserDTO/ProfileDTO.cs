using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Team5Hackathon.Application.DTOs.UserDTO
{
    public class ProfileDTO
    {
        public Guid UserId { get; set; }
        public string? FullName { get; set; } = null!;
        public string? PhoneNumber { get; set; } = null!;
        public string? Email { get; set; } = null!;
        public string? UserName { get; set; } = null!;
        public DateTime? LastLoginAt { get; set; }
        public string? ProfilePhotoUrl { get; set; }
    }
}
