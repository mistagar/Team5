using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Team5Hackathon.Domain.Entities
{
    [Index(nameof(Email), Name = "Index_Email_Unique", IsUnique = true)]
    public class User
    {
        public Guid Id { get; set; }
        public string? UserName { get; set; } = null!;
        public string? Email { get; set; } = null!;
        public bool IsEmailConfirmed { get; set; }
        public bool IsActive { get; set; }
        public string? PhoneNumber { get; set; }
        public string? FullName { get; set; }
        public string? ProfilePhotoUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public ICollection<UserRequest> Requests { get; set; } = new List<UserRequest>();
    }
}
