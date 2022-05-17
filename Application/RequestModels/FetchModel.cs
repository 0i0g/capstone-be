using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.Helper;

namespace Application.RequestModels
{
    public class FetchModel
    {
        public string Keyword { get; set; }

        public int Size { get; set; } = 5;
    }
}
