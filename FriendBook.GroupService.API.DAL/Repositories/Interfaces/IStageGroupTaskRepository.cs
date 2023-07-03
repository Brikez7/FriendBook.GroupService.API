using FriendBook.GroupService.API.Domain.Entities.MongoDB;
using MongoDB.Driver;
using System.Linq.Expressions;

namespace FriendBook.GroupService.API.DAL.Repositories.Interfaces
{
    public interface IStageGroupTaskRepository
    {
        public Task<StageGroupTask> AddAsync(StageGroupTask entity);
        public Task<bool> Update(FilterDefinition<StageGroupTask> filter, UpdateDefinition<StageGroupTask> updateDefinition);
        public Task<bool> Delete(Expression<Func<StageGroupTask,bool>> predicate);
        public IQueryable<StageGroupTask> GetAll();
    }
}
