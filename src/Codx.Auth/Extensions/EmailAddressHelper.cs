using Codx.Auth.Data.Entities.AspNet;
using System;

namespace Codx.Auth.Extensions
{
    public static class EmailAddressHelper
    {
        public static (string Username, string Domain) GetEmailParts(string email)
        {
            if (string.IsNullOrWhiteSpace(email) || !email.Contains("@"))
            {
                throw new ArgumentException("Invalid email address", nameof(email));
            }

            var parts = email.Split('@');
            return (parts[0], parts[1]);
        }

        public static string GetEmailUsername(string email)
        {
            if (string.IsNullOrWhiteSpace(email) || !email.Contains("@"))
            {
                throw new ArgumentException("Invalid email address", nameof(email));
            }
            var parts = email.Split('@');
            return parts[0];
        }

        public static string GetEmailDomain(string email)
        {
            if (string.IsNullOrWhiteSpace(email) || !email.Contains("@"))
            {
                throw new ArgumentException("Invalid email address", nameof(email));
            }
            var parts = email.Split('@');
            return parts[1];
        }
    }

    /// <summary>
    /// Extension methods for ApplicationUser
    /// </summary>
    public static class ApplicationUserExtensions
    {
        /// <summary>
        /// Get display name for user (email username or full email if no username available)
        /// </summary>
        /// <param name="user">ApplicationUser instance</param>
        /// <returns>Display name for user</returns>
        public static string GetDisplayName(this ApplicationUser user)
        {
            if (user == null)
                return "Unknown User";

            if (!string.IsNullOrEmpty(user.Email))
            {
                try
                {
                    return EmailAddressHelper.GetEmailUsername(user.Email);
                }
                catch
                {
                    return user.Email;
                }
            }

            return user.UserName ?? "Unknown User";
        }
    }
}
