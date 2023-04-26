namespace FriendBook.GroupService.API.Domain.InnerResponse
{
    public class StandartResponse<T> : BaseResponse<T>
    {
        public override string Message { get; set; } = null!;
        public override StatusCode StatusCode { get; set; }
        public override T Data { get; set; }
    }
}