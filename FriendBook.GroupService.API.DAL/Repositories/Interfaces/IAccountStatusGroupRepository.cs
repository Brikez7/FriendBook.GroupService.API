using FriendBook.GroupService.API.Domain.Entities;

namespace FriendBook.GroupService.API.DAL.Repositories.Interfaces
{
    public interface IAccountStatusGroupRepository
    {
        public Task<AccountStatusGroup> AddAsync(AccountStatusGroup entity);
        public Task<AccountStatusGroup> Update(AccountStatusGroup entity);
        public bool Delete(AccountStatusGroup entity);
        public IQueryable<AccountStatusGroup> GetAll();
        public Task<bool> SaveAsync();
    }
}
