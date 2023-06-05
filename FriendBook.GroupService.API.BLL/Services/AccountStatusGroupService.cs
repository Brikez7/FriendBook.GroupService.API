using FriendBook.GroupService.API.BLL.Interfaces;
using FriendBook.GroupService.API.DAL.Repositories.Interfaces;
using FriendBook.GroupService.API.Domain;
using FriendBook.GroupService.API.Domain.Entities;
using FriendBook.GroupService.API.Domain.InnerResponse;
using Microsoft.EntityFrameworkCore;

namespace FriendBook.GroupService.API.BLL.Services
{
    public class AccountStatusGroupService : IAccountStatusGroupService
    {
        private readonly IAccountStatusGroupRepository _accountStatusGroupRepository;
        private readonly IGroupRepository _groupRepository;
        public AccountStatusGroupService(IAccountStatusGroupRepository accountStatusGroupRepository, IGroupRepository groupRepository)
        {
            _accountStatusGroupRepository = accountStatusGroupRepository;
            _groupRepository = groupRepository;
        }

        public async Task<BaseResponse<AccountStatusGroupDTO>> CreateAccountStatusGroup(AccountStatusGroup accountStatusGroup)
        {
            if (await _accountStatusGroupRepository.getAll().AnyAsync(x => x.IdGroup == accountStatusGroup.IdGroup && x.AccountId == accountStatusGroup.AccountId))
            {
                return new StandartResponse<AccountStatusGroupDTO>()
                {
                    Message = $"Account in group exists",
                    StatusCode = StatusCode.InternalServerError
                };
            }

            var createdAccountaStatusGroup = await _accountStatusGroupRepository.AddAsync(accountStatusGroup);
            await _accountStatusGroupRepository.SaveAsync();

            return new StandartResponse<AccountStatusGroupDTO>()
            {
                Data = new AccountStatusGroupDTO(createdAccountaStatusGroup),
                StatusCode = StatusCode.AccountStatusGroupCreate
            };
        }

        public async Task<BaseResponse<bool>> DeleteAccountStatusGroup(Guid userId, Guid createrId, Guid groupId)
        {
            if (await _groupRepository.GetAll().AnyAsync(x => x.CreaterId == createrId && groupId == x.Id))
            {
                var accountStatusGroup = await _accountStatusGroupRepository.getAll().SingleOrDefaultAsync(x => x.AccountId == userId && x.IdGroup == groupId && x.AccountId != createrId);

                if (accountStatusGroup is null)
                {
                    return new StandartResponse<bool>()
                    {
                        Message = $"Account in group not exists",
                        StatusCode = StatusCode.InternalServerError
                    };
                }

                var Result = _accountStatusGroupRepository.Delete(accountStatusGroup);
                Result = await _accountStatusGroupRepository.SaveAsync();

                if (!Result)
                {
                    return new StandartResponse<bool>()
                    {
                        Message = "Account status group has not deleted",
                        StatusCode = StatusCode.InternalServerError
                    };
                }

                return new StandartResponse<bool>()
                {
                    Data = Result,
                    StatusCode = StatusCode.AccountStatusGroupDelete
                };
            }
            return new StandartResponse<bool>()
            {
                Message = "Group with id creater not found",
                StatusCode = StatusCode.InternalServerError
            };
        }

        public BaseResponse<IQueryable<AccountStatusGroup>> GetAccountStatusGroupOData()
        {
            var accountsStatusGroups = _accountStatusGroupRepository.getAll();
            if (accountsStatusGroups.Count() == 0)
            {
                return new StandartResponse<IQueryable<AccountStatusGroup>>()
                {
                    Message = "entity not found"
                };
            }

            return new StandartResponse<IQueryable<AccountStatusGroup>>()
            {
                Data = accountsStatusGroups,
                StatusCode = StatusCode.AccountStatusGroupRead
            };
        }

        public async Task<BaseResponse<AccountStatusGroupDTO>> UpdateAccountStatusGroup(AccountStatusGroup accountStatusGroup, Guid idCreater)
        {
            if (await _groupRepository.GetAll().AnyAsync(x => x.Id == accountStatusGroup.IdGroup && x.CreaterId == idCreater))
            {
                var accountStatus = await _accountStatusGroupRepository.getAll().SingleOrDefaultAsync(x => x.IdGroup == accountStatusGroup.IdGroup && x.AccountId == accountStatusGroup.AccountId && accountStatusGroup.AccountId != idCreater);
                if (accountStatus == null) 
                {
                    return new StandartResponse<AccountStatusGroupDTO>()
                    {
                        Message = "Account not found",
                        StatusCode = StatusCode.InternalServerError
                    };
                }

                var updatedAccountaStatusGroup = await _accountStatusGroupRepository.Update(accountStatusGroup);
                var result = await _accountStatusGroupRepository.SaveAsync();

                if (updatedAccountaStatusGroup == null || result == false) 
                {
                    return new StandartResponse<AccountStatusGroupDTO>()
                    {
                        Message = "Error update",
                        StatusCode = StatusCode.InternalServerError
                    };
                }

                return new StandartResponse<AccountStatusGroupDTO>()
                {
                    Data = new AccountStatusGroupDTO(updatedAccountaStatusGroup),
                    StatusCode = StatusCode.AccountStatusGroupUpdate
                };
            }
            return new StandartResponse<AccountStatusGroupDTO>()
            {
                Message = "Group with id creater not found",
                StatusCode = StatusCode.InternalServerError
            };
        }
    }
}
