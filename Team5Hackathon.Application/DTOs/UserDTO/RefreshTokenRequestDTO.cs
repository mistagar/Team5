using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Team5Hackathon.Application.DTOs.UserDTO
{
    public class RefreshTokenRequestDTO
    {
        [Required(ErrorMessage = "Refresh token is required.")]
        public string RefreshToken { get; set; } = null!;
        [Required(ErrorMessage = "Client ID is required.")]
        public string ClientId { get; set; } = null!; // e.g., "Web", "Android", "iOS"
    }
}
