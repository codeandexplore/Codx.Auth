﻿using AutoMapper;
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
            CreateMap<ApiResource, ApiResourceAddViewModel>().ReverseMap();
            CreateMap<ApiResource, ApiResourceEditViewModel>().ReverseMap();
            CreateMap<ApiResource, ApiResourceEditViewModel>();
        }
    }
}
