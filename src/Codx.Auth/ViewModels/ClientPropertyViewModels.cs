using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Codx.Auth.ViewModels
{
    public class ClientPropertyDetailsViewModel : BaseClientPropertyViewModel
    {
        public int Id { get; set; }
    }


    public class ClientPropertyAddViewModel : BaseClientPropertyViewModel
    {
    }

    public class ClientPropertyEditViewModel : BaseClientPropertyViewModel
    {
        public int Id { get; set; }
    }

    public class BaseClientPropertyViewModel
    {

        public int ClientId { get; set; }

        [Required]
        [StringLength(250)]
        public string Key { get; set; }

        [Required]
        [StringLength(2000)]
        public string Value { get; set; }
    }


}
