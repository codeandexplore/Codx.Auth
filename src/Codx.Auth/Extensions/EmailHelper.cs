using System;

namespace Codx.Auth.Extensions
{
    public static class EmailHelper
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
}
