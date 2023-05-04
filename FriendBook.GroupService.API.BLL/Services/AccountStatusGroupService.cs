using FriendBook.GroupService.API.BLL.Interfaces;
using FriendBook.GroupService.API.DAL.Repositories.Interfaces;
using FriendBook.GroupService.API.DAL.Repositories.Repositories;
using FriendBook.GroupService.API.Domain;
using FriendBook.GroupService.API.Domain.Entities;
using FriendBook.GroupService.API.Domain.InnerResponse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriendBook.GroupService.API.BLL.Services
{
    public class AccountStatusGroupService : IAccountStatusGroupService
    {
        private readonly IAccountStatusGroupRepository _accountStatusGroupRepository;

        public AccountStatusGroupService(IAccountStatusGroupRepository accountStatusGroupRepository)
        {
            _accountStatusGroupRepository = accountStatusGroupRepository;
        }

        public async Task<BaseResponse<AccountStatusGroup>> CreateAccountStatusGroup(AccountStatusGroup accountStatusGroup)
        {
            var createdAccountaStatusGroup = await _accountStatusGroupRepository.AddAsync(accountStatusGroup);
            await _accountStatusGroupRepository.SaveAsync();

            return new StandartResponse<AccountStatusGroup>()
            {
                Data = createdAccountaStatusGroup,
                StatusCode = StatusCode.AccountStatusGroupCreate
                ICollection
            };
        }

        public async Task<BaseResponse<bool>> DeleteAccountStatusGroup(Guid id)
        {
            var Result = _accountStatusGroupRepository.Delete(new AccountStatusGroup(id));
            await _accountStatusGroupRepository.SaveAsync();

            return new StandartResponse<bool>()
            {
                Data = Result,
                StatusCode = StatusCode.AccountStatusGroupDelete
            };
        }

        public BaseResponse<IQueryable<AccountStatusGroup>> GetAccountStatusGroupOData()
        {
            var accountsStatusGroups = _accountStatusGroupRepository.GetAsync();
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

        public async Task<BaseResponse<AccountStatusGroup>> UpdateAccountStatusGroup(AccountStatusGroup accountStatusGroup)
        {
            var updatedAccountaStatusGroup = _accountStatusGroupRepository.Update(accountStatusGroup);
            await _accountStatusGroupRepository.SaveAsync();

            return new StandartResponse<AccountStatusGroup>()
            {
                Data = updatedAccountaStatusGroup,
                StatusCode = StatusCode.AccountStatusGroupUpdate
            };
        }
    }
}
