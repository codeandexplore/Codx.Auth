using System.Collections.Generic;

namespace Codx.Auth.Infrastructure.Lifecycle
{
    /// <summary>
    /// Result returned by <see cref="LifecycleTransitionGuard.Validate"/>.
    /// Callers return HTTP 409 when <see cref="IsValid"/> is false.
    /// </summary>
    public sealed class TransitionResult
    {
        public bool IsValid { get; private init; }
        public string Error { get; private init; }

        private TransitionResult() { }

        public static TransitionResult Ok() => new TransitionResult { IsValid = true };

        public static TransitionResult Fail(string error) =>
            new TransitionResult { IsValid = false, Error = error };
    }

    /// <summary>
    /// Validates that a requested status transition is permitted for a given entity category.
    /// Must be called before every Status write. Returns <see cref="TransitionResult"/>
    /// rather than throwing so callers can surface a clean 409 Conflict response.
    /// </summary>
    public class LifecycleTransitionGuard
    {
        // Keyed by (entityCategory, fromStatus) → set of allowed target statuses.
        private static readonly Dictionary<(string category, string from), HashSet<string>> _table =
            new Dictionary<(string, string), HashSet<string>>
            {
                // Tenant
                { ("Tenant", LifecycleStatus.Tenant.Active),    new HashSet<string> { LifecycleStatus.Tenant.Suspended, LifecycleStatus.Tenant.Cancelled } },
                { ("Tenant", LifecycleStatus.Tenant.Suspended), new HashSet<string> { LifecycleStatus.Tenant.Active,    LifecycleStatus.Tenant.Cancelled  } },
                { ("Tenant", LifecycleStatus.Tenant.Cancelled), new HashSet<string>() }, // terminal

                // Company
                { ("Company", LifecycleStatus.Company.Active),    new HashSet<string> { LifecycleStatus.Company.Suspended, LifecycleStatus.Company.Cancelled } },
                { ("Company", LifecycleStatus.Company.Suspended), new HashSet<string> { LifecycleStatus.Company.Active,    LifecycleStatus.Company.Cancelled  } },
                { ("Company", LifecycleStatus.Company.Cancelled), new HashSet<string>() }, // terminal

                // Membership
                { ("Membership", LifecycleStatus.Membership.Active),    new HashSet<string> { LifecycleStatus.Membership.Suspended, LifecycleStatus.Membership.Removed } },
                { ("Membership", LifecycleStatus.Membership.Suspended), new HashSet<string> { LifecycleStatus.Membership.Active,    LifecycleStatus.Membership.Removed  } },
                { ("Membership", LifecycleStatus.Membership.Removed),   new HashSet<string>() }, // terminal

                // MembershipRole
                { ("MembershipRole", LifecycleStatus.MembershipRole.Active),    new HashSet<string> { LifecycleStatus.MembershipRole.Suspended, LifecycleStatus.MembershipRole.Removed } },
                { ("MembershipRole", LifecycleStatus.MembershipRole.Suspended), new HashSet<string> { LifecycleStatus.MembershipRole.Active,    LifecycleStatus.MembershipRole.Removed  } },
                { ("MembershipRole", LifecycleStatus.MembershipRole.Removed),   new HashSet<string>() }, // terminal

                // Application
                { ("Application", LifecycleStatus.Application.Active),   new HashSet<string> { LifecycleStatus.Application.Inactive } },
                { ("Application", LifecycleStatus.Application.Inactive), new HashSet<string> { LifecycleStatus.Application.Active   } },

                // AppRole
                { ("AppRole", LifecycleStatus.AppRole.Active),   new HashSet<string> { LifecycleStatus.AppRole.Inactive } },
                { ("AppRole", LifecycleStatus.AppRole.Inactive), new HashSet<string> { LifecycleStatus.AppRole.Active   } },

                // RoleDefinition
                { ("RoleDefinition", LifecycleStatus.RoleDefinition.Active),   new HashSet<string> { LifecycleStatus.RoleDefinition.Inactive } },
                { ("RoleDefinition", LifecycleStatus.RoleDefinition.Inactive), new HashSet<string> { LifecycleStatus.RoleDefinition.Active   } },

                // RoleAssignment
                { ("RoleAssignment", LifecycleStatus.RoleAssignment.Active),  new HashSet<string> { LifecycleStatus.RoleAssignment.Revoked } },
                { ("RoleAssignment", LifecycleStatus.RoleAssignment.Revoked), new HashSet<string>() }, // terminal

                // User
                { ("User", LifecycleStatus.User.Active),      new HashSet<string> { LifecycleStatus.User.Suspended, LifecycleStatus.User.Deactivated } },
                { ("User", LifecycleStatus.User.Suspended),   new HashSet<string> { LifecycleStatus.User.Active,    LifecycleStatus.User.Deactivated  } },
                { ("User", LifecycleStatus.User.Deactivated), new HashSet<string>() }, // terminal

                // EmailTemplate
                { ("EmailTemplate", LifecycleStatus.EmailTemplate.Active),   new HashSet<string> { LifecycleStatus.EmailTemplate.Archived } },
                { ("EmailTemplate", LifecycleStatus.EmailTemplate.Archived), new HashSet<string> { LifecycleStatus.EmailTemplate.Active   } },
            };

        /// <summary>
        /// Validates that transitioning <paramref name="entityCategory"/> from
        /// <paramref name="currentStatus"/> to <paramref name="requestedStatus"/> is permitted.
        /// </summary>
        /// <param name="entityCategory">
        /// One of: Tenant, Company, Membership, MembershipRole, Application, AppRole,
        /// RoleDefinition, RoleAssignment, User, EmailTemplate.
        /// </param>
        public TransitionResult Validate(string entityCategory, string currentStatus, string requestedStatus)
        {
            var key = (entityCategory, currentStatus);

            if (!_table.TryGetValue(key, out var allowed))
            {
                return TransitionResult.Fail(
                    $"Unknown entity category '{entityCategory}' or unrecognised current status '{currentStatus}'.");
            }

            if (!allowed.Contains(requestedStatus))
            {
                return TransitionResult.Fail(
                    $"{entityCategory} cannot transition from '{currentStatus}' to '{requestedStatus}'.");
            }

            return TransitionResult.Ok();
        }
    }
}
