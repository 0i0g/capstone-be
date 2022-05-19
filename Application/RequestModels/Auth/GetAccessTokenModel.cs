using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.RequestModels.Auth
{
    public class GetAccessTokenModel
    {
        [Required]
        public Guid UserId { get; set; }

        [Required]
        public string RefreshToken { get; set; }
    }
}
