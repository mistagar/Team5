using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Team5Hackathon.Domain.Entities;
using Team5Hackathon.Domain.RepositoriesContract;
using Team5Hackathon.Infrastructure.Identity;
using Team5Hackathon.Infrastructure.Persistence;

namespace Team5Hackathon.Infrastructure.Repositories
{
    
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AppDbContext _dbContext;
        public UserRepository(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            AppDbContext dbContext)
        {
            _userManager = userManager;
            _dbContext = dbContext;
        }
        private User MapToDomain(ApplicationUser appUser)
        {
            if (appUser == null) return null!;
            return new User
            {
                Id = appUser.Id,
                UserName = appUser.UserName,
                Email = appUser.Email,
                FullName = appUser.FullName,
                PhoneNumber = appUser.PhoneNumber,
                ProfilePhotoUrl = appUser.ProfilePhotoUrl,
                IsActive = appUser.IsActive,
                CreatedAt = appUser.CreatedAt,
                LastLoginAt = appUser.LastLoginAt,
                IsEmailConfirmed = appUser.EmailConfirmed
            };
        }
        private ApplicationUser MapToApplicationUser(User user)
        {
            return new ApplicationUser
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                FullName = user.FullName,
                PhoneNumber = user.PhoneNumber,
                ProfilePhotoUrl = user.ProfilePhotoUrl,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt,
                LastLoginAt = user.LastLoginAt,
                EmailConfirmed = user.IsEmailConfirmed
            };
        }
        public async Task<User?> FindByEmailAsync(string email)
        {
            var appUser = await _userManager.FindByEmailAsync(email);
            if (appUser == null)
                return null;
            return MapToDomain(appUser);
        }
        public async Task<User?> FindByUserNameAsync(string userName)
        {
            var appUser = await _userManager.FindByNameAsync(userName);
            if (appUser == null)
                return null;
            return MapToDomain(appUser);
        }
        public async Task<User?> FindByIdAsync(Guid id)
        {
            var appUser = await _userManager.FindByIdAsync(id.ToString());
            if (appUser == null)
                return null;
            return MapToDomain(appUser);
        }
        public async Task<bool> CreateUserAsync(User user, string password)
        {
            var appUser = MapToApplicationUser(user);
            var result = await _userManager.CreateAsync(appUser, password);
            return result.Succeeded;
        }
        public async Task<bool> CheckPasswordAsync(User user, string password)
        {
            var appUser = await _userManager.FindByIdAsync(user.Id.ToString());
            if (appUser == null)
                return false;
            return await _userManager.CheckPasswordAsync(appUser, password);
        }
        public async Task<bool> UpdateUserAsync(User user)
        {
            var appUser = await _userManager.FindByIdAsync(user.Id.ToString());
            if (appUser == null)
                return false;
            appUser.UserName = user.UserName;
            appUser.Email = user.Email;
            appUser.FullName = user.FullName;
            appUser.PhoneNumber = user.PhoneNumber;
            appUser.ProfilePhotoUrl = user.ProfilePhotoUrl;
            var result = await _userManager.UpdateAsync(appUser);
            return result.Succeeded;
        }
        public async Task<IList<string>> GetUserRolesAsync(User user)
        {
            var appUser = await _userManager.FindByIdAsync(user.Id.ToString());
            if (appUser == null)
                return new List<string>();
            return await _userManager.GetRolesAsync(appUser);
        }
        public async Task<bool> AddUserToRoleAsync(User user, string role)
        {
            var appUser = await _userManager.FindByIdAsync(user.Id.ToString());
            if (appUser == null)
                return false;
            var result = await _userManager.AddToRoleAsync(appUser, role);
            return result.Succeeded;
        }
        public async Task<string?> GenerateEmailConfirmationTokenAsync(User user)
        {
            var appUser = await _userManager.FindByIdAsync(user.Id.ToString());
            if (appUser == null)
                return null;
            return await _userManager.GenerateEmailConfirmationTokenAsync(appUser);
        }
        public async Task<bool> VerifyConfirmaionEmailAsync(User user, string token)
        {
            var appUser = await _userManager.FindByIdAsync(user.Id.ToString());
            if (appUser == null)
                return false;
            var result = await _userManager.ConfirmEmailAsync(appUser, token);
            return result.Succeeded;
        }
        public async Task<string?> GeneratePasswordResetTokenAsync(User user)
        {
            var appUser = await _userManager.FindByIdAsync(user.Id.ToString());
            if (appUser == null)
                return null;
            var token = await _userManager.GeneratePasswordResetTokenAsync(appUser);
            return token;
        }
        public async Task<bool> ResetPasswordAsync(User user, string token, string newPassword)
        {
            var appUser = await _userManager.FindByIdAsync(user.Id.ToString());
            if (appUser == null)
                return false;
            var result = await _userManager.ResetPasswordAsync(appUser, token, newPassword);
            return result.Succeeded;
        }
        public async Task<bool> ChangePasswordAsync(User user, string currentPassword, string newPassword)
        {
            var appUser = await _userManager.FindByIdAsync(user.Id.ToString());
            if (appUser == null)
                return false;
            var result = await _userManager.ChangePasswordAsync(appUser, currentPassword, newPassword);
            return result.Succeeded;
        }
        public async Task UpdateLastLoginAsync(User user, DateTime loginTime)
        {
            var appUser = await _userManager.FindByIdAsync(user.Id.ToString());
            if (appUser == null) return;
            appUser.LastLoginAt = loginTime;
            await _userManager.UpdateAsync(appUser);
        }
        public async Task<string> GenerateAndStoreRefreshTokenAsync(Guid userId, string clientId, string userAgent, string ipAddress)
        {
            
            await RevokeAllRefreshTokensAsync(userId, clientId, userAgent, ipAddress);
            var refreshToken = new RefreshToken
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                ClientId = clientId,
                UserAgent = userAgent,
                Token = Convert.ToBase64String(Guid.NewGuid().ToByteArray()),
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                CreatedByIp = ipAddress
            };
            _dbContext.RefreshTokens.Add(refreshToken);
            await _dbContext.SaveChangesAsync();
            return refreshToken.Token;
        }
        public async Task<RefreshToken?> GetRefreshTokenAsync(string token)
        {
            return await _dbContext.RefreshTokens.FirstOrDefaultAsync(rt => rt.Token == token);
        }
        public async Task RevokeRefreshTokenAsync(RefreshToken refreshToken, string ipAddress)
        {
            refreshToken.RevokedAt = DateTime.UtcNow;
            refreshToken.RevokedByIp = ipAddress;
            await _dbContext.SaveChangesAsync();
        }
       
        
        
        public async Task<bool> IsLockedOutAsync(User user)
        {
            var appUser = await _userManager.FindByIdAsync(user.Id.ToString());
            return appUser != null && await _userManager.IsLockedOutAsync(appUser);
        }
        public async Task<bool> IsTwoFactorEnabledAsync(User user)
        {
            var appUser = await _userManager.FindByIdAsync(user.Id.ToString());
            return appUser != null && await _userManager.GetTwoFactorEnabledAsync(appUser);
        }
        public async Task IncrementAccessFailedCountAsync(User user)
        {
            var appUser = await _userManager.FindByIdAsync(user.Id.ToString());
            if (appUser != null)
                await _userManager.AccessFailedAsync(appUser);
        }
        public async Task ResetAccessFailedCountAsync(User user)
        {
            var appUser = await _userManager.FindByIdAsync(user.Id.ToString());
            if (appUser != null)
                await _userManager.ResetAccessFailedCountAsync(appUser);
        }
        public async Task<DateTime?> GetLockoutEndDateAsync(User user)
        {
            var appUser = await _userManager.FindByIdAsync(user.Id.ToString());
            if (appUser == null)
                return null;
            
            return appUser.LockoutEnd?.UtcDateTime;
        }
        public Task<int> GetMaxFailedAccessAttemptsAsync()
        {
            return Task.FromResult(_userManager.Options.Lockout.MaxFailedAccessAttempts);
        }
        public async Task<int> GetAccessFailedCountAsync(User user)
        {
            var appUser = await _userManager.FindByIdAsync(user.Id.ToString());
            return appUser?.AccessFailedCount ?? 0;
        }
        public async Task<bool> IsValidClientAsync(string clientId)
        {
            return await _dbContext.Clients.AnyAsync(c => c.ClientId == clientId);
        }
        private async Task RevokeAllRefreshTokensAsync(Guid userId, string clientId, string userAgent, string revokedByIp)
        {
            var tokens = await _dbContext.RefreshTokens
                .Where(t => t.UserId == userId
                    && t.ClientId == clientId
                    && t.UserAgent == userAgent
                    && t.RevokedAt == null)
                .ToListAsync();
            foreach (var token in tokens)
            {
                token.RevokedAt = DateTime.UtcNow;
                token.RevokedByIp = revokedByIp;
            }
            await _dbContext.SaveChangesAsync();
        }
        public async Task<bool> IsUserExistsAsync(Guid userId)
        {
            return await _dbContext.Users.AsNoTracking().AnyAsync(u => u.Id == userId);
        }
        public async Task<UserRequest> CreateUserRequestAsync(UserRequest request)
        {
            _dbContext.UserRequests.Add(request);
            await _dbContext.SaveChangesAsync();
            return request;
        }
        public async Task<UserRequest?> GetUserRequestByIdAsync(Guid requestId)
        {
            return await _dbContext.UserRequests.FindAsync(requestId);
        }
        public async Task<IEnumerable<UserRequest>> GetUserRequestsByUserIdAsync(Guid userId)
        {
            return await _dbContext.UserRequests.Where(ur => ur.UserId == userId).ToListAsync();
        }
        public async Task<bool> UpdateUserRequestAsync(UserRequest request)
        {
            _dbContext.UserRequests.Update(request);
            return await _dbContext.SaveChangesAsync() > 0;
        }
    }
}
