using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;
using Team5Hackathon.API.DTOs;
using Team5Hackathon.Application.DTOs.UserDTO;
using Team5Hackathon.Application.Services;
using UAParser;

namespace Team5Hackathon.API.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }



        [HttpPost("register")]
        [ProducesResponseType(typeof(ApiResponse<string>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResponse<string>), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Register([FromBody] RegisterDTO dto)
        {
            try
            {
                var result = await _userService.RegisterAsync(dto);
                if (!result)
                    return BadRequest(ApiResponse<string>.FailResponse("Registration failed. Email or username might already exist."));
                return Ok(ApiResponse<string>.SuccessResponse("User registered successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.FailResponse("Error during registration.", new List<string> { ex.Message }));
            }
        }


        [HttpPost("register-customer")]
        [ProducesResponseType(typeof(ApiResponse<string>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResponse<string>), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> RegisterCustomer([FromBody] RegisterDTO dto)
        {
            try
            {
                var result = await _userService.RegisterCustomerAsync(dto);
                if (!result)
                    return BadRequest(ApiResponse<string>.FailResponse("Registration failed. Email or username might already exist."));
                return Ok(ApiResponse<string>.SuccessResponse("User registered successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.FailResponse("Error during registration.", new List<string> { ex.Message }));
            }
        }

        [Authorize]
        [HttpPost("register-admin")]
        [ProducesResponseType(typeof(ApiResponse<string>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResponse<string>), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> RegisterAdmin([FromBody] RegisterDTO dto)
        {
            try
            {
                var result = await _userService.RegisterAdminAsync(dto);
                if (!result)
                    return BadRequest(ApiResponse<string>.FailResponse("Registration failed. Email or username might already exist."));
                return Ok(ApiResponse<string>.SuccessResponse("User registered successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.FailResponse("Error during registration.", new List<string> { ex.Message }));
            }
        }

        [Authorize]
        [HttpPost("register-supervisor")]
        [ProducesResponseType(typeof(ApiResponse<string>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResponse<string>), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> RegisterSupervisor([FromBody] RegisterDTO dto)
        {
            try
            {
                var result = await _userService.RegisterSupervisorAsync(dto);
                if (!result)
                    return BadRequest(ApiResponse<string>.FailResponse("Registration failed. Email or username might already exist."));
                return Ok(ApiResponse<string>.SuccessResponse("User registered successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.FailResponse("Error during registration.", new List<string> { ex.Message }));
            }
        }

        [HttpPost("register-agent")]
        [ProducesResponseType(typeof(ApiResponse<string>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResponse<string>), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> RegisterAgent([FromBody] RegisterDTO dto)
        {
            try
            {
                var result = await _userService.RegisterAgentAsync(dto);
                if (!result)
                    return BadRequest(ApiResponse<string>.FailResponse("Registration failed. Email or username might already exist."));
                return Ok(ApiResponse<string>.SuccessResponse("User registered successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.FailResponse("Error during registration.", new List<string> { ex.Message }));
            }
        }


        [HttpPost("send-confirmation-email")]
        [ProducesResponseType(typeof(ApiResponse<EmailConfirmationTokenResponseDTO>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> SendConfirmationEmail([FromBody] EmailDTO dto)
        {
            try
            {
                var emailTokenResponse = await _userService.SendConfirmationEmailAsync(dto.Email);
                if (emailTokenResponse == null)
                    return NotFound(ApiResponse<string>.FailResponse("User with this email not found"));
                return Ok(ApiResponse<EmailConfirmationTokenResponseDTO>.SuccessResponse(emailTokenResponse, "Email confirmation token generated successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.FailResponse("Error generating confirmation token.", new List<string> { ex.Message }));
            }
        }




        [HttpPost("verify-email")]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        [ProducesResponseType(typeof(ApiResponse<string>), 400)]
        public async Task<IActionResult> VerifyConfirmationEmailAsync([FromBody] ConfirmEmailDTO dto)
        {
            try
            {
                var success = await _userService.VerifyConfirmationEmailAsync(dto);
                if (!success)
                    return BadRequest(ApiResponse<string>.FailResponse("Invalid confirmation token or user."));
                return Ok(ApiResponse<string>.SuccessResponse("Email confirmed successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.FailResponse("Error confirming email.", new List<string> { ex.Message }));
            }
        }



        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO dto)
        {
            var IPAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "";
            var UserAgent = GetNormalizedUserAgent();
            var loginResponse = await _userService.LoginAsync(dto, IPAddress, UserAgent);
            // Always return LoginResponseDTO wrapped in ApiResponse
            if (!string.IsNullOrEmpty(loginResponse.ErrorMessage))
            {
                // Failure case - Success = false, return DTO with error message
                loginResponse.Succeeded = false; // Add this property if missing
                return Unauthorized(ApiResponse<LoginResponseDTO>.FailResponse(loginResponse.ErrorMessage, errors: null, data: loginResponse));
            }
            // Success or requires 2FA
            loginResponse.Succeeded = true; // Make sure this is set on success path as well
            return Ok(ApiResponse<LoginResponseDTO>.SuccessResponse(loginResponse,
                loginResponse.RequiresTwoFactor ? "Two-factor authentication required." : "Login successful."));
        }



        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDTO dto)
        {
            var IPAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "";
            var UserAgent = GetNormalizedUserAgent();

            var refreshTokenResponse = await _userService.RefreshTokenAsync(dto, IPAddress, UserAgent);
            if (!string.IsNullOrEmpty(refreshTokenResponse.ErrorMessage))
                return Unauthorized(ApiResponse<string>.FailResponse(refreshTokenResponse.ErrorMessage));
            return Ok(ApiResponse<RefreshTokenResponseDTO>.SuccessResponse(refreshTokenResponse, "Token refreshed successfully."));
        }



        [HttpPost("revoke-token")]
        [ProducesResponseType(typeof(ApiResponse<string>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResponse<string>), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> RevokeToken([FromBody] RefreshTokenRequestDTO dto)
        {
            try
            {
                var success = await _userService.RevokeRefreshTokenAsync(dto.RefreshToken, HttpContext.Connection.RemoteIpAddress?.ToString() ?? "");
                if (!success)
                    return BadRequest(ApiResponse<string>.FailResponse("Invalid token or token already revoked."));
                return Ok(ApiResponse<string>.SuccessResponse("Token revoked successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.FailResponse("Error revoking token.", new List<string> { ex.Message }));
            }
        }




        [HttpGet("profile/{userId}")]
        [ProducesResponseType(typeof(ApiResponse<ProfileDTO>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResponse<string>), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetProfile(Guid userId)
        {
            try
            {
                var profile = await _userService.GetProfileAsync(userId);
                if (profile == null)
                    return NotFound(ApiResponse<string>.FailResponse("User profile not found."));
                return Ok(ApiResponse<ProfileDTO>.SuccessResponse(profile));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.FailResponse("Error fetching profile.", new List<string> { ex.Message }));
            }
        }



        [HttpPut("profile")]
        [ProducesResponseType(typeof(ApiResponse<string>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResponse<string>), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDTO dto)
        {
            try
            {
                var success = await _userService.UpdateProfileAsync(dto);
                if (!success)
                    return BadRequest(ApiResponse<string>.FailResponse("Failed to update profile."));
                return Ok(ApiResponse<string>.SuccessResponse("Profile updated successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.FailResponse("Error updating profile.", new List<string> { ex.Message }));
            }
        }


        [HttpPost("forgot-password")]
        [ProducesResponseType(typeof(ApiResponse<string>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResponse<string>), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> ForgotPassword([FromBody] EmailDTO dto)
        {
            try
            {
                var forgotPassword = await _userService.ForgotPasswordAsync(dto.Email);
                if (forgotPassword == null)
                    return NotFound(ApiResponse<string>.FailResponse("Email not found."));
                return Ok(ApiResponse<ForgotPasswordResponseDTO>.SuccessResponse(forgotPassword, "Password reset token sent to email."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.FailResponse("Error in forgot password process.", new List<string> { ex.Message }));
            }
        }

        // Reset Password (Forgot Password Flow)
        [HttpPost("reset-password")]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        [ProducesResponseType(typeof(ApiResponse<string>), 400)]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDTO dto)
        {
            try
            {
                var success = await _userService.ResetPasswordAsync(dto.UserId, dto.Token, dto.NewPassword);
                if (!success)
                    return BadRequest(ApiResponse<string>.FailResponse("Invalid token or user."));
                return Ok(ApiResponse<string>.SuccessResponse("Password reset successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.FailResponse("Error resetting password.", new List<string> { ex.Message }));
            }
        }


        [Authorize]
        [HttpPost("change-password")]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        [ProducesResponseType(typeof(ApiResponse<string>), 400)]
        [ProducesResponseType(typeof(ApiResponse<string>), 401)]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDTO dto)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
                    return Unauthorized(ApiResponse<string>.FailResponse("Invalid user token."));
                var success = await _userService.ChangePasswordAsync(userId, dto.CurrentPassword, dto.NewPassword);
                if (!success)
                    return BadRequest(ApiResponse<string>.FailResponse("Password change failed."));
                return Ok(ApiResponse<string>.SuccessResponse("Password changed successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.FailResponse("Error changing password.", new List<string> { ex.Message }));
            }
        }
       
        
       
        [HttpGet("{userId}/exists")]
        public async Task<IActionResult> UserExists(Guid userId)
        {
            bool exists = await _userService.IsUserExistsAsync(userId);
            var response = new ApiResponse<bool>
            {
                Success = true,
                Data = exists,
                Message = exists ? "User exists." : "User does not exist."
            };
            return Ok(response);
        }
        [HttpPost("request")]
        [ProducesResponseType(typeof(ApiResponse<UserRequestDTO>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> CreateUserRequest([FromBody] CreateUserRequestDTO dto)
        {
            try
            {
                var request = await _userService.CreateUserRequestAsync(dto);
                return Ok(ApiResponse<UserRequestDTO>.SuccessResponse(request, "Request created successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.FailResponse("Error creating request", new List<string> { ex.Message }));
            }
        }
        [HttpGet("request/{requestId}")]
        [ProducesResponseType(typeof(ApiResponse<UserRequestDTO>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetUserRequest(Guid requestId)
        {
            try
            {
                var request = await _userService.GetUserRequestAsync(requestId);
                if (request == null) return NotFound(ApiResponse<string>.FailResponse("Request not found"));
                return Ok(ApiResponse<UserRequestDTO>.SuccessResponse(request));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.FailResponse("Error fetching request", new List<string> { ex.Message }));
            }
        }
        [HttpGet("{userId}/requests")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<UserRequestDTO>>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetUserRequests(Guid userId)
        {
            try
            {
                var requests = await _userService.GetUserRequestsAsync(userId);
                return Ok(ApiResponse<IEnumerable<UserRequestDTO>>.SuccessResponse(requests));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.FailResponse("Error fetching requests", new List<string> { ex.Message }));
            }
        }
        private string GetNormalizedUserAgent()
        {
            var userAgentRaw = HttpContext.Request.Headers["User-Agent"].ToString();
            if (string.IsNullOrWhiteSpace(userAgentRaw))
                return "Unknown";
            try
            {
                var uaParser = Parser.GetDefault();
                ClientInfo clientInfo = uaParser.Parse(userAgentRaw);
                var browser = clientInfo.UA.Family ?? "UnknownBrowser";
                var browserVersion = clientInfo.UA.Major ?? "0";
                var os = clientInfo.OS.Family ?? "UnknownOS";
                return $"{browser}-{browserVersion}_{os}";
            }
            catch
            {
                // In case parsing fails, fallback to raw user agent or unknown
                return "Unknown";
            }
        }
    }

}
