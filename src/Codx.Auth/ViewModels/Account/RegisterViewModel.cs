using Codx.Auth.Helpers.CustomAttributes;
using Codx.Auth.ViewModels.Account;
using System.ComponentModel.DataAnnotations;

namespace Codx.Auth.ViewModels
{
    public class RegisterViewModel: RegisterBaseModel
    {
        public string ReturnUrl { get; set; }
    }
}
