using FriendBook.GroupService.API.BLL.gRPCClients.AccountService;
using FriendBook.GroupService.API.BLL.gRPCClients.ContactService;
using FriendBook.GroupService.API.BLL.Interfaces;
using FriendBook.GroupService.API.DAL.Repositories.Interfaces;
using FriendBook.GroupService.API.Domain.DTO.AccountStatusGroupDTOs;
using FriendBook.GroupService.API.Domain.DTO.GroupTaskDTOs;
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

        public async Task<BaseResponse<AccountStatusGroupDTO>> CreateAccountStatusGroup(AccountStatusGroupDTO accountStatusGroupDTO)
        {
            if (accountStatusGroupDTO.RoleAccount == RoleAccount.Creater)
            {
                return new StandartResponse<AccountStatusGroupDTO>()
                {
                    Message = "Deleted account not been status creator",
                    StatusCode = StatusCode.InternalServerError,
                };
            }
            if (await _accountStatusGroupRepository.GetAll().AnyAsync(x => x.IdGroup == accountStatusGroupDTO.IdGroup && x.AccountId == accountStatusGroupDTO.AccountId))
            {
                return new StandartResponse<AccountStatusGroupDTO>()
                {
                    Message = $"Account in group exists",
                    StatusCode = StatusCode.InternalServerError
                };
            }

            var accountStatusGroup = new AccountStatusGroup(accountStatusGroupDTO);

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
                var accountStatusGroup = await _accountStatusGroupRepository.GetAll().SingleOrDefaultAsync(x => x.AccountId == userId && x.IdGroup == groupId && x.AccountId != createrId);

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

                if (Result)
                {
                    return new StandartResponse<bool>()
                    {
                        Data = Result,
                        StatusCode = StatusCode.AccountStatusGroupDelete
                    };
                }
                return new StandartResponse<bool>()
                {
                    Message = "Account status group has not deleted",
                    StatusCode = StatusCode.InternalServerError
                };
            }
            return new StandartResponse<bool>()
            {
                Message = "Group with id creater not found",
                StatusCode = StatusCode.InternalServerError
            };
        }

        public async Task<BaseResponse<AccountStatusGroup?>> GetAccountStatusGroupByIdGroupAndUserId(Guid userId, Guid groupId)
        {
            var accountStatusGroup = await _accountStatusGroupRepository.GetAll()
                                                                        .Where(x => x.AccountId == userId)
                                                                        .Include(x => x.Group)
                                                                            .ThenInclude(x => x.GroupTasks)
                                                                        .Include(x => x.Group)
                                                                            .ThenInclude(x => x.AccountStatusGroups)
                                                                        .FirstOrDefaultAsync(x => x.Group != null && x.Group.Id == groupId);

            if (accountStatusGroup == null || accountStatusGroup.Group == null)
            {
                return new StandartResponse<AccountStatusGroup?>
                {
                    Message = "Group not found or Account not found",
                    StatusCode = StatusCode.InternalServerError
                };
            }
            else if (accountStatusGroup.Group.GroupTasks is null)
            {
                return new StandartResponse<AccountStatusGroup?>
                {
                    Message = "Tasks no found",
                    StatusCode = StatusCode.InternalServerError
                };
            }

            return new StandartResponse<AccountStatusGroup?>
            {
                Data = accountStatusGroup,
            };
        }

        public BaseResponse<IQueryable<AccountStatusGroup>> GetAccountStatusGroupOData()
        {
            var accountsStatusGroups = _accountStatusGroupRepository.GetAll();
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

        public async Task<BaseResponse<Profile[]>> GetProfilesByIdGroup(Guid idGroup, ResponseProfiles profileDTOs)
        {
            var usersInSearchedGroudId = await _accountStatusGroupRepository.GetAll()
                                                                            .Where(x => x.IdGroup == idGroup).Select(x => x.AccountId)
                                                                            .ToArrayAsync();

            if (usersInSearchedGroudId is null || usersInSearchedGroudId.Length == 0)
            {
                return new StandartResponse<Profile[]>()
                {
                    Message = "Group not found",
                    StatusCode = StatusCode.InternalServerError,
                };
            }

            var usersInGroup = profileDTOs.Profiles.AsEnumerable().Join(usersInSearchedGroudId,
                               profile => Guid.Parse(profile.Id),
                               id => id,
                               (profile, id) => profile);

            return new StandartResponse<Profile[]>
            {
                Data = usersInGroup.ToArray(),
            }; ;
        }

        public BaseResponse<ResponseTasksPage> TasksJoinUsersLoginWithId(List<GroupTask> groupTasks, User[] usersLoginWithId, bool isAdmin)
        {
            List<ResponseGroupTaskView> tasksPages = new List<ResponseGroupTaskView>();
            foreach (var task in groupTasks)
            {
                var namesUser = task.Team.Join(
                                        usersLoginWithId,
                                        userId => userId,
                                        loginWithIdUser => Guid.Parse(loginWithIdUser.Id),
                                        (task, loginWithIdUser) => loginWithIdUser.Login).ToArray();

                ResponseGroupTaskView groupTaskViewDTO = new ResponseGroupTaskView(task, namesUser);
                tasksPages.Add(groupTaskViewDTO);
            }

            if (tasksPages.Count == 0)
            {
                return new StandartResponse<ResponseTasksPage>
                {
                    Message = "Tasks not found",
                    StatusCode = StatusCode.InternalServerError
                };
            }

            var tasksPageDTO = new ResponseTasksPage(tasksPages.ToArray(), isAdmin);

            return new StandartResponse<ResponseTasksPage>
            {
                Data = tasksPageDTO
            };
        }

        public async Task<BaseResponse<AccountStatusGroupDTO>> UpdateAccountStatusGroup(AccountStatusGroupDTO accountStatusGroupDTO, Guid idCreater)
        {
            if (await _groupRepository.GetAll().AnyAsync(x => x.Id == accountStatusGroupDTO.IdGroup && x.CreaterId == idCreater))
            {
                var accountStatus = await _accountStatusGroupRepository.GetAll().SingleOrDefaultAsync(x => x.IdGroup == accountStatusGroupDTO.IdGroup && x.AccountId == accountStatusGroupDTO.AccountId && accountStatusGroupDTO.AccountId != idCreater);
                if (accountStatus == null) 
                {
                    return new StandartResponse<AccountStatusGroupDTO>()
                    {
                        Message = "Account not found",
                        StatusCode = StatusCode.InternalServerError
                    };
                }

                var accountStatusGroup = new AccountStatusGroup(accountStatusGroupDTO);

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
