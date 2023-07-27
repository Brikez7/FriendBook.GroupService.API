namespace FriendBook.GroupService.API.Domain.Response
{
    public class StandartResponse<T> : BaseResponse<T>
    {
        public override string Message { get; set; } = null!;
        public override ServiceCode StatusCode { get; set; }
        public override T Data { get; set; }
    }
}