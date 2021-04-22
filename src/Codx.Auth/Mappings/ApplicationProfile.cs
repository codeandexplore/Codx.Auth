using AutoMapper;
using Codx.Auth.ViewModels;
using IdentityServer4.EntityFramework.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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


        }
    }
}
