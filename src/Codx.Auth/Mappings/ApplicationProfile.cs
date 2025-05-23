﻿using AutoMapper;
using Codx.Auth.Data.Entities.AspNet;
using Codx.Auth.Data.Entities.Enterprise;
using Codx.Auth.ViewModels;
using Duende.IdentityServer.EntityFramework.Entities;

namespace Codx.Auth.Mappings
{
    public class ApplicationProfile : Profile
    {
        public ApplicationProfile()
        {

            CreateMap<Client, ClientDetailsViewModel>();
            CreateMap<Client, ClientAddViewModel>().ReverseMap();
            CreateMap<Client, ClientEditViewModel>().ReverseMap();
            CreateMap<Client, ClientEditViewModel>();

            CreateMap<ClientClaim, ClientClaimDetailsViewModel>();
            CreateMap<ClientClaim, ClientClaimAddViewModel>().ReverseMap();
            CreateMap<ClientClaim, ClientClaimEditViewModel>().ReverseMap();
            CreateMap<ClientClaim, ClientClaimEditViewModel>();

            CreateMap<ClientGrantType, ClientGrantTypeDetailsViewModel>();
            CreateMap<ClientGrantType, ClientGrantTypeAddViewModel>().ReverseMap();
            CreateMap<ClientGrantType, ClientGrantTypeEditViewModel>().ReverseMap();
            CreateMap<ClientGrantType, ClientGrantTypeEditViewModel>();

            CreateMap<ClientScope, ClientScopeDetailsViewModel>();
            CreateMap<ClientScope, ClientScopeAddViewModel>().ReverseMap();
            CreateMap<ClientScope, ClientScopeEditViewModel>().ReverseMap();
            CreateMap<ClientScope, ClientScopeEditViewModel>();

            CreateMap<ClientCorsOrigin, ClientCorsOriginDetailsViewModel>();
            CreateMap<ClientCorsOrigin, ClientCorsOriginAddViewModel>().ReverseMap();
            CreateMap<ClientCorsOrigin, ClientCorsOriginEditViewModel>().ReverseMap();
            CreateMap<ClientCorsOrigin, ClientCorsOriginEditViewModel>();

            CreateMap<ClientSecret, ClientSecretDetailsViewModel>();
            CreateMap<ClientSecret, ClientSecretAddViewModel>().ReverseMap();
            CreateMap<ClientSecret, ClientSecretEditViewModel>().ReverseMap();
            CreateMap<ClientSecret, ClientSecretEditViewModel>();

            CreateMap<ClientIdPRestriction, ClientIdpRestrictionDetailsViewModel>();
            CreateMap<ClientIdPRestriction, ClientIdpRestrictionAddViewModel>().ReverseMap();
            CreateMap<ClientIdPRestriction, ClientIdpRestrictionEditViewModel>().ReverseMap();
            CreateMap<ClientIdPRestriction, ClientIdpRestrictionEditViewModel>();

            CreateMap<ClientRedirectUri, ClientRedirectUriDetailsViewModel>();
            CreateMap<ClientRedirectUri, ClientRedirectUriAddViewModel>().ReverseMap();
            CreateMap<ClientRedirectUri, ClientRedirectUriEditViewModel>().ReverseMap();
            CreateMap<ClientRedirectUri, ClientRedirectUriEditViewModel>();

            CreateMap<ClientPostLogoutRedirectUri, ClientPostLogoutRedirectUriDetailsViewModel>();
            CreateMap<ClientPostLogoutRedirectUri, ClientPostLogoutRedirectUriAddViewModel>().ReverseMap();
            CreateMap<ClientPostLogoutRedirectUri, ClientPostLogoutRedirectUriEditViewModel>().ReverseMap();
            CreateMap<ClientPostLogoutRedirectUri, ClientPostLogoutRedirectUriEditViewModel>();

            CreateMap<ClientProperty, ClientPropertyDetailsViewModel>();
            CreateMap<ClientProperty, ClientPropertyAddViewModel>().ReverseMap();
            CreateMap<ClientProperty, ClientPropertyEditViewModel>().ReverseMap();
            CreateMap<ClientProperty, ClientPropertyEditViewModel>();


            CreateMap<ApiResource, ApiResourceDetailsViewModel>();
            CreateMap<ApiResource, ApiResourceAddViewModel>().ReverseMap();
            CreateMap<ApiResource, ApiResourceEditViewModel>().ReverseMap();
            CreateMap<ApiResource, ApiResourceEditViewModel>();

            CreateMap<ApiResourceClaim, ApiResourceClaimDetailsViewModel>();
            CreateMap<ApiResourceClaim, ApiResourceClaimAddViewModel>().ReverseMap();
            CreateMap<ApiResourceClaim, ApiResourceClaimEditViewModel>().ReverseMap();
            CreateMap<ApiResourceClaim, ApiResourceClaimEditViewModel>();

            CreateMap<ApiResourceScope, ApiResourceScopeDetailsViewModel>();
            CreateMap<ApiResourceScope, ApiResourceScopeAddViewModel>().ReverseMap();
            CreateMap<ApiResourceScope, ApiResourceScopeEditViewModel>().ReverseMap();
            CreateMap<ApiResourceScope, ApiResourceScopeEditViewModel>();

            CreateMap<ApiResourceSecret, ApiResourceSecretDetailsViewModel>();
            CreateMap<ApiResourceSecret, ApiResourceSecretAddViewModel>().ReverseMap();
            CreateMap<ApiResourceSecret, ApiResourceSecretEditViewModel>().ReverseMap();
            CreateMap<ApiResourceSecret, ApiResourceSecretEditViewModel>();

            CreateMap<ApiResourceProperty, ApiResourcePropertyDetailsViewModel>();
            CreateMap<ApiResourceProperty, ApiResourcePropertyAddViewModel>().ReverseMap();
            CreateMap<ApiResourceProperty, ApiResourcePropertyEditViewModel>().ReverseMap();
            CreateMap<ApiResourceProperty, ApiResourcePropertyEditViewModel>();

            CreateMap<ApiScope, ApiScopeDetailsViewModel>();
            CreateMap<ApiScope, ApiScopeAddViewModel>().ReverseMap();
            CreateMap<ApiScope, ApiScopeEditViewModel>().ReverseMap();
            CreateMap<ApiScope, ApiScopeEditViewModel>();

            CreateMap<ApiScopeClaim, ApiScopeClaimDetailsViewModel>();
            CreateMap<ApiScopeClaim, ApiScopeClaimAddViewModel>().ReverseMap();
            CreateMap<ApiScopeClaim, ApiScopeClaimEditViewModel>().ReverseMap();
            CreateMap<ApiScopeClaim, ApiScopeClaimEditViewModel>();

            CreateMap<ApiScopeProperty, ApiScopePropertyDetailsViewModel>();
            CreateMap<ApiScopeProperty, ApiScopePropertyAddViewModel>().ReverseMap();
            CreateMap<ApiScopeProperty, ApiScopePropertyEditViewModel>().ReverseMap();
            CreateMap<ApiScopeProperty, ApiScopePropertyEditViewModel>();


            CreateMap<IdentityResource, IdentityResourceDetailsViewModel>();
            CreateMap<IdentityResource, IdentityResourceAddViewModel>().ReverseMap();
            CreateMap<IdentityResource, IdentityResourceEditViewModel>().ReverseMap();
            CreateMap<IdentityResource, IdentityResourceEditViewModel>();

            CreateMap<IdentityResourceClaim, IdentityResourceClaimDetailsViewModel>();
            CreateMap<IdentityResourceClaim, IdentityResourceClaimAddViewModel>().ReverseMap();
            CreateMap<IdentityResourceClaim, IdentityResourceClaimEditViewModel>().ReverseMap();
            CreateMap<IdentityResourceClaim, IdentityResourceClaimEditViewModel>();

            CreateMap<IdentityResourceProperty, IdentityResourcePropertyDetailsViewModel>();
            CreateMap<IdentityResourceProperty, IdentityResourcePropertyAddViewModel>().ReverseMap();
            CreateMap<IdentityResourceProperty, IdentityResourcePropertyEditViewModel>().ReverseMap();
            CreateMap<IdentityResourceProperty, IdentityResourcePropertyEditViewModel>();


            CreateMap<ApplicationRole, RoleDetailsViewModel>();
            CreateMap<ApplicationRole, RoleAddViewModel>().ReverseMap();
            CreateMap<ApplicationRole, RoleEditViewModel>().ReverseMap();
            CreateMap<ApplicationRole, RoleEditViewModel>();

            CreateMap<ApplicationUser, UserDetailsViewModel>();
            CreateMap<ApplicationUser, UserAddViewModel>().ReverseMap();
            CreateMap<ApplicationUser, UserEditViewModel>().ReverseMap();
            CreateMap<ApplicationUser, UserEditViewModel>();

            CreateMap<ApplicationUser, MyProfileViewModel>();

            CreateMap<ApplicationUserClaim, UserClaimDetailsViewModel>();
            CreateMap<ApplicationUserClaim, UserClaimAddViewModel>().ReverseMap();
            CreateMap<ApplicationUserClaim, UserClaimEditViewModel>().ReverseMap();
            CreateMap<ApplicationUserClaim, UserClaimEditViewModel>();

            CreateMap<ApplicationUserRole, UserRoleDetailsViewModel>();
            CreateMap<ApplicationUserRole, UserRoleAddViewModel>().ReverseMap();
            CreateMap<ApplicationUserRole, UserRoleEditViewModel>().ReverseMap();
            CreateMap<ApplicationUserRole, UserRoleEditViewModel>();

            CreateMap<Tenant, TenantDetailsViewModel>();
            CreateMap<Tenant, TenantAddViewModel>().ReverseMap();
            CreateMap<Tenant, TenantEditViewModel>().ReverseMap();
            CreateMap<Tenant, TenantEditViewModel>();

            CreateMap<TenantManager, TenantManagerDetailsViewModel>()
              .ForMember(dest => dest.UserEmail, opt => opt.MapFrom(src => src.Manager.Email))
              .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Manager.UserName));
            CreateMap<TenantManager, TenantManagerAddViewModel>().ReverseMap();
            CreateMap<TenantManager, TenantManagerEditViewModel>().ReverseMap();
            CreateMap<TenantManager, TenantManagerEditViewModel>()
                .ForMember(dest => dest.UserEmail, opt => opt.MapFrom(src => src.Manager.Email))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Manager.UserName));

            CreateMap<Company, CompanyDetailsViewModel>();
            CreateMap<Company, CompanyAddViewModel>().ReverseMap();
            CreateMap<Company, CompanyEditViewModel>().ReverseMap();
            CreateMap<Company, CompanyEditViewModel>();

            CreateMap<UserCompany, CompanyUserDetailsViewModel>()
                .ForMember(dest => dest.UserEmail, opt => opt.MapFrom(src => src.User.Email))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName));
            CreateMap<UserCompany, CompanyUserAddViewModel>().ReverseMap();
            CreateMap<UserCompany, CompanyUserEditViewModel>().ReverseMap();
            CreateMap<UserCompany, CompanyUserEditViewModel>();

            CreateMap<UserCompany, UserCompanyDetailsViewModel>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.CompanyId, opt => opt.MapFrom(src => src.CompanyId))
                .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.Company.Name))
                .ForMember(dest => dest.TenantId, opt => opt.MapFrom(src => src.Company.TenantId))
                .ForMember(dest => dest.TenantName, opt => opt.MapFrom(src => src.User.UserName));
            CreateMap<UserCompany, UserCompanyAddViewModel>().ReverseMap();
            CreateMap<UserCompany, UserCompanyEditViewModel>().ReverseMap();
            CreateMap<UserCompany, UserCompanyEditViewModel>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.CompanyId, opt => opt.MapFrom(src => src.CompanyId))
                .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.Company.Name))
                .ForMember(dest => dest.TenantId, opt => opt.MapFrom(src => src.Company.TenantId))
                .ForMember(dest => dest.TenantName, opt => opt.MapFrom(src => src.Company.Tenant.Name));
        }
    }
}
