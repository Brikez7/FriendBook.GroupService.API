using FluentValidation;
using FriendBook.GroupService.API.Domain.DTO.AccountStatusGroupDTOs;
using FriendBook.GroupService.API.Domain.Entities;

namespace FriendBook.GroupService.API.Domain.Validators.AccountStatusGroupDTOValidators
{
    public class ValidatorAccountStatusGroupDTO : AbstractValidator<AccountStatusGroupDTO>
    {
        public ValidatorAccountStatusGroupDTO()
        {
            RuleFor(dto => dto.GroupId)
                .NotEmpty().WithMessage("IdGroup is required.");

            RuleFor(dto => dto.AccountId)
                .NotEmpty().WithMessage("AccountId is required.");

            RuleFor(dto => dto.RoleAccount)
                .NotEmpty().WithMessage("RoleAccount is required.")
                .IsInEnum().WithMessage("RoleAccount must be greater than or equal to Default and must be less than or equal to Admin.");
        }
    }
}
