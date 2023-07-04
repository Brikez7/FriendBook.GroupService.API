using FriendBook.GroupService.API.BLL.gRPCClients.AccountService;
using FriendBook.GroupService.API.BLL.gRPCClients.ContactService;
using FriendBook.GroupService.API.BLL.Interfaces;
using FriendBook.GroupService.API.DAL.Repositories.Interfaces;
using FriendBook.GroupService.API.Domain.DTO.DocumentGroupTaskDTOs;
using FriendBook.GroupService.API.Domain.DTO.GroupTaskDTOs;
using FriendBook.GroupService.API.Domain.Entities;
using FriendBook.GroupService.API.Domain.Entities.Postgres;
using FriendBook.GroupService.API.Domain.Response;
using Microsoft.EntityFrameworkCore;

namespace FriendBook.GroupService.API.BLL.Services
{
    public class AccountStatusGroupService : IAccountStatusGroupService
    {
        private readonly IAccountStatusGroupRepository _accountStatusGroupRepository;
        private readonly IGroupRepository _groupRepository;
        private readonly IStageGroupTaskRepository _stageGroupTaskRepository;
        public AccountStatusGroupService(IAccountStatusGroupRepository accountStatusGroupRepository, IGroupRepository groupRepository, IStageGroupTaskRepository stageGroupTaskRepository)
        {
            _accountStatusGroupRepository = accountStatusGroupRepository;
            _groupRepository = groupRepository;
            _stageGroupTaskRepository = stageGroupTaskRepository;
        }

        public async Task<BaseResponse<AccountStatusGroupDTO>> CreateAccountStatusGroup(Guid createrId,AccountStatusGroupDTO accountStatusGroupDTO)
        {
            if (!await _groupRepository.GetAll().AnyAsync(x => x.CreaterId == createrId && x.Id == accountStatusGroupDTO.GroupId))
                return new StandartResponse<AccountStatusGroupDTO> { Message = "Account not found or you not access add new account", StatusCode = StatusCode.UserNotAccess };

            if (accountStatusGroupDTO.RoleAccount == RoleAccount.Creater)
            {
                return new StandartResponse<AccountStatusGroupDTO>()
                {
                    Message = "New account with status status creator not been added from group",
                    StatusCode = StatusCode.UserNotAccess,
                };
            }
            if (await _accountStatusGroupRepository.GetAll().AnyAsync(x => x.IdGroup == accountStatusGroupDTO.GroupId && x.AccountId == accountStatusGroupDTO.AccountId))
            {
                return new StandartResponse<AccountStatusGroupDTO>()
                {
                    Message = $"New account in group already exists",
                    StatusCode = StatusCode.AccountStatusGroupAlreadyExists
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

        public async Task<BaseResponse<bool>> DeleteAccountStatusGroup(Guid deletedStatusAccountId, Guid createrId, Guid groupId)
        {
            if (!await _groupRepository.GetAll().AnyAsync(x => x.CreaterId == createrId && x.Id == groupId))
                return new StandartResponse<bool> { Message = "Account not found or you not access delete account", StatusCode = StatusCode.UserNotAccess };

            var accountStatusGroup = await _accountStatusGroupRepository.GetAll().SingleOrDefaultAsync(x => x.AccountId == deletedStatusAccountId && x.IdGroup == groupId);

            if (accountStatusGroup is null || deletedStatusAccountId == createrId)
            {
                return new StandartResponse<bool>()
                {
                    Message = $"Account in group not exists",
                    StatusCode = StatusCode.EntityNotFound
                };
            }

            var Result = _accountStatusGroupRepository.Delete(accountStatusGroup);
            await _accountStatusGroupRepository.SaveAsync();

            return new StandartResponse<bool>()
            {
                Data = Result,
                StatusCode = StatusCode.AccountStatusGroupDelete
            };
        }

        public async Task<BaseResponse<AccountStatusGroup?>> GetAccountStatusesGroupFromUserGroup(Guid userId, Guid groupId)
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
                    StatusCode = StatusCode.EntityNotFound
                };
            }
            else if (accountStatusGroup.Group.GroupTasks is null)
            {
                return new StandartResponse<AccountStatusGroup?>
                {
                    Message = "Tasks not found",
                    StatusCode = StatusCode.EntityNotFound
                };
            }

            return new StandartResponse<AccountStatusGroup?>
            {
                Data = accountStatusGroup,
                StatusCode = StatusCode.AccountStatusGroupRead
            };
        }

        public BaseResponse<IQueryable<AccountStatusGroup>> GetAccountStatusGroupOData()
        {
            var accountsStatusGroups = _accountStatusGroupRepository.GetAll();

            return new StandartResponse<IQueryable<AccountStatusGroup>>()
            {
                Data = accountsStatusGroups,
                StatusCode = StatusCode.AccountStatusGroupRead
            };
        }

        public async Task<BaseResponse<Profile[]>> GetProfilesByIdGroup(Guid groupId, ResponseProfiles profileDTOs)
        {
            var usersInSearchedGroudId = await _accountStatusGroupRepository.GetAll()
                                                                            .Where(x => x.IdGroup == groupId).Select(x => x.AccountId)
                                                                            .ToArrayAsync();

            if (usersInSearchedGroudId?.Length > 0)
            {
                var usersInGroup = profileDTOs.Profiles.AsEnumerable().Join(usersInSearchedGroudId,
                   profile => Guid.Parse(profile.Id),
                   id => id,
                   (profile, id) => profile);

                return new StandartResponse<Profile[]>
                {
                    Data = usersInGroup.ToArray(),
                    StatusCode = StatusCode.ProphileMapped,
                };
            }
            return new StandartResponse<Profile[]>()
            {
                Message = "Group not found",
                StatusCode = StatusCode.EntityNotFound,
            };
        }

        public async Task<BaseResponse<ResponseTasksPage>> TasksAddSubscribedUserLogins(List<GroupTask> groupTasks, User[] usersLoginWithId, bool isAdmin)
        { 
            List<StageGroupTaskIconDTO> stageGroupTask = new List<StageGroupTaskIconDTO>();
            List<ResponseGroupTaskView> tasksPages = new List<ResponseGroupTaskView>();
            foreach (var task in groupTasks)
            {
                stageGroupTask = await _stageGroupTaskRepository.GetAll().Where(x => x.IdGroupTask == task.Id).Select(x => new StageGroupTaskIconDTO(x.Id,x.Name,x.IdGroupTask)).ToListAsync();

                var namesUser = task.Team.Join(
                                        usersLoginWithId,
                                        userId => userId,
                                        loginWithIdUser => Guid.Parse(loginWithIdUser.Id),
                                        (task, loginWithIdUser) => loginWithIdUser.Login).ToArray();

                ResponseGroupTaskView groupTaskViewDTO = new ResponseGroupTaskView(task, namesUser, stageGroupTask);
                tasksPages.Add(groupTaskViewDTO);
            }

            if (tasksPages.Count == 0)
            {
                return new StandartResponse<ResponseTasksPage>
                {
                    Message = "Tasks not found",
                    StatusCode = StatusCode.EntityNotFound
                };
            }

            var tasksPageDTO = new ResponseTasksPage(tasksPages.ToArray(), isAdmin);

            return new StandartResponse<ResponseTasksPage>
            {
                Data = tasksPageDTO,
                StatusCode = StatusCode.GroupTaskMapped
            };
        }

        public async Task<BaseResponse<AccountStatusGroupDTO>> UpdateAccountStatusGroup(AccountStatusGroupDTO accountStatusGroupDTO, Guid createrId)
        {
            if (!await _groupRepository.GetAll().AnyAsync(x => x.CreaterId == createrId && x.Id == accountStatusGroupDTO.GroupId))
                return new StandartResponse<AccountStatusGroupDTO> { Message = "Account not found or you not access update account", StatusCode = StatusCode.UserNotAccess };

            var accountStatus = await _accountStatusGroupRepository.GetAll().SingleOrDefaultAsync(x => x.IdGroup == accountStatusGroupDTO.GroupId && x.AccountId == accountStatusGroupDTO.AccountId);
            if (accountStatus == null || accountStatusGroupDTO.AccountId == createrId) 
            {
                return new StandartResponse<AccountStatusGroupDTO>()
                {
                    Message = "Account not found",
                    StatusCode = StatusCode.EntityNotFound
                };
            }

            accountStatus.RoleAccount = accountStatusGroupDTO.RoleAccount;
            var updatedAccountaStatusGroup = _accountStatusGroupRepository.Update(accountStatus);
            var result = await _accountStatusGroupRepository.SaveAsync();

            return new StandartResponse<AccountStatusGroupDTO>()
            {
                Data = new AccountStatusGroupDTO(updatedAccountaStatusGroup),
                StatusCode = StatusCode.AccountStatusGroupUpdate
            };
            
        }
    }
}
