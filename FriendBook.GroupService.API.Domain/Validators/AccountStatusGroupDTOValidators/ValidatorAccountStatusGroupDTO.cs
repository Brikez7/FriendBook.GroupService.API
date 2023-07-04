using FluentValidation;
using FriendBook.GroupService.API.Domain.DTO.AccountStatusGroupDTOs;
using FriendBook.GroupService.API.Domain.Entities;

namespace FriendBook.GroupService.API.Domain.Validators.AccountStatusGroupDTOValidators
{
    public class ValidatorAccountStatusGroupDTO : AbstractValidator<AccountStatusGroupDTO>
    {
        public ValidatorAccountStatusGroupDTO()
        {
            RuleFor(dto => dto.GroupId).NotEmpty();

            RuleFor(dto => dto.AccountId).NotEmpty();

            RuleFor(dto => dto.RoleAccount).NotEmpty()
                                           .IsInEnum();
        }
    }
}
