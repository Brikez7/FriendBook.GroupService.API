using FriendBook.GroupService.API.Domain.InnerResponse;

namespace FriendBook.GroupService.API.BLL.Interfaces
{
    public interface IValidationService<T>
    {
        public Task<BaseResponse<List<Tuple<string, string>>>?> ValidateAsync(T dto);
        public BaseResponse<List<Tuple<string, string>>>? Validate(T dto);
    }
}
