using System;
using Codx.Auth.Data.Entities.AspNet;
using Codx.Auth.Data.Entities.Enterprise;
using Codx.Auth.Mappings;
using Codx.Auth.ViewModels;

namespace Codx.Auth.Unit.Test.Mappings
{
    /// <summary>
    /// Smoke tests for the Mapperly-based <see cref="ApplicationMapper"/>,
    /// focused on the non-trivial mappings (property renames and nested-path
    /// flattening) that replaced AutoMapper's <c>ForMember</c> configuration.
    /// </summary>
    public class ApplicationMapperTests
    {
        private readonly ApplicationMapper _mapper = new();

        [Fact]
        public void MapToUserDetails_MapsGivenAndFamilyNameToFirstAndLastName()
        {
            var user = new ApplicationUser
            {
                UserName = "jsmith",
                Email = "jsmith@example.com",
                GivenName = "John",
                FamilyName = "Smith",
                MiddleName = "Q",
            };

            var vm = _mapper.MapToUserDetails(user);

            Assert.Equal("John", vm.FirstName);
            Assert.Equal("Smith", vm.LastName);
            Assert.Equal("Q", vm.MiddleName);
            Assert.Equal("jsmith", vm.UserName);
        }

        [Fact]
        public void MapToMyProfile_MapsGivenAndFamilyNameToFirstAndLastName()
        {
            var user = new ApplicationUser
            {
                GivenName = "Jane",
                FamilyName = "Doe",
            };

            var vm = _mapper.MapToMyProfile(user);

            Assert.Equal("Jane", vm.FirstName);
            Assert.Equal("Doe", vm.LastName);
        }

        [Fact]
        public void MapToApplicationUser_MapsFirstAndLastNameToGivenAndFamilyName()
        {
            var vm = new UserAddViewModel
            {
                UserName = "jsmith",
                Email = "jsmith@example.com",
                FirstName = "John",
                MiddleName = "Q",
                LastName = "Smith",
            };

            var user = _mapper.MapToApplicationUser(vm);

            Assert.Equal("John", user.GivenName);
            Assert.Equal("Smith", user.FamilyName);
            Assert.Equal("Q", user.MiddleName);
            Assert.Equal("jsmith", user.UserName);
            Assert.Equal("jsmith@example.com", user.Email);
            // Server-owned field: the mapper must not set Status (the controller assigns it).
            Assert.Null(user.Status);
        }

        [Fact]
        public void MapToTenantManagerDetails_FlattensManagerEmailAndUserName()
        {
            var manager = new ApplicationUser { Email = "mgr@example.com", UserName = "mgr.user" };
            var tenantManager = new TenantManager
            {
                UserId = Guid.NewGuid(),
                TenantId = Guid.NewGuid(),
                Manager = manager,
            };

            var vm = _mapper.MapToTenantManagerDetails(tenantManager);

            Assert.Equal("mgr@example.com", vm.UserEmail);
            Assert.Equal("mgr.user", vm.UserName);
            Assert.Equal(tenantManager.UserId, vm.UserId);
            Assert.Equal(tenantManager.TenantId, vm.TenantId);
        }

        [Fact]
        public void MapToCompanyUserDetails_FlattensUserEmailAndUserName()
        {
            var user = new ApplicationUser { Email = "user@example.com", UserName = "user.name" };
            var userCompany = new UserCompany
            {
                UserId = Guid.NewGuid(),
                CompanyId = Guid.NewGuid(),
                User = user,
            };

            var vm = _mapper.MapToCompanyUserDetails(userCompany);

            Assert.Equal("user@example.com", vm.UserEmail);
            Assert.Equal("user.name", vm.UserName);
            Assert.Equal(userCompany.UserId, vm.UserId);
            Assert.Equal(userCompany.CompanyId, vm.CompanyId);
        }

        [Fact]
        public void MapToCompanyDetails_MapsSameNamedProperties()
        {
            var company = new Company
            {
                Name = "Acme",
                Email = "acme@example.com",
                TenantId = Guid.NewGuid(),
            };

            var vm = _mapper.MapToCompanyDetails(company);

            Assert.Equal("Acme", vm.Name);
            Assert.Equal("acme@example.com", vm.Email);
            Assert.Equal(company.TenantId, vm.TenantId);
        }
    }
}
