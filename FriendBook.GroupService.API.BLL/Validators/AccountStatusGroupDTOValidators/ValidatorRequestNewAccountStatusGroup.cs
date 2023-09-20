using FluentValidation;
using FriendBook.GroupService.API.Domain.DTO.AccountStatusGroupDTOs;

namespace FriendBook.GroupService.API.Domain.Validators.AccountStatusGroupDTOValidators
{
    public class ValidatorRequestNewAccountStatusGroup : AbstractValidator<RequestNewAccountStatusGroup>
    {
        public ValidatorRequestNewAccountStatusGroup()
        {
            RuleFor(dto => dto.GroupId).NotEmpty();

            RuleFor(dto => dto.AccountId).NotEmpty();

            RuleFor(dto => dto.RoleAccount).NotEmpty()
                                           .IsInEnum();
        }
    }
}
