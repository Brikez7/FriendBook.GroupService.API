using FluentValidation;
using FriendBook.GroupService.API.Domain.DTO.GroupTaskDTOs;

namespace FriendBook.GroupService.API.Domain.Validators.GroupTaskDTOValidators
{
    public class ValidatorRequestGroupTaskChanged : AbstractValidator<RequestGroupTaskChanged>
    {
        public ValidatorRequestGroupTaskChanged()
        {
            RuleFor(dto => dto.GroupId).NotEmpty();

            RuleFor(dto => dto.OldName).Length(2, 50)
                                       .NotEmpty();

            RuleFor(dto => dto.NewName).Length(2, 50)
                                       .NotEmpty();

            RuleFor(dto => dto.Description).Length(0, 100);

            DateTime currentDate = DateTime.Now.AddDays(-1);
            RuleFor(dto => dto.DateEndWork).GreaterThan(currentDate)
                                           .LessThan(new DateTime(currentDate.Year + 50, currentDate.Month, currentDate.Day))
                                           .NotEmpty();

            RuleFor(dto => dto.Status).IsInEnum();
        }
    }
}
