using System;

namespace Codx.Auth.ViewModels
{
    public class MyProfileViewModel
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string TenantName { get; set; }
        public string CompanyName { get; set; }
        
        // External Authentication Properties
        public bool IsGoogleAuthEnabled { get; set; }
        public bool HasGoogleAccount { get; set; }
        
        // Microsoft Authentication Properties
        public bool IsMicrosoftAuthEnabled { get; set; }
        public bool HasMicrosoftAccount { get; set; }
    }
}
