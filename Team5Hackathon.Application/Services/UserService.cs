using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Team5Hackathon.Application.DTOs.UserDTO;
using Team5Hackathon.Domain.Entities;
using Team5Hackathon.Domain.RepositoriesContract;

namespace Team5Hackathon.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        public UserService(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }
        public async Task<bool> RegisterAsync(RegisterDTO dto)
        {
            if (await _userRepository.FindByEmailAsync(dto.Email) != null)
                return false;
            if (await _userRepository.FindByUserNameAsync(dto.UserName) != null)
                return false;
            var user = new User
            {
                Id = Guid.NewGuid(),
                UserName = dto.UserName,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                FullName = dto.FullName,
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
                IsEmailConfirmed = false
            };
            var created = await _userRepository.CreateUserAsync(user, dto.Password);
            if (!created)
                return false;
            await _userRepository.AddUserToRoleAsync(user, "Customer");
            return true;
        }



        public async Task<bool> RegisterCustomerAsync(RegisterDTO dto)
        {
            if (await _userRepository.FindByEmailAsync(dto.Email) != null)
                return false;
            if (await _userRepository.FindByUserNameAsync(dto.UserName) != null)
                return false;
            var user = new User
            {
                Id = Guid.NewGuid(),
                UserName = dto.UserName,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                FullName = dto.FullName,
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
                IsEmailConfirmed = false
            };
            var created = await _userRepository.CreateUserAsync(user, dto.Password);
            if (!created)
                return false;
            await _userRepository.AddUserToRoleAsync(user, "Customer");
            return true;
        }



        public async Task<bool> RegisterSupervisorAsync(RegisterDTO dto)
        {
            if (await _userRepository.FindByEmailAsync(dto.Email) != null)
                return false;
            if (await _userRepository.FindByUserNameAsync(dto.UserName) != null)
                return false;
            var user = new User
            {
                Id = Guid.NewGuid(),
                UserName = dto.UserName,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                FullName = dto.FullName,
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
                IsEmailConfirmed = false
            };
            var created = await _userRepository.CreateUserAsync(user, dto.Password);
            if (!created)
                return false;
            await _userRepository.AddUserToRoleAsync(user, "Supervisor");
            return true;
        }


        public async Task<bool> RegisterAdminAsync(RegisterDTO dto)
        {
            if (await _userRepository.FindByEmailAsync(dto.Email) != null)
                return false;
            if (await _userRepository.FindByUserNameAsync(dto.UserName) != null)
                return false;
            var user = new User
            {
                Id = Guid.NewGuid(),
                UserName = dto.UserName,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                FullName = dto.FullName,
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
                IsEmailConfirmed = false
            };
            var created = await _userRepository.CreateUserAsync(user, dto.Password);
            if (!created)
                return false;
            await _userRepository.AddUserToRoleAsync(user, "Admin");
            return true;
        }


        public async Task<bool> RegisterAgentAsync(RegisterDTO dto)
        {
            if (await _userRepository.FindByEmailAsync(dto.Email) != null)
                return false;
            if (await _userRepository.FindByUserNameAsync(dto.UserName) != null)
                return false;
            var user = new User
            {
                Id = Guid.NewGuid(),
                UserName = dto.UserName,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                FullName = dto.FullName,
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
                IsEmailConfirmed = false
            };
            var created = await _userRepository.CreateUserAsync(user, dto.Password);
            if (!created)
                return false;
            await _userRepository.AddUserToRoleAsync(user, "Agent");
            return true;
        }


        public async Task<LoginResponseDTO> LoginAsync(LoginDTO dto, string ipAddress, string userAgent)
        {
            var response = new LoginResponseDTO();
            // Validate Client
            if (!await _userRepository.IsValidClientAsync(dto.ClientId))
            {
                response.ErrorMessage = "Invalid client ID.";
                return response;
            }
            // Get user by email or username
            var user = dto.EmailOrUserName.Contains("@")
                ? await _userRepository.FindByEmailAsync(dto.EmailOrUserName)
                : await _userRepository.FindByUserNameAsync(dto.EmailOrUserName);
            if (user == null)
            {
                response.ErrorMessage = "Invalid username or password.";
                return response;
            }
            // Check lockout info
            if (await _userRepository.IsLockedOutAsync(user))
            {
                var lockoutEnd = await _userRepository.GetLockoutEndDateAsync(user);
                if (lockoutEnd.HasValue && lockoutEnd > DateTime.UtcNow)
                {
                    var timeLeft = lockoutEnd.Value - DateTime.UtcNow;
                    response.ErrorMessage = $"Account is locked. Try again after {timeLeft.Minutes} minute(s) and {timeLeft.Seconds} second(s).";
                    response.RemainingAttempts = 0;
                    return response;
                }
                else
                {
                    await _userRepository.ResetAccessFailedCountAsync(user);
                }
            }
            if (!user.IsEmailConfirmed)
            {
                response.ErrorMessage = "Email not confirmed. Please verify your email.";
                return response;
            }
            // Validate password
            var passwordValid = await _userRepository.CheckPasswordAsync(user, dto.Password);
            if (!passwordValid)
            {
                await _userRepository.IncrementAccessFailedCountAsync(user);
                if (await _userRepository.IsLockedOutAsync(user))
                {
                    response.ErrorMessage = "Account locked due to multiple failed login attempts.";
                    response.RemainingAttempts = 0;
                    return response;
                }
                var maxAttempts = await _userRepository.GetMaxFailedAccessAttemptsAsync();
                var failedCount = await _userRepository.GetAccessFailedCountAsync(user);
                var attemptsLeft = maxAttempts - failedCount;
                response.ErrorMessage = "Invalid username or password.";
                response.RemainingAttempts = attemptsLeft > 0 ? attemptsLeft : 0;
                return response;
            }
            await _userRepository.ResetAccessFailedCountAsync(user);
            if (await _userRepository.IsTwoFactorEnabledAsync(user))
            {
                response.RequiresTwoFactor = true;
                return response;
            }
            await _userRepository.UpdateLastLoginAsync(user, DateTime.UtcNow);
            var roles = await _userRepository.GetUserRolesAsync(user);
            response.Token = GenerateJwtToken(user, roles, dto.ClientId);
            response.RefreshToken = await _userRepository.GenerateAndStoreRefreshTokenAsync(user.Id, dto.ClientId, userAgent, ipAddress);
            return response;
        }
        public async Task<EmailConfirmationTokenResponseDTO?> SendConfirmationEmailAsync(string email)
        {
            EmailConfirmationTokenResponseDTO? emailConfirmationTokenResponseDTO = null;
            var user = await _userRepository.FindByEmailAsync(email);
            if (user == null)
                return null;
            var token = await _userRepository.GenerateEmailConfirmationTokenAsync(user);
            if (token != null)
            {
                emailConfirmationTokenResponseDTO = new EmailConfirmationTokenResponseDTO()
                {
                    UserId = user.Id,
                    Token = token
                };
            }
            return emailConfirmationTokenResponseDTO;
        }
        public async Task<bool> VerifyConfirmationEmailAsync(ConfirmEmailDTO dto)
        {
            var user = await _userRepository.FindByIdAsync(dto.UserId);
            if (user == null)
                return false;
            var result = await _userRepository.VerifyConfirmaionEmailAsync(user, dto.Token);
            if (result)
            {
                user.IsActive = true;
                await _userRepository.UpdateUserAsync(user);
            }
            return result;
        }
        public async Task<RefreshTokenResponseDTO> RefreshTokenAsync(RefreshTokenRequestDTO dto, string ipAddress, string userAgent)
        {
            var response = new RefreshTokenResponseDTO();
            // Validate Client
            if (!await _userRepository.IsValidClientAsync(dto.ClientId))
            {
                response.ErrorMessage = "Invalid client ID.";
                return response;
            }
            var refreshTokenEntity = await _userRepository.GetRefreshTokenAsync(dto.RefreshToken);
            if (refreshTokenEntity == null || !refreshTokenEntity.IsActive)
            {
                response.ErrorMessage = "Invalid or expired refresh token.";
                return response;
            }
            // Revoke the old refresh token and generate a new one
            var newRefreshToken = await _userRepository.GenerateAndStoreRefreshTokenAsync(refreshTokenEntity.UserId, dto.ClientId, userAgent, ipAddress);
            var user = await _userRepository.FindByIdAsync(refreshTokenEntity.UserId);
            if (user == null)
            {
                response.ErrorMessage = "User not found.";
                return response;
            }
            var roles = await _userRepository.GetUserRolesAsync(user);
            response.Token = GenerateJwtToken(user, roles, dto.ClientId);
            response.RefreshToken = newRefreshToken;
            return response;
        }
        public async Task<bool> RevokeRefreshTokenAsync(string token, string ipAddress)
        {
            var refreshToken = await _userRepository.GetRefreshTokenAsync(token);
            if (refreshToken == null || !refreshToken.IsActive)
                return false;
            await _userRepository.RevokeRefreshTokenAsync(refreshToken, ipAddress);
            return true;
        }
        public async Task<ForgotPasswordResponseDTO?> ForgotPasswordAsync(string email)
        {
            ForgotPasswordResponseDTO? forgotPasswordResponseDTO = null;
            var user = await _userRepository.FindByEmailAsync(email);
            if (user == null)
                return null;
            var token = await _userRepository.GeneratePasswordResetTokenAsync(user);
            if (token != null)
            {
                forgotPasswordResponseDTO = new ForgotPasswordResponseDTO()
                {
                    UserId = user.Id,
                    Token = token
                };
            }
            return forgotPasswordResponseDTO;
        }
        public async Task<bool> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword)
        {
            var user = await _userRepository.FindByIdAsync(userId);
            if (user == null)
                return false;
            return await _userRepository.ChangePasswordAsync(user, currentPassword, newPassword);
        }
        public async Task<bool> ResetPasswordAsync(Guid userId, string token, string newPassword)
        {
            var user = await _userRepository.FindByIdAsync(userId);
            if (user == null) return false;
            return await _userRepository.ResetPasswordAsync(user, token, newPassword);
        }
        public async Task<ProfileDTO?> GetProfileAsync(Guid userId)
        {
            var user = await _userRepository.FindByIdAsync(userId);
            if (user == null) return null;
            return new ProfileDTO
            {
                UserId = user.Id,
                FullName = user.FullName,
                PhoneNumber = user.PhoneNumber,
                ProfilePhotoUrl = user.ProfilePhotoUrl,
                Email = user.Email,
                LastLoginAt = user.LastLoginAt,
                UserName = user.UserName
            };
        }
        public async Task<bool> UpdateProfileAsync(UpdateProfileDTO dto)
        {
            var user = await _userRepository.FindByIdAsync(dto.UserId);
            if (user == null)
                return false;
            user.FullName = dto.FullName;
            user.PhoneNumber = dto.PhoneNumber;
            user.ProfilePhotoUrl = dto.ProfilePhotoUrl;
            return await _userRepository.UpdateUserAsync(user);
        }
       
       
        public async Task<bool> IsUserExistsAsync(Guid userId)
        {
            return await _userRepository.IsUserExistsAsync(userId);
        }
      
        private string GenerateJwtToken(User user, IList<string> roles, string clientId)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email ?? ""),
                new Claim(ClaimTypes.Name, user.UserName ?? ""),
                new Claim("client_id", clientId)
            };
            // Add role claims
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
            // Read JWT settings from configuration
            var secretKey = _configuration["JwtSettings:SecretKey"];
            var issuer = _configuration["JwtSettings:Issuer"];
            var expiryMinutes = Convert.ToInt32(_configuration["JwtSettings:AccessTokenExpirationMinutes"]);
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var tokenDescriptor = new JwtSecurityToken(
                issuer: issuer,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
                signingCredentials: creds
            );
            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }
    }
}
