using System;
using System.Collections.Generic;

namespace Codx.Auth.Models.WorkspaceUsers
{
    /// <summary>
    /// Discriminated union result for AddOrInviteUserAsync.
    /// Use the static factory methods; check the Outcome to determine which path was taken.
    /// </summary>
    public sealed class AddWorkspaceUserResult
    {
        public enum ResultOutcome
        {
            Added,
            Invited,
            Conflict,
            Forbidden,
            BadRequest
        }

        public ResultOutcome Outcome { get; private init; }

        /// <summary>Populated on Added or Invited outcome.</summary>
        public AddWorkspaceUserResponse? Response { get; private init; }

        /// <summary>Error code on Conflict: USER_ALREADY_MEMBER or INVITATION_ALREADY_PENDING.</summary>
        public string? ErrorCode { get; private init; }

        /// <summary>Human-readable detail on Forbidden or BadRequest.</summary>
        public string? Detail { get; private init; }

        public static AddWorkspaceUserResult Added(AddWorkspaceUserResponse response) =>
            new() { Outcome = ResultOutcome.Added, Response = response };

        public static AddWorkspaceUserResult Invited(AddWorkspaceUserResponse response) =>
            new() { Outcome = ResultOutcome.Invited, Response = response };

        public static AddWorkspaceUserResult Conflict(string errorCode) =>
            new() { Outcome = ResultOutcome.Conflict, ErrorCode = errorCode };

        public static AddWorkspaceUserResult Forbidden(string detail) =>
            new() { Outcome = ResultOutcome.Forbidden, Detail = detail };

        public static AddWorkspaceUserResult BadRequest(string detail) =>
            new() { Outcome = ResultOutcome.BadRequest, Detail = detail };
    }
}
