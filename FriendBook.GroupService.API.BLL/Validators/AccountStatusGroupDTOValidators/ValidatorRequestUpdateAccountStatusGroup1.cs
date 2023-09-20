using FluentValidation;
using FriendBook.GroupService.API.Domain.DTO.AccountStatusGroupDTOs;

namespace FriendBook.GroupService.API.Domain.Validators.AccountStatusGroupDTOValidators
{
    public class ValidatorRequestUpdateAccountStatusGroup1 : AbstractValidator<RequestUpdateAccountStatusGroup>
    {
        public ValidatorRequestUpdateAccountStatusGroup1()
        {
            RuleFor(dto => dto.Id).NotEmpty();

            RuleFor(dto => dto.RoleAccount).NotEmpty()
                                           .IsInEnum();
        }
    }
}
