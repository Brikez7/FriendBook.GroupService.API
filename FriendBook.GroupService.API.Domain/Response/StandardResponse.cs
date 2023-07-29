namespace FriendBook.GroupService.API.Domain.Response
{
    public class StandardResponse<T> : BaseResponse<T>
    {
        public override string? Message { get; set; }
        public override ServiceCode ServiceCode { get; set; }
        public override T Data { get; set; }
    }
}