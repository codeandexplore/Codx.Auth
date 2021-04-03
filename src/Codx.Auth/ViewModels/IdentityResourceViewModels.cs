using System;
using System.ComponentModel.DataAnnotations;

namespace Codx.Auth.ViewModels
{
    public class IdentityResourceViewModels
    {
    }

    public class IdentityResourceDetailsViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
    }

    public class IdentityResourceAddViewModel : BaseIdentityResourceViewModel
    {

    }

    public class IdentityResourceEditViewModel : BaseIdentityResourceViewModel
    {

    }

    public class BaseIdentityResourceViewModel
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
        public string DisplayName { get; set; }
    }
}
