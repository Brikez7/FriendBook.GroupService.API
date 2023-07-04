using FluentValidation;
using FriendBook.GroupService.API.Domain.DTO.GroupDTOs;

namespace FriendBook.GroupService.API.Domain.Validators.GroupDTOValidators
{
    public class ValidatorGroupDTO : AbstractValidator<GroupDTO>
    {
        public ValidatorGroupDTO()
        {
            RuleFor(dto => dto.GroupId).NotEmpty();

            RuleFor(dto => dto.Name).NotEmpty()
                                    .Length(2, 50);
        }
    }
}
