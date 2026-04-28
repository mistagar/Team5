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
using Microsoft.Extensions.Logging;
using Team5Hackathon.Application.Services;
using Team5Hackathon.Application.DTOs.Audit;

namespace Team5Hackathon.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        private readonly ILogger<UserService> _logger;
        private readonly IAuditService _auditService;
        public UserService(IUserRepository userRepository, IConfiguration configuration, ILogger<UserService> logger, IAuditService auditService)
        {
            _userRepository = userRepository;
            _configuration = configuration;
            _logger = logger;
            _auditService = auditService;
        }
        public async Task<bool> RegisterAsync(RegisterDTO dto)
        {
            if (await _userRepository.FindByEmailAsync(dto.Email) != null)
            {
                _logger.LogWarning("Registration failed: Email {Email} already exists.", dto.Email);
                return false;
            }
            if (await _userRepository.FindByUserNameAsync(dto.UserName) != null)
            {
                _logger.LogWarning("Registration failed: Username {UserName} already exists.", dto.UserName);
                return false;
            }
            var user = new User
            {
                Id = Guid.NewGuid(),
                UserName = dto.UserName,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                FullName = dto.FullName,
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
                IsEmailConfirmed = true
            };
            var created = await _userRepository.CreateUserAsync(user, dto.Password);
            if (!created)
            {
                _logger.LogError("Registration failed: Could not create user {UserName}.", dto.UserName);
                return false;
            }
            await _userRepository.AddUserToRoleAsync(user, "Customer");
            _logger.LogInformation("User {UserName} registered successfully.", dto.UserName);
            await _auditService.RecordAsync(new AuditEntry
            {
                Action = "User Registration",
                Outcome = "success",
                EntityName = "User",
                EntityId = user.Id.ToString(),
                Description = $"User {dto.UserName} registered successfully",
                PerformedBy = dto.UserName,
                PerformedById = user.Id.ToString()
            });
            return true;
        }



        public async Task<bool> RegisterCustomerAsync(RegisterDTO dto)
        {
            if (await _userRepository.FindByEmailAsync(dto.Email) != null)
            {
                _logger.LogWarning("Registration failed: Email {Email} already exists.", dto.Email);
                return false;
            }
            if (await _userRepository.FindByUserNameAsync(dto.UserName) != null)
            {
                _logger.LogWarning("Registration failed: Username {UserName} already exists.", dto.UserName);
                return false;
            }
            var user = new User
            {
                Id = Guid.NewGuid(),
                UserName = dto.UserName,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                FullName = dto.FullName,
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
                IsEmailConfirmed = true
            };
            var created = await _userRepository.CreateUserAsync(user, dto.Password);
            if (!created)
            {
                _logger.LogError("Registration failed: Could not create user {UserName}.", dto.UserName);
                return false;
            }
            await _userRepository.AddUserToRoleAsync(user, "Customer");
            _logger.LogInformation("User {UserName} registered as Customer successfully.", dto.UserName);
            await _auditService.RecordAsync(new AuditEntry
            {
                Action = "User Registration",
                Outcome = "success",
                EntityName = "User",
                EntityId = user.Id.ToString(),
                Description = $"User {dto.UserName} registered as Customer successfully",
                PerformedBy = dto.UserName,
                PerformedById = user.Id.ToString()
            });
            return true;
        }



        public async Task<bool> RegisterSupervisorAsync(RegisterDTO dto)
        {
            if (await _userRepository.FindByEmailAsync(dto.Email) != null)
            {
                _logger.LogWarning("Registration failed: Email {Email} already exists.", dto.Email);
                return false;
            }
            if (await _userRepository.FindByUserNameAsync(dto.UserName) != null)
            {
                _logger.LogWarning("Registration failed: Username {UserName} already exists.", dto.UserName);
                return false;
            }
            var user = new User
            {
                Id = Guid.NewGuid(),
                UserName = dto.UserName,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                FullName = dto.FullName,
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
                IsEmailConfirmed = true
            };
            var created = await _userRepository.CreateUserAsync(user, dto.Password);
            if (!created)
            {
                _logger.LogError("Registration failed: Could not create user {UserName}.", dto.UserName);
                return false;
            }
            await _userRepository.AddUserToRoleAsync(user, "Supervisor");
            _logger.LogInformation("User {UserName} registered as Supervisor successfully.", dto.UserName);
            await _auditService.RecordAsync(new AuditEntry
            {
                Action = "User Registration",
                Outcome = "success",
                EntityName = "User",
                EntityId = user.Id.ToString(),
                Description = $"User {dto.UserName} registered as Supervisor successfully",
                PerformedBy = dto.UserName,
                PerformedById = user.Id.ToString()
            });
            return true;
        }


        public async Task<bool> RegisterAdminAsync(RegisterDTO dto)
        {
            if (await _userRepository.FindByEmailAsync(dto.Email) != null)
            {
                _logger.LogWarning("Registration failed: Email {Email} already exists.", dto.Email);
                return false;
            }
            if (await _userRepository.FindByUserNameAsync(dto.UserName) != null)
            {
                _logger.LogWarning("Registration failed: Username {UserName} already exists.", dto.UserName);
                return false;
            }
            var user = new User
            {
                Id = Guid.NewGuid(),
                UserName = dto.UserName,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                FullName = dto.FullName,
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
                IsEmailConfirmed = true
            };
            var created = await _userRepository.CreateUserAsync(user, dto.Password);
            if (!created)
            {
                _logger.LogError("Registration failed: Could not create user {UserName}.", dto.UserName);
                return false;
            }
            await _userRepository.AddUserToRoleAsync(user, "Admin");
            _logger.LogInformation("User {UserName} registered as Admin successfully.", dto.UserName);
            await _auditService.RecordAsync(new AuditEntry
            {
                Action = "User Registration",
                Outcome = "success",
                EntityName = "User",
                EntityId = user.Id.ToString(),
                Description = $"User {dto.UserName} registered as Admin successfully",
                PerformedBy = dto.UserName,
                PerformedById = user.Id.ToString()
            });
            return true;
        }


        public async Task<bool> RegisterAgentAsync(RegisterDTO dto)
        {
            if (await _userRepository.FindByEmailAsync(dto.Email) != null)
            {
                _logger.LogWarning("Registration failed: Email {Email} already exists.", dto.Email);
                return false;
            }
            if (await _userRepository.FindByUserNameAsync(dto.UserName) != null)
            {
                _logger.LogWarning("Registration failed: Username {UserName} already exists.", dto.UserName);
                return false;
            }
            var user = new User
            {
                Id = Guid.NewGuid(),
                UserName = dto.UserName,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                FullName = dto.FullName,
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
                IsEmailConfirmed = true
            };
            var created = await _userRepository.CreateUserAsync(user, dto.Password);
            if (!created)
            {
                _logger.LogError("Registration failed: Could not create user {UserName}.", dto.UserName);
                return false;
            }
            await _userRepository.AddUserToRoleAsync(user, "Agent");
            _logger.LogInformation("User {UserName} registered as Agent successfully.", dto.UserName);
            await _auditService.RecordAsync(new AuditEntry
            {
                Action = "User Registration",
                Outcome = "success",
                EntityName = "User",
                EntityId = user.Id.ToString(),
                Description = $"User {dto.UserName} registered as Agent successfully",
                PerformedBy = dto.UserName,
                PerformedById = user.Id.ToString()
            });
            return true;
        }


        public async Task<LoginResponseDTO> LoginAsync(LoginDTO dto, string ipAddress, string userAgent)
        {
            var response = new LoginResponseDTO();
            // Validate Client
            if (!await _userRepository.IsValidClientAsync(dto.ClientId))
            {
                _logger.LogWarning("Login failed: Invalid client ID {ClientId} from IP {IPAddress}.", dto.ClientId, ipAddress);
                response.ErrorMessage = "Invalid client ID.";
                return response;
            }
            // Get user by email or username
            var user = dto.EmailOrUserName.Contains("@")
                ? await _userRepository.FindByEmailAsync(dto.EmailOrUserName)
                : await _userRepository.FindByUserNameAsync(dto.EmailOrUserName);
            if (user == null)
            {
                _logger.LogWarning("Login failed: User {EmailOrUserName} not found from IP {IPAddress}.", dto.EmailOrUserName, ipAddress);
                await _auditService.RecordAsync(new AuditEntry
                {
                    Action = "User Login",
                    Outcome = "failed",
                    Description = $"Login failed: User {dto.EmailOrUserName} not found",
                    IpAddress = ipAddress,
                    UserAgent = userAgent
                });
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
                    _logger.LogWarning("Login failed: Account {UserName} is locked out from IP {IPAddress}.", user.UserName, ipAddress);
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
                _logger.LogWarning("Login failed: Email not confirmed for {UserName} from IP {IPAddress}.", user.UserName, ipAddress);
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
                    _logger.LogWarning("Login failed: Account {UserName} locked due to failed attempts from IP {IPAddress}.", user.UserName, ipAddress);
                    response.ErrorMessage = "Account locked due to multiple failed login attempts.";
                    response.RemainingAttempts = 0;
                    return response;
                }
                var maxAttempts = await _userRepository.GetMaxFailedAccessAttemptsAsync();
                var failedCount = await _userRepository.GetAccessFailedCountAsync(user);
                var attemptsLeft = maxAttempts - failedCount;
                _logger.LogWarning("Login failed: Invalid password for {UserName} from IP {IPAddress}. Attempts left: {AttemptsLeft}.", user.UserName, ipAddress, attemptsLeft);
                await _auditService.RecordAsync(new AuditEntry
                {
                    Action = "User Login",
                    Outcome = "failed",
                    EntityName = "User",
                    EntityId = user.Id.ToString(),
                    Description = $"Login failed: Invalid password for {user.UserName}",
                    IpAddress = ipAddress,
                    UserAgent = userAgent,
                    PerformedBy = user.UserName,
                    PerformedById = user.Id.ToString()
                });
                response.ErrorMessage = "Invalid username or password.";
                response.RemainingAttempts = attemptsLeft > 0 ? attemptsLeft : 0;
                return response;
            }
            await _userRepository.ResetAccessFailedCountAsync(user);
            if (await _userRepository.IsTwoFactorEnabledAsync(user))
            {
                _logger.LogInformation("Login requires 2FA for {UserName} from IP {IPAddress}.", user.UserName, ipAddress);
                response.RequiresTwoFactor = true;
                return response;
            }
            await _userRepository.UpdateLastLoginAsync(user, DateTime.UtcNow);
            var roles = await _userRepository.GetUserRolesAsync(user);
            response.Token = GenerateJwtToken(user, roles, dto.ClientId);
            response.RefreshToken = await _userRepository.GenerateAndStoreRefreshTokenAsync(user.Id, dto.ClientId, userAgent, ipAddress);
            _logger.LogInformation("User {UserName} logged in successfully from IP {IPAddress}.", user.UserName, ipAddress);
            await _auditService.RecordAsync(new AuditEntry
            {
                Action = "User Login",
                Outcome = "success",
                EntityName = "User",
                EntityId = user.Id.ToString(),
                Description = $"User {user.UserName} logged in successfully",
                IpAddress = ipAddress,
                UserAgent = userAgent,
                PerformedBy = user.UserName,
                PerformedById = user.Id.ToString()
            });
            return response;
        }
        public async Task<EmailConfirmationTokenResponseDTO?> SendConfirmationEmailAsync(string email)
        {
            EmailConfirmationTokenResponseDTO? emailConfirmationTokenResponseDTO = null;
            var user = await _userRepository.FindByEmailAsync(email);
            if (user == null)
            {
                _logger.LogWarning("Send confirmation email failed: User with email {Email} not found.", email);
                return null;
            }
            var token = await _userRepository.GenerateEmailConfirmationTokenAsync(user);
            if (token != null)
            {
                emailConfirmationTokenResponseDTO = new EmailConfirmationTokenResponseDTO()
                {
                    UserId = user.Id,
                    Token = token
                };
                _logger.LogInformation("Confirmation email token generated for user {UserName}.", user.UserName);
            }
            else
            {
                _logger.LogError("Failed to generate confirmation token for user {UserName}.", user.UserName);
            }
            return emailConfirmationTokenResponseDTO;
        }
        public async Task<bool> VerifyConfirmationEmailAsync(ConfirmEmailDTO dto)
        {
            var user = await _userRepository.FindByIdAsync(dto.UserId);
            if (user == null)
            {
                _logger.LogWarning("Email verification failed: User {UserId} not found.", dto.UserId);
                return false;
            }
            var result = await _userRepository.VerifyConfirmaionEmailAsync(user, dto.Token);
            if (result)
            {
                user.IsActive = true;
                await _userRepository.UpdateUserAsync(user);
                _logger.LogInformation("Email verified successfully for user {UserName}.", user.UserName);
                await _auditService.RecordAsync(new AuditEntry
                {
                    Action = "Email Verification",
                    Outcome = "success",
                    EntityName = "User",
                    EntityId = user.Id.ToString(),
                    Description = $"Email verified for user {user.UserName}",
                    PerformedBy = user.UserName,
                    PerformedById = user.Id.ToString()
                });
            }
            else
            {
                _logger.LogWarning("Email verification failed: Invalid token for user {UserId}.", dto.UserId);
                await _auditService.RecordAsync(new AuditEntry
                {
                    Action = "Email Verification",
                    Outcome = "failed",
                    EntityName = "User",
                    EntityId = dto.UserId.ToString(),
                    Description = "Email verification failed: Invalid token"
                });
            }
            return result;
        }
        public async Task<RefreshTokenResponseDTO> RefreshTokenAsync(RefreshTokenRequestDTO dto, string ipAddress, string userAgent)
        {
            var response = new RefreshTokenResponseDTO();
            // Validate Client
            if (!await _userRepository.IsValidClientAsync(dto.ClientId))
            {
                _logger.LogWarning("Refresh token failed: Invalid client ID {ClientId} from IP {IPAddress}.", dto.ClientId, ipAddress);
                response.ErrorMessage = "Invalid client ID.";
                return response;
            }
            var refreshTokenEntity = await _userRepository.GetRefreshTokenAsync(dto.RefreshToken);
            if (refreshTokenEntity == null || !refreshTokenEntity.IsActive)
            {
                _logger.LogWarning("Refresh token failed: Invalid or expired token from IP {IPAddress}.", ipAddress);
                response.ErrorMessage = "Invalid or expired refresh token.";
                return response;
            }
            // Revoke the old refresh token and generate a new one
            var newRefreshToken = await _userRepository.GenerateAndStoreRefreshTokenAsync(refreshTokenEntity.UserId, dto.ClientId, userAgent, ipAddress);
            var user = await _userRepository.FindByIdAsync(refreshTokenEntity.UserId);
            if (user == null)
            {
                _logger.LogError("Refresh token failed: User {UserId} not found.", refreshTokenEntity.UserId);
                response.ErrorMessage = "User not found.";
                return response;
            }
            var roles = await _userRepository.GetUserRolesAsync(user);
            response.Token = GenerateJwtToken(user, roles, dto.ClientId);
            response.RefreshToken = newRefreshToken;
            _logger.LogInformation("Token refreshed successfully for user {UserName} from IP {IPAddress}.", user.UserName, ipAddress);
            await _auditService.RecordAsync(new AuditEntry
            {
                Action = "Token Refresh",
                Outcome = "success",
                EntityName = "User",
                EntityId = user.Id.ToString(),
                Description = $"Token refreshed for user {user.UserName}",
                IpAddress = ipAddress,
                UserAgent = userAgent,
                PerformedBy = user.UserName,
                PerformedById = user.Id.ToString()
            });
            return response;
        }
        public async Task<bool> RevokeRefreshTokenAsync(string token, string ipAddress)
        {
            var refreshToken = await _userRepository.GetRefreshTokenAsync(token);
            if (refreshToken == null || !refreshToken.IsActive)
            {
                _logger.LogWarning("Revoke token failed: Token not found or already inactive from IP {IPAddress}.", ipAddress);
                return false;
            }
            await _userRepository.RevokeRefreshTokenAsync(refreshToken, ipAddress);
            _logger.LogInformation("Token revoked successfully from IP {IPAddress}.", ipAddress);
            await _auditService.RecordAsync(new AuditEntry
            {
                Action = "Token Revocation",
                Outcome = "success",
                EntityName = "RefreshToken",
                EntityId = refreshToken.Id.ToString(),
                Description = "Refresh token revoked",
                IpAddress = ipAddress
            });
            return true;
        }
        public async Task<ForgotPasswordResponseDTO?> ForgotPasswordAsync(string email)
        {
            ForgotPasswordResponseDTO? forgotPasswordResponseDTO = null;
            var user = await _userRepository.FindByEmailAsync(email);
            if (user == null)
            {
                _logger.LogWarning("Forgot password failed: User with email {Email} not found.", email);
                return null;
            }
            var token = await _userRepository.GeneratePasswordResetTokenAsync(user);
            if (token != null)
            {
                forgotPasswordResponseDTO = new ForgotPasswordResponseDTO()
                {
                    UserId = user.Id,
                    Token = token
                };
                _logger.LogInformation("Password reset token generated for user {UserName}.", user.UserName);
            }
            else
            {
                _logger.LogError("Failed to generate password reset token for user {UserName}.", user.UserName);
            }
            return forgotPasswordResponseDTO;
        }
        public async Task<bool> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword)
        {
            var user = await _userRepository.FindByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("Change password failed: User {UserId} not found.", userId);
                return false;
            }
            var result = await _userRepository.ChangePasswordAsync(user, currentPassword, newPassword);
            if (result)
            {
                _logger.LogInformation("User {UserName} changed password successfully.", user.UserName);
                await _auditService.RecordAsync(new AuditEntry
                {
                    Action = "Password Change",
                    Outcome = "success",
                    EntityName = "User",
                    EntityId = user.Id.ToString(),
                    Description = $"User {user.UserName} changed password successfully",
                    PerformedBy = user.UserName,
                    PerformedById = user.Id.ToString()
                });
            }
            else
            {
                _logger.LogWarning("Change password failed: Invalid current password for {UserName}.", user.UserName);
                await _auditService.RecordAsync(new AuditEntry
                {
                    Action = "Password Change",
                    Outcome = "failed",
                    EntityName = "User",
                    EntityId = user.Id.ToString(),
                    Description = $"Change password failed: Invalid current password for {user.UserName}",
                    PerformedBy = user.UserName,
                    PerformedById = user.Id.ToString()
                });
            }
            return result;
        }
        public async Task<bool> ResetPasswordAsync(Guid userId, string token, string newPassword)
        {
            var user = await _userRepository.FindByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("Reset password failed: User {UserId} not found.", userId);
                return false;
            }
            var result = await _userRepository.ResetPasswordAsync(user, token, newPassword);
            if (result)
            {
                _logger.LogInformation("User {UserName} reset password successfully.", user.UserName);
                await _auditService.RecordAsync(new AuditEntry
                {
                    Action = "Password Reset",
                    Outcome = "success",
                    EntityName = "User",
                    EntityId = user.Id.ToString(),
                    Description = $"User {user.UserName} reset password successfully",
                    PerformedBy = user.UserName,
                    PerformedById = user.Id.ToString()
                });
            }
            else
            {
                _logger.LogWarning("Reset password failed: Invalid token for user {UserId}.", userId);
                await _auditService.RecordAsync(new AuditEntry
                {
                    Action = "Password Reset",
                    Outcome = "failed",
                    EntityName = "User",
                    EntityId = userId.ToString(),
                    Description = "Password reset failed: Invalid token"
                });
            }
            return result;
        }
        public async Task<ProfileDTO?> GetProfileAsync(Guid userId)
        {
            var user = await _userRepository.FindByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("Get profile failed: User {UserId} not found.", userId);
                return null;
            }
            _logger.LogInformation("Profile accessed for user {UserName}.", user.UserName);
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
            {
                _logger.LogWarning("Update profile failed: User {UserId} not found.", dto.UserId);
                return false;
            }
            user.FullName = dto.FullName;
            user.PhoneNumber = dto.PhoneNumber;
            user.ProfilePhotoUrl = dto.ProfilePhotoUrl;
            var result = await _userRepository.UpdateUserAsync(user);
            if (result)
            {
                _logger.LogInformation("Profile updated successfully for user {UserName}.", user.UserName);
                await _auditService.RecordAsync(new AuditEntry
                {
                    Action = "Profile Update",
                    Outcome = "success",
                    EntityName = "User",
                    EntityId = user.Id.ToString(),
                    Description = $"Profile updated for user {user.UserName}",
                    PerformedBy = user.UserName,
                    PerformedById = user.Id.ToString()
                });
            }
            else
            {
                _logger.LogError("Failed to update profile for user {UserName}.", user.UserName);
            }
            return result;
        }
       
       
        public async Task<bool> IsUserExistsAsync(Guid userId)
        {
            var exists = await _userRepository.IsUserExistsAsync(userId);
            _logger.LogInformation("User existence check for {UserId}: {Exists}.", userId, exists);
            return exists;
        }
        public async Task<UserRequestDTO> CreateUserRequestAsync(CreateUserRequestDTO dto)
        {
            var request = new UserRequest
            {
                Id = Guid.NewGuid(),
                UserId = dto.UserId,
                Type = dto.Type,
                Content = dto.Content,
                CreatedAt = DateTime.UtcNow,
                Status = "pending"
            };
            var created = await _userRepository.CreateUserRequestAsync(request);
            _logger.LogInformation("User request created for user {UserId}", dto.UserId);
            return new UserRequestDTO
            {
                Id = created.Id,
                UserId = created.UserId,
                Type = created.Type,
                Content = created.Content,
                CreatedAt = created.CreatedAt,
                Status = created.Status,
                Summary = created.Summary,
                Response = created.Response
            };
        }
        public async Task<UserRequestDTO?> GetUserRequestAsync(Guid requestId)
        {
            var request = await _userRepository.GetUserRequestByIdAsync(requestId);
            if (request == null) return null;
            return new UserRequestDTO
            {
                Id = request.Id,
                UserId = request.UserId,
                Type = request.Type,
                Content = request.Content,
                CreatedAt = request.CreatedAt,
                Status = request.Status,
                Summary = request.Summary,
                Response = request.Response
            };
        }
        public async Task<IEnumerable<UserRequestDTO>> GetUserRequestsAsync(Guid userId)
        {
            var requests = await _userRepository.GetUserRequestsByUserIdAsync(userId);
            return requests.Select(r => new UserRequestDTO
            {
                Id = r.Id,
                UserId = r.UserId,
                Type = r.Type,
                Content = r.Content,
                CreatedAt = r.CreatedAt,
                Status = r.Status,
                Summary = r.Summary,
                Response = r.Response
            });
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
