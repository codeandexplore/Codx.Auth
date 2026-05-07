namespace Codx.Auth.Infrastructure.Lifecycle
{
    /// <summary>
    /// Single source of truth for all lifecycle status values across every domain entity.
    /// No controller, service, migration, or query may use a raw string literal for a
    /// status value — always reference a constant from this class.
    /// </summary>
    public static class LifecycleStatus
    {
        public static class Tenant
        {
            public const string Active    = "Active";
            public const string Suspended = "Suspended";
            public const string Cancelled = "Cancelled";
        }

        public static class Company
        {
            public const string Active    = "Active";
            public const string Suspended = "Suspended";
            public const string Cancelled = "Cancelled";
        }

        public static class Membership
        {
            public const string Active    = "Active";
            public const string Suspended = "Suspended";
            public const string Removed   = "Removed";
        }

        public static class MembershipRole
        {
            public const string Active    = "Active";
            public const string Suspended = "Suspended";
            public const string Removed   = "Removed";
        }

        public static class Application
        {
            public const string Active   = "Active";
            public const string Inactive = "Inactive";
        }

        public static class AppRole
        {
            public const string Active   = "Active";
            public const string Inactive = "Inactive";
        }

        public static class RoleDefinition
        {
            public const string Active   = "Active";
            public const string Inactive = "Inactive";
        }

        public static class RoleAssignment
        {
            public const string Active  = "Active";
            public const string Revoked = "Revoked";
        }

        public static class User
        {
            public const string Active      = "Active";
            public const string Suspended   = "Suspended";
            public const string Deactivated = "Deactivated";
        }

        public static class EmailTemplate
        {
            public const string Active   = "Active";
            public const string Archived = "Archived";
        }
    }
}
