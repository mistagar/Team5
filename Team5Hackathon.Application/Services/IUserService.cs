using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Team5Hackathon.Application.DTOs.UserDTO;

namespace Team5Hackathon.Application.Services
{
    public interface IUserService
    {
        Task<bool> RegisterAsync(RegisterDTO dto);
        Task<LoginResponseDTO> LoginAsync(LoginDTO dto, string ipAddress, string userAgent);
        Task<RefreshTokenResponseDTO> RefreshTokenAsync(RefreshTokenRequestDTO dto, string ipAddress, string userAgent);
        Task<bool> RevokeRefreshTokenAsync(string token, string ipAddress);
        Task<EmailConfirmationTokenResponseDTO?> SendConfirmationEmailAsync(string email);
        Task<bool> VerifyConfirmationEmailAsync(ConfirmEmailDTO dto);
        Task<ForgotPasswordResponseDTO?> ForgotPasswordAsync(string email);
        Task<bool> ResetPasswordAsync(Guid userId, string token, string newPassword);
        Task<bool> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword);
        Task<ProfileDTO?> GetProfileAsync(Guid userId);
        Task<bool> UpdateProfileAsync(UpdateProfileDTO dto);               
        Task<bool> IsUserExistsAsync(Guid userId);
       
    }
}
