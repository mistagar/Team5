using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Team5Hackathon.Application.DTOs.UserDTO
{
    public class LoginDTO
    {
        [Required(ErrorMessage = "Email or Username is required.")]
        public string EmailOrUserName { get; set; } = null!;
        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; } = null!;
        [Required(ErrorMessage = "ClientId is required.")]
        public string ClientId { get; set; } = null!;
    }
}
