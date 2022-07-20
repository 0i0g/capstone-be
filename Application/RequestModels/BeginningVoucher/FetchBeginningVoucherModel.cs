namespace Application.RequestModels
{
    public class FetchBeginningVoucherModel
    {
        public string Code { get; set; }

        public int Size { get; set; } = 5;
    }
}