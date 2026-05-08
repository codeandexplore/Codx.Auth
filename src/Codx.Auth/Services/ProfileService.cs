using Codx.Auth.Data.Entities.AspNet;
using Codx.Auth.Models.Requests;
using Codx.Auth.Models.Responses;
using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Codx.Auth.Services
{
    public interface IUserProfileService
    {
        Task<ProfileResponse> GetProfileAsync(Guid userId);
        Task<(bool Success, string[] Errors, ProfileResponse Profile)> UpdateProfileAsync(Guid userId, UpdateProfileRequest request);
        Task<(bool Success, string[] Errors)> ChangePasswordAsync(Guid userId, ChangePasswordRequest request);
    }

    public class ProfileService : IUserProfileService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public ProfileService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<ProfileResponse> GetProfileAsync(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null) return null;

            return MapToResponse(user);
        }

        public async Task<(bool Success, string[] Errors, ProfileResponse Profile)> UpdateProfileAsync(
            Guid userId,
            UpdateProfileRequest request)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                return (false, new[] { "User not found." }, null);

            user.GivenName = request.FirstName;
            user.MiddleName = request.MiddleName;
            user.FamilyName = request.LastName;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToArray();
                return (false, errors, null);
            }

            return (true, Array.Empty<string>(), MapToResponse(user));
        }

        public async Task<(bool Success, string[] Errors)> ChangePasswordAsync(
            Guid userId,
            ChangePasswordRequest request)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                return (false, new[] { "User not found." });

            var result = await _userManager.ChangePasswordAsync(
                user,
                request.CurrentPassword,
                request.NewPassword);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToArray();
                return (false, errors);
            }

            return (true, Array.Empty<string>());
        }

        private static ProfileResponse MapToResponse(ApplicationUser user)
        {
            var displayParts = new[] { user.GivenName, user.FamilyName }
                .Where(p => !string.IsNullOrWhiteSpace(p));

            return new ProfileResponse
            {
                Sub = user.Id.ToString(),
                Email = user.Email,
                DisplayName = string.Join(" ", displayParts),
                FirstName = user.GivenName,
                MiddleName = user.MiddleName,
                LastName = user.FamilyName,
                PhoneNumber = user.PhoneNumber,
            };
        }
    }
}
