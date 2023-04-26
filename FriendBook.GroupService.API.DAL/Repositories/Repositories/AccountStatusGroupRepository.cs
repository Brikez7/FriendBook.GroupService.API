using FriendBook.GroupService.API.DAL.Repositories.Interfaces;
using FriendBook.GroupService.API.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FriendBook.GroupService.API.DAL.Repositories.Repositories
{
    public class AccountStatusGroupRepository : IAccountStatusGroupRepository
    {
        private GroupAppDBContext _dbContext;

        public AccountStatusGroupRepository(GroupAppDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<AccountStatusGroup> AddAsync(AccountStatusGroup entity)
        {
            var entity = await _dbContext.AddAsync(entity);

            return entity;
        }

        public bool Delete(AccountStatusGroup entity)
        {
            throw new NotImplementedException();
        }

        public IQueryable<AccountStatusGroup> GetAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<bool> SaveAsync()
        {
            await _dbContext.SaveChangesAsync();

            return true;
        }

        public AccountStatusGroup Update(AccountStatusGroup entity)
        {
            throw new NotImplementedException();
        }
    }
}
