using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Team5Hackathon.Application.DTOs.UserDTO
{
    public class ConfirmEmailDTO
    {
        [Required(ErrorMessage = "User ID is required.")]
        public Guid UserId { get; set; }
        [Required(ErrorMessage = "Confirmation token is required.")]
        public string Token { get; set; } = null!;
    }
}
