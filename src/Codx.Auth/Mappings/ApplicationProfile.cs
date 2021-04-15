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
        }
    }
}
