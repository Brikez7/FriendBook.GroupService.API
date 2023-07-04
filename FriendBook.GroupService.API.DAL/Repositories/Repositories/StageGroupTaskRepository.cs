using FriendBook.GroupService.API.DAL.Repositories.Interfaces;
using FriendBook.GroupService.API.Domain.Entities.MongoDB;
using MongoDB.Driver;
using System.Linq.Expressions;

namespace FriendBook.GroupService.API.DAL.Repositories
{
    public class StageGroupTaskRepository : IStageGroupTaskRepository
    {
        private readonly IMongoCollection<StageGroupTask> _collection;

        public StageGroupTaskRepository(IMongoCollection<StageGroupTask> collection)
        {
            _collection = collection;
        }

        public async Task<StageGroupTask> AddAsync(StageGroupTask entity)
        {
            await _collection.InsertOneAsync(entity);
            return entity;
        }

        public async Task<bool> Delete( Expression<Func<StageGroupTask, bool>> predicate)
        {
            var result = await _collection.DeleteManyAsync(predicate);
            return result.IsAcknowledged;
        }

        public IQueryable<StageGroupTask> GetAll()
        {
            return _collection.AsQueryable();
        }

        public async Task<bool> Update(FilterDefinition<StageGroupTask> filter, UpdateDefinition<StageGroupTask> updateDefinition)
        {
            var result = await _collection.UpdateManyAsync(filter, updateDefinition);
            return result.IsAcknowledged;
        }
    }
}
