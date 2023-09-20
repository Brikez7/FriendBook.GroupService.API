using FluentValidation;
using FriendBook.GroupService.API.Domain.DTO.GroupTaskDTOs;
using NodaTime.Extensions;

namespace FriendBook.GroupService.API.Domain.Validators.GroupTaskDTOValidators
{
    public class ValidatorRequestGroupTaskNew : AbstractValidator<RequestNewGroupTask>
    {
        public ValidatorRequestGroupTaskNew()
        {
            RuleFor(dto => dto.GroupId).NotEmpty();

            RuleFor(dto => dto.Name).NotEmpty()
                                    .Length(2, 50);

            RuleFor(dto => dto.Description).Length(0, 100);

            var date = DateTimeOffset.Now.AddDays(-1).ToOffsetDateTime().Date.ToDateOnly();
            RuleFor(dto => dto.DateEndWork.Date.ToDateOnly()).NotEmpty()
                                           .GreaterThan(date);
        }
    }
}



