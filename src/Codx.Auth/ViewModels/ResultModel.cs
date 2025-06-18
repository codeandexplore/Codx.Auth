using System.Collections.Generic;
using System.Linq;

namespace Codx.Auth.ViewModels
{
    public class ResultModel
    {
        public bool Success { get; set; }
        public IEnumerable<string> Errors { get; set; } = Enumerable.Empty<string>();
    }
}
