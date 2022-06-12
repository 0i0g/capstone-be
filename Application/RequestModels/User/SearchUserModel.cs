namespace Application.RequestModels.User
{
    public class SearchUserModel: PaginationModel
    {
        public string Name { get; set; }
    }
}