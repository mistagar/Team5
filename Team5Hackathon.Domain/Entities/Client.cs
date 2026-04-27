using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Team5Hackathon.Domain.Entities
{
    [Index(nameof(ClientName), Name = "Index_ClientName_Unique", IsUnique = true)]
    public class Client
    {
        public string ClientId { get; set; } = null!;
        public string ClientName { get; set; } = null!; // e.g., "Web", "Android", "iOS"
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    }
}
