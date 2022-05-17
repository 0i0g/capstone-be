using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.Extensions
{
    public static class QueryableExtensions
    {
        public static PaginationViewModel<TSource> ToPagination<TSource>(this IQueryable<TSource> source, int index, int size) where TSource : class
        {
            return new PaginationViewModel<TSource>
            {
                PageIndex = index,
                PageSize = size,
                Items = source.Skip((index - 1) * size).Take(size).ToList(),
                //TotalRows = (int)Math.Ceiling(source.Count() / (double)size)
                TotalRows = source.Count()
            };
        }
    }

    public class PaginationViewModel<T> where T : class
    {
        public int PageIndex { get; set; }

        public int PageSize { get; set; }

        public int TotalRows { get; set; }

        public ICollection<T> Items { get; set; }
    }
}
