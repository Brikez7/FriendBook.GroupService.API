using FriendBook.GroupService.API.BLL.gRPCClients.AccountClient;
using FriendBook.GroupService.API.BLL.gRPCClients.ContactClient;
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

        public async Task<BaseResponse<AccountStatusGroupDTO>> CreateAccountStatusGroup(Guid creatorId,AccountStatusGroupDTO accountStatusGroupDTO)
        {
            if (!await _groupRepository.GetAll().AnyAsync(x => x.CreaterId == creatorId && x.Id == accountStatusGroupDTO.GroupId))
                return new StandardResponse<AccountStatusGroupDTO> { Message = "Account not found or you not access add new account", ServiceCode = ServiceCode.UserNotAccess };

            if (accountStatusGroupDTO.RoleAccount == RoleAccount.Creator)
            {
                return new StandardResponse<AccountStatusGroupDTO>()
                {
                    Message = "New account with status status creator not been added from group",
                    ServiceCode = ServiceCode.UserNotAccess,
                };
            }
            if (await _accountStatusGroupRepository.GetAll().AnyAsync(x => x.IdGroup == accountStatusGroupDTO.GroupId && x.AccountId == accountStatusGroupDTO.AccountId))
            {
                return new StandardResponse<AccountStatusGroupDTO>()
                {
                    Message = $"New account in group already exists",
                    ServiceCode = ServiceCode.AccountStatusGroupAlreadyExists
                };
            }

            var accountStatusGroup = new AccountStatusGroup(accountStatusGroupDTO);

            var createdAccountStatusGroup = await _accountStatusGroupRepository.AddAsync(accountStatusGroup);
            await _accountStatusGroupRepository.SaveAsync();

            return new StandardResponse<AccountStatusGroupDTO>()
            {
                Data = new AccountStatusGroupDTO(createdAccountStatusGroup),
                ServiceCode = ServiceCode.AccountStatusGroupCreated
            };
        }

        public async Task<BaseResponse<bool>> DeleteAccountStatusGroup(Guid deletedStatusAccountId, Guid creatorId, Guid groupId)
        {
            if (!await _groupRepository.GetAll().AnyAsync(x => x.CreaterId == creatorId && x.Id == groupId))
                return new StandardResponse<bool> { Message = "Account not found or you not access delete account", ServiceCode = ServiceCode.UserNotAccess };

            var accountStatusGroup = await _accountStatusGroupRepository.GetAll().SingleOrDefaultAsync(x => x.AccountId == deletedStatusAccountId && x.IdGroup == groupId);

            if (accountStatusGroup is null || deletedStatusAccountId == creatorId)
            {
                return new StandardResponse<bool>()
                {
                    Message = $"Account in group not exists",
                    ServiceCode = ServiceCode.EntityNotFound
                };
            }

            var Result = _accountStatusGroupRepository.Delete(accountStatusGroup);
            await _accountStatusGroupRepository.SaveAsync();

            return new StandardResponse<bool>()
            {
                Data = Result,
                ServiceCode = ServiceCode.AccountStatusGroupDeleted
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
                return new StandardResponse<AccountStatusGroup?>
                {
                    Message = "Group not found or Account not found",
                    ServiceCode = ServiceCode.EntityNotFound
                };
            }
            else if (accountStatusGroup.Group.GroupTasks is null)
            {
                return new StandardResponse<AccountStatusGroup?>
                {
                    Message = "Tasks not found",
                    ServiceCode = ServiceCode.EntityNotFound
                };
            }

            return new StandardResponse<AccountStatusGroup?>
            {
                Data = accountStatusGroup,
                ServiceCode = ServiceCode.AccountStatusGroupReadied
            };
        }

        public BaseResponse<IQueryable<AccountStatusGroup>> GetAccountStatusGroupOData()
        {
            var accountsStatusGroups = _accountStatusGroupRepository.GetAll();

            return new StandardResponse<IQueryable<AccountStatusGroup>>()
            {
                Data = accountsStatusGroups,
                ServiceCode = ServiceCode.AccountStatusGroupReadied
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

                return new StandardResponse<Profile[]>
                {
                    Data = usersInGroup.ToArray(),
                    ServiceCode = ServiceCode.AccountStatusWithGroupMapped,
                };
            }
            return new StandardResponse<Profile[]>()
            {
                Message = "Group not found",
                ServiceCode = ServiceCode.EntityNotFound,
            };
        }

        public async Task<BaseResponse<ResponseTasksPage>> GetTasksPage(List<GroupTask> groupTasks, User[] usersLoginWithId, bool isAdmin)
        { 
            List<ResponseStageGroupTaskIcon> stageGroupTask = new List<ResponseStageGroupTaskIcon>();
            List<ResponseGroupTaskView> tasksPages = new List<ResponseGroupTaskView>();
            foreach (var task in groupTasks)
            {
                stageGroupTask = await _stageGroupTaskRepository.GetAll().Where(x => x.IdGroupTask == task.Id).Select(x => new ResponseStageGroupTaskIcon(x.Id,x.Name,x.IdGroupTask)).ToListAsync();

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
                return new StandardResponse<ResponseTasksPage>
                {
                    Message = "Tasks not found",
                    ServiceCode = ServiceCode.EntityNotFound
                };
            }

            var tasksPageDTO = new ResponseTasksPage(tasksPages.ToArray(), isAdmin);

            return new StandardResponse<ResponseTasksPage>
            {
                Data = tasksPageDTO,
                ServiceCode = ServiceCode.GroupTaskReadied
            };
        }

        public async Task<BaseResponse<AccountStatusGroupDTO>> UpdateAccountStatusGroup(AccountStatusGroupDTO accountStatusGroupDTO, Guid createrId)
        {
            if (!await _groupRepository.GetAll().AnyAsync(x => x.CreaterId == createrId && x.Id == accountStatusGroupDTO.GroupId))
                return new StandardResponse<AccountStatusGroupDTO> { Message = "Account not found or you not access update account", ServiceCode = ServiceCode.UserNotAccess };

            var accountStatus = await _accountStatusGroupRepository.GetAll().SingleOrDefaultAsync(x => x.IdGroup == accountStatusGroupDTO.GroupId && x.AccountId == accountStatusGroupDTO.AccountId);
            if (accountStatus == null || accountStatusGroupDTO.AccountId == createrId) 
            {
                return new StandardResponse<AccountStatusGroupDTO>()
                {
                    Message = "Account not found",
                    ServiceCode = ServiceCode.EntityNotFound
                };
            }

            accountStatus.RoleAccount = accountStatusGroupDTO.RoleAccount;
            var updatedAccountStatusGroup = _accountStatusGroupRepository.Update(accountStatus);
            var result = await _accountStatusGroupRepository.SaveAsync();

            return new StandardResponse<AccountStatusGroupDTO>()
            {
                Data = new AccountStatusGroupDTO(updatedAccountStatusGroup),
                ServiceCode = ServiceCode.AccountStatusGroupUpdated
            };
            
        }
    }
}
