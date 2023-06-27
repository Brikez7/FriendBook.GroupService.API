using FluentValidation;
using FriendBook.GroupService.API.Domain.DTO.GroupDTOs;

namespace FriendBook.GroupService.API.Domain.Validators.GroupDTOValidators
{
    public class ValidatorGroupDTO : AbstractValidator<GroupDTO>
    {
        public ValidatorGroupDTO()
        {
            RuleFor(dto => dto.GroupId)
                .NotEmpty().WithMessage("GroupId is required.");

            RuleFor(dto => dto.Name)
                .NotEmpty().WithMessage("Name is required.")
                .Length(2, 50).WithMessage("Name must be between 2 and 50 characters.");
        }
    }
}
