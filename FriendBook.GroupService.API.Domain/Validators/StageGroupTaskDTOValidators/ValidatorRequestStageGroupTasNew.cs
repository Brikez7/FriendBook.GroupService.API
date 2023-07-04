using FluentValidation;
using FriendBook.GroupService.API.Domain.DTO.DocumentGroupTaskDTOs;

namespace FriendBook.GroupService.API.Domain.Validators.StageGroupTaskDTOValidators
{
    public class ValidatorRequestStageGroupTasNew : AbstractValidator<RequestStageGroupTasNew>
    {
        public ValidatorRequestStageGroupTasNew() 
        {
            RuleFor(dto => dto.Name).NotEmpty()
                                    .NotNull()
                                    .Length(2, 50);

            RuleFor(dto => dto.IdGroupTask).NotEmpty();
        }
    }
}
