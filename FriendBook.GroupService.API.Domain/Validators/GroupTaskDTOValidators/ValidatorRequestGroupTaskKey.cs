using FluentValidation;
using FriendBook.GroupService.API.Domain.DTO.GroupTaskDTOs;

namespace FriendBook.GroupService.API.Domain.Validators.GroupTaskDTOValidators
{
    public class ValidatorRequestGroupTaskKey : AbstractValidator<RequestGroupTaskKey>
    {
        public ValidatorRequestGroupTaskKey()
        {
            RuleFor(dto => dto.GroupId).NotEmpty();

            RuleFor(dto => dto.Name).NotEmpty()
                                    .Length(2, 50);
        }
    }
}
