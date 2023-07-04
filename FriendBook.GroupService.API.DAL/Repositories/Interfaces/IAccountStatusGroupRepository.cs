using FriendBook.GroupService.API.Domain.Entities.Postgres;

namespace FriendBook.GroupService.API.DAL.Repositories.Interfaces
{
    public interface IAccountStatusGroupRepository
    {
        public Task<AccountStatusGroup> AddAsync(AccountStatusGroup entity);
        public AccountStatusGroup Update(AccountStatusGroup entity);
        public bool Delete(AccountStatusGroup entity);
        public IQueryable<AccountStatusGroup> GetAll();
        public Task<int> SaveAsync();
    }
}
