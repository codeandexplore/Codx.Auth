using System;
using System.ComponentModel.DataAnnotations;

namespace Codx.Auth.Data.Entities.AspNet
{
    /// <summary>
    /// Entity to store temporary 2FA verification codes
    /// </summary>
    public class TwoFactorCode
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid UserId { get; set; }

        [Required]
        [StringLength(10)]
        public string Code { get; set; } = string.Empty;

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        public DateTime ExpiresAt { get; set; }

        public bool IsUsed { get; set; } = false;

        public DateTime? UsedAt { get; set; }

        /// <summary>
        /// Navigation property to user
        /// </summary>
        public virtual ApplicationUser User { get; set; }

        /// <summary>
        /// Check if code is still valid (not expired and not used)
        /// </summary>
        public bool IsValid => !IsUsed && DateTime.UtcNow <= ExpiresAt;
    }
}