using FluentValidation;
using FriendBook.GroupService.API.Domain.DTO.GroupTaskDTOs;

namespace FriendBook.GroupService.API.Domain.Validators.GroupTaskDTOValidators
{
    public class ValidatorRequestGroupTaskChanged : AbstractValidator<RequestGroupTaskChanged>
    {
        public ValidatorRequestGroupTaskChanged()
        {
            RuleFor(dto => dto.GroupId)
                .NotEmpty().WithMessage("GroupId is required.");

            RuleFor(dto => dto.OldName)
                .Length(2, 50).WithMessage("OldName must be between 2 and 50 characters.")
                .NotEmpty().WithMessage("OldName is required.");

            RuleFor(dto => dto.NewName)
                .Length(2, 50).WithMessage("NewName must be between 2 and 50 characters.")
                .NotEmpty().WithMessage("NewName is required.");

            RuleFor(dto => dto.Description)
                .Length(0, 100).WithMessage("Description must be between 0 and 100 characters.");

            DateTime currentDate = DateTime.Now.AddDays(-1);
            RuleFor(dto => dto.DateEndWork)
                .GreaterThan(currentDate).WithMessage("The end of the work should be more than today's date")
                .LessThan(new DateTime(currentDate.Year + 50, currentDate.Month, currentDate.Day)).WithMessage("The end of the work should be more than today's date")
                .NotEmpty().WithMessage("DateEndWork is required.");

            RuleFor(dto => dto.Status)
                .IsInEnum().WithMessage("Invalid Status value.");
        }
    }
}
