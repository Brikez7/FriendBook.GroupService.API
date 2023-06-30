namespace FriendBook.GroupService.API.Domain.Response
{
    public abstract class BaseResponse<T>
    {
        public virtual T Data { get; set; }
        public virtual StatusCode StatusCode { get; set; }
        public virtual string? Message { get; set; } 
    }
}