namespace Application.RequestModels
{
    public class SearchWarehousesModel : PaginationModel
    {
        public string Name { get; set; }

        public string Address { get; set; }
    }
}