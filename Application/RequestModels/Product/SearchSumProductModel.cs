using System;

namespace Application.RequestModels
{
    public class SearchSumProductModel:PaginationModel
    {
        public Guid? Id { get; set; }

        public string Name { get; set; }
    }
}