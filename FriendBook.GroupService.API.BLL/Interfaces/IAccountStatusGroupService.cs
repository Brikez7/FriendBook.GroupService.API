﻿using FriendBook.GroupService.API.Domain;
using FriendBook.GroupService.API.Domain.Entities;
using FriendBook.GroupService.API.Domain.InnerResponse;

namespace FriendBook.GroupService.API.BLL.Interfaces
{
    public interface IAccountStatusGroupService
    {
        public Task<BaseResponse<AccountStatusGroupDTO>> CreateAccountStatusGroup(AccountStatusGroup accountStatusGroup);
        public Task<BaseResponse<bool>> DeleteAccountStatusGroup(Guid id, Guid createrId, Guid groupId);
        public Task<BaseResponse<AccountStatusGroupDTO>> UpdateAccountStatusGroup(AccountStatusGroup accountStatusGroup, Guid idCreater);
        public BaseResponse<IQueryable<AccountStatusGroup>> GetAccountStatusGroupOData();
    }
}
