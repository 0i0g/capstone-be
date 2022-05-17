using Data.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.RequestModels
{
    public class PaginationModel
    {
        [Range(1, int.MaxValue)]
        public int PageIndex { get; set; } = 1;

        [Range(1, int.MaxValue)]
        public int PageSize { get; set; } = 10;

        public OrderBy OrderBy { get; set; }

        // use internal to hide on swagger
        internal bool IsSortAsc => OrderBy is { Sort: Sort.ASC };

        // use internal to hide on swagger
        internal string OrderByName => OrderBy?.Name?.ToUpper() ?? string.Empty;
    }

    public class OrderBy
    {
        public string Name { get; set; }

        public Sort Sort { get; set; }
    }
}
