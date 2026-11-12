using System.Collections.Generic;
using Codx.Auth.Data.Entities.AspNet;
using Codx.Auth.Data.Entities.Enterprise;
using Codx.Auth.ViewModels;
using Duende.IdentityServer.EntityFramework.Entities;
using Riok.Mapperly.Abstractions;

namespace Codx.Auth.Mappings
{
    /// <summary>
    /// Compile-time object mapper (Riok.Mapperly). Replaces the former AutoMapper
    /// <c>ApplicationProfile</c>.
    /// </summary>
    [Mapper(RequiredMappingStrategy = RequiredMappingStrategy.None)]
    public partial class ApplicationMapper
    {
        // ── IdentityServer: Clients ──────────────────────────────────────────
        [MapperIgnoreTarget(nameof(ClientDetailsViewModel.AllowSelfRegistration))]
        public partial ClientDetailsViewModel MapToClientDetails(Client source);

        [MapperIgnoreTarget(nameof(ClientEditViewModel.ApplicationId))]
        [MapperIgnoreTarget(nameof(ClientEditViewModel.AllowSelfRegistration))]
        public partial ClientEditViewModel MapToClientEdit(Client source);

        public partial Client MapToClient(ClientEditViewModel source);

        // ── IdentityServer: Client child entities ────────────────────────────
        public partial ClientClaimDetailsViewModel MapToClientClaimDetails(ClientClaim source);
        public partial ClientClaimEditViewModel MapToClientClaimEdit(ClientClaim source);
        public partial ClientClaim MapToClientClaim(ClientClaimAddViewModel source);
        public partial ClientClaim MapToClientClaim(ClientClaimEditViewModel source);

        public partial ClientGrantTypeDetailsViewModel MapToClientGrantTypeDetails(ClientGrantType source);
        public partial ClientGrantTypeEditViewModel MapToClientGrantTypeEdit(ClientGrantType source);
        public partial ClientGrantType MapToClientGrantType(ClientGrantTypeAddViewModel source);
        public partial ClientGrantType MapToClientGrantType(ClientGrantTypeEditViewModel source);

        public partial ClientScopeDetailsViewModel MapToClientScopeDetails(ClientScope source);
        public partial ClientScopeEditViewModel MapToClientScopeEdit(ClientScope source);
        public partial ClientScope MapToClientScope(ClientScopeAddViewModel source);
        public partial ClientScope MapToClientScope(ClientScopeEditViewModel source);

        public partial ClientCorsOriginDetailsViewModel MapToClientCorsOriginDetails(ClientCorsOrigin source);
        public partial ClientCorsOriginEditViewModel MapToClientCorsOriginEdit(ClientCorsOrigin source);
        public partial ClientCorsOrigin MapToClientCorsOrigin(ClientCorsOriginAddViewModel source);
        public partial ClientCorsOrigin MapToClientCorsOrigin(ClientCorsOriginEditViewModel source);

        public partial ClientSecretDetailsViewModel MapToClientSecretDetails(ClientSecret source);
        public partial ClientSecretEditViewModel MapToClientSecretEdit(ClientSecret source);
        public partial ClientSecret MapToClientSecret(ClientSecretAddViewModel source);
        public partial ClientSecret MapToClientSecret(ClientSecretEditViewModel source);

        public partial ClientIdpRestrictionDetailsViewModel MapToClientIdpRestrictionDetails(ClientIdPRestriction source);
        public partial ClientIdpRestrictionEditViewModel MapToClientIdpRestrictionEdit(ClientIdPRestriction source);
        public partial ClientIdPRestriction MapToClientIdPRestriction(ClientIdpRestrictionAddViewModel source);
        public partial ClientIdPRestriction MapToClientIdPRestriction(ClientIdpRestrictionEditViewModel source);

        public partial ClientRedirectUriDetailsViewModel MapToClientRedirectUriDetails(ClientRedirectUri source);
        public partial ClientRedirectUriEditViewModel MapToClientRedirectUriEdit(ClientRedirectUri source);
        public partial ClientRedirectUri MapToClientRedirectUri(ClientRedirectUriAddViewModel source);
        public partial ClientRedirectUri MapToClientRedirectUri(ClientRedirectUriEditViewModel source);

        public partial ClientPostLogoutRedirectUriDetailsViewModel MapToClientPostLogoutRedirectUriDetails(ClientPostLogoutRedirectUri source);
        public partial ClientPostLogoutRedirectUriEditViewModel MapToClientPostLogoutRedirectUriEdit(ClientPostLogoutRedirectUri source);
        public partial ClientPostLogoutRedirectUri MapToClientPostLogoutRedirectUri(ClientPostLogoutRedirectUriAddViewModel source);
        public partial ClientPostLogoutRedirectUri MapToClientPostLogoutRedirectUri(ClientPostLogoutRedirectUriEditViewModel source);

        public partial ClientPropertyDetailsViewModel MapToClientPropertyDetails(ClientProperty source);
        public partial ClientPropertyEditViewModel MapToClientPropertyEdit(ClientProperty source);
        public partial ClientProperty MapToClientProperty(ClientPropertyAddViewModel source);
        public partial ClientProperty MapToClientProperty(ClientPropertyEditViewModel source);

        // ── IdentityServer: ApiResources ─────────────────────────────────────
        public partial ApiResourceDetailsViewModel MapToApiResourceDetails(ApiResource source);
        public partial ApiResourceEditViewModel MapToApiResourceEdit(ApiResource source);
        public partial ApiResource MapToApiResource(ApiResourceAddViewModel source);
        public partial ApiResource MapToApiResource(ApiResourceEditViewModel source);

        public partial ApiResourceClaimDetailsViewModel MapToApiResourceClaimDetails(ApiResourceClaim source);
        public partial ApiResourceClaimEditViewModel MapToApiResourceClaimEdit(ApiResourceClaim source);
        public partial ApiResourceClaim MapToApiResourceClaim(ApiResourceClaimAddViewModel source);
        public partial ApiResourceClaim MapToApiResourceClaim(ApiResourceClaimEditViewModel source);

        public partial ApiResourceScopeDetailsViewModel MapToApiResourceScopeDetails(ApiResourceScope source);
        public partial ApiResourceScopeEditViewModel MapToApiResourceScopeEdit(ApiResourceScope source);
        public partial ApiResourceScope MapToApiResourceScope(ApiResourceScopeAddViewModel source);
        public partial ApiResourceScope MapToApiResourceScope(ApiResourceScopeEditViewModel source);

        public partial ApiResourceSecretDetailsViewModel MapToApiResourceSecretDetails(ApiResourceSecret source);
        public partial ApiResourceSecretEditViewModel MapToApiResourceSecretEdit(ApiResourceSecret source);
        public partial ApiResourceSecret MapToApiResourceSecret(ApiResourceSecretAddViewModel source);
        public partial ApiResourceSecret MapToApiResourceSecret(ApiResourceSecretEditViewModel source);

        public partial ApiResourcePropertyDetailsViewModel MapToApiResourcePropertyDetails(ApiResourceProperty source);
        public partial ApiResourcePropertyEditViewModel MapToApiResourcePropertyEdit(ApiResourceProperty source);
        public partial ApiResourceProperty MapToApiResourceProperty(ApiResourcePropertyAddViewModel source);
        public partial ApiResourceProperty MapToApiResourceProperty(ApiResourcePropertyEditViewModel source);

        // ── IdentityServer: ApiScopes ────────────────────────────────────────
        public partial ApiScopeDetailsViewModel MapToApiScopeDetails(ApiScope source);
        public partial ApiScopeEditViewModel MapToApiScopeEdit(ApiScope source);
        public partial ApiScope MapToApiScope(ApiScopeAddViewModel source);
        public partial ApiScope MapToApiScope(ApiScopeEditViewModel source);

        public partial ApiScopeClaimDetailsViewModel MapToApiScopeClaimDetails(ApiScopeClaim source);
        public partial ApiScopeClaimEditViewModel MapToApiScopeClaimEdit(ApiScopeClaim source);
        public partial ApiScopeClaim MapToApiScopeClaim(ApiScopeClaimAddViewModel source);
        public partial ApiScopeClaim MapToApiScopeClaim(ApiScopeClaimEditViewModel source);

        public partial ApiScopePropertyDetailsViewModel MapToApiScopePropertyDetails(ApiScopeProperty source);
        public partial ApiScopePropertyEditViewModel MapToApiScopePropertyEdit(ApiScopeProperty source);
        public partial ApiScopeProperty MapToApiScopeProperty(ApiScopePropertyAddViewModel source);
        public partial ApiScopeProperty MapToApiScopeProperty(ApiScopePropertyEditViewModel source);

        // ── IdentityServer: IdentityResources ────────────────────────────────
        public partial IdentityResourceDetailsViewModel MapToIdentityResourceDetails(IdentityResource source);
        public partial IdentityResourceEditViewModel MapToIdentityResourceEdit(IdentityResource source);
        public partial IdentityResource MapToIdentityResource(IdentityResourceAddViewModel source);
        public partial IdentityResource MapToIdentityResource(IdentityResourceEditViewModel source);

        public partial IdentityResourceClaimDetailsViewModel MapToIdentityResourceClaimDetails(IdentityResourceClaim source);
        public partial IdentityResourceClaimEditViewModel MapToIdentityResourceClaimEdit(IdentityResourceClaim source);
        public partial IdentityResourceClaim MapToIdentityResourceClaim(IdentityResourceClaimAddViewModel source);
        public partial IdentityResourceClaim MapToIdentityResourceClaim(IdentityResourceClaimEditViewModel source);

        public partial IdentityResourcePropertyDetailsViewModel MapToIdentityResourcePropertyDetails(IdentityResourceProperty source);
        public partial IdentityResourcePropertyEditViewModel MapToIdentityResourcePropertyEdit(IdentityResourceProperty source);
        public partial IdentityResourceProperty MapToIdentityResourceProperty(IdentityResourcePropertyAddViewModel source);
        public partial IdentityResourceProperty MapToIdentityResourceProperty(IdentityResourcePropertyEditViewModel source);

        // ── AspNet: Roles ────────────────────────────────────────────────────
        public partial RoleDetailsViewModel MapToRoleDetails(ApplicationRole source);
        public partial RoleEditViewModel MapToRoleEdit(ApplicationRole source);
        public partial ApplicationRole MapToApplicationRole(RoleAddViewModel source);

        // ── AspNet: Users ────────────────────────────────────────────────────
        [MapProperty(nameof(ApplicationUser.GivenName), nameof(UserDetailsViewModel.FirstName))]
        [MapProperty(nameof(ApplicationUser.FamilyName), nameof(UserDetailsViewModel.LastName))]
        [MapperIgnoreTarget(nameof(UserDetailsViewModel.TenantName))]
        [MapperIgnoreTarget(nameof(UserDetailsViewModel.CompanyName))]
        public partial UserDetailsViewModel MapToUserDetails(ApplicationUser source);

        [MapProperty(nameof(ApplicationUser.GivenName), nameof(MyProfileViewModel.FirstName))]
        [MapProperty(nameof(ApplicationUser.FamilyName), nameof(MyProfileViewModel.LastName))]
        [MapperIgnoreTarget(nameof(MyProfileViewModel.TenantName))]
        [MapperIgnoreTarget(nameof(MyProfileViewModel.CompanyName))]
        [MapperIgnoreTarget(nameof(MyProfileViewModel.IsGoogleAuthEnabled))]
        [MapperIgnoreTarget(nameof(MyProfileViewModel.HasGoogleAccount))]
        [MapperIgnoreTarget(nameof(MyProfileViewModel.IsMicrosoftAuthEnabled))]
        [MapperIgnoreTarget(nameof(MyProfileViewModel.HasMicrosoftAccount))]
        [MapperIgnoreTarget(nameof(MyProfileViewModel.CanUseTwoFactor))]
        public partial MyProfileViewModel MapToMyProfile(ApplicationUser source);

        [MapProperty(nameof(UserAddViewModel.FirstName), nameof(ApplicationUser.GivenName))]
        [MapProperty(nameof(UserAddViewModel.LastName), nameof(ApplicationUser.FamilyName))]
        [MapperIgnoreTarget(nameof(ApplicationUser.Status))]
        [MapperIgnoreTarget(nameof(ApplicationUser.StatusChangedAt))]
        [MapperIgnoreTarget(nameof(ApplicationUser.StatusChangedBy))]
        public partial ApplicationUser MapToApplicationUser(UserAddViewModel source);

        // ── AspNet: User claims ──────────────────────────────────────────────
        public partial UserClaimDetailsViewModel MapToUserClaimDetails(ApplicationUserClaim source);
        public partial UserClaimEditViewModel MapToUserClaimEdit(ApplicationUserClaim source);
        public partial ApplicationUserClaim MapToApplicationUserClaim(UserClaimEditViewModel source);

        // ── Enterprise: Tenants ──────────────────────────────────────────────
        public partial TenantDetailsViewModel MapToTenantDetails(Tenant source);
        public partial TenantEditViewModel MapToTenantEdit(Tenant source);
        public partial Tenant MapToTenant(TenantAddViewModel source);
        public partial Tenant MapToTenant(TenantEditViewModel source);

        // ── Enterprise: Tenant managers ──────────────────────────────────────
        [MapProperty("Manager.Email", nameof(TenantManagerDetailsViewModel.UserEmail))]
        [MapProperty("Manager.UserName", nameof(TenantManagerDetailsViewModel.UserName))]
        [MapperIgnoreTarget(nameof(TenantManagerDetailsViewModel.TenantName))]
        public partial TenantManagerDetailsViewModel MapToTenantManagerDetails(TenantManager source);

        public partial List<TenantManagerDetailsViewModel> MapToTenantManagerDetailsList(IEnumerable<TenantManager> source);

        [MapProperty("Manager.Email", nameof(TenantManagerEditViewModel.UserEmail))]
        [MapProperty("Manager.UserName", nameof(TenantManagerEditViewModel.UserName))]
        [MapperIgnoreTarget(nameof(TenantManagerEditViewModel.TenantName))]
        public partial TenantManagerEditViewModel MapToTenantManagerEdit(TenantManager source);

        // ── Enterprise: Companies ────────────────────────────────────────────
        public partial CompanyDetailsViewModel MapToCompanyDetails(Company source);
        public partial List<CompanyDetailsViewModel> MapToCompanyDetailsList(IEnumerable<Company> source);
        public partial CompanyEditViewModel MapToCompanyEdit(Company source);
        public partial Company MapToCompany(CompanyAddViewModel source);
        public partial Company MapToCompany(CompanyEditViewModel source);

        // ── Enterprise: User companies ───────────────────────────────────────
        [MapProperty("User.Email", nameof(CompanyUserDetailsViewModel.UserEmail))]
        [MapProperty("User.UserName", nameof(CompanyUserDetailsViewModel.UserName))]
        public partial CompanyUserDetailsViewModel MapToCompanyUserDetails(UserCompany source);

        public partial List<CompanyUserDetailsViewModel> MapToCompanyUserDetailsList(IEnumerable<UserCompany> source);

        [MapperIgnoreTarget(nameof(CompanyUserEditViewModel.UserEmail))]
        [MapperIgnoreTarget(nameof(CompanyUserEditViewModel.UserName))]
        public partial CompanyUserEditViewModel MapToCompanyUserEdit(UserCompany source);

        [MapperIgnoreTarget(nameof(UserCompanyEditViewModel.CompanyName))]
        [MapperIgnoreTarget(nameof(UserCompanyEditViewModel.TenantId))]
        [MapperIgnoreTarget(nameof(UserCompanyEditViewModel.TenantName))]
        public partial UserCompanyEditViewModel MapToUserCompanyEdit(UserCompany source);
    }
}
