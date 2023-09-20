﻿using FluentValidation;
using FriendBook.GroupService.API.Domain.DTO.GroupTaskDTOs;
using NodaTime.Extensions;

namespace FriendBook.GroupService.API.Domain.Validators.GroupTaskDTOValidators
{
    public class ValidatorRequestUpdateGroupTask : AbstractValidator<UpdateGroupTaskDTO>
    {
        public ValidatorRequestUpdateGroupTask()
        {
            RuleFor(dto => dto.Id).NotEmpty();

            RuleFor(dto => dto.Name).Length(2, 50)
                                       .NotEmpty();

            RuleFor(dto => dto.Description).Length(0, 100);

            var currentDate = DateTimeOffset.UtcNow.AddDays(-1).ToOffsetDateTime().Date.ToDateOnly();
            var MaxDate = new DateTimeOffset(new DateTime(currentDate.Year + 50, currentDate.Month, currentDate.Day)).ToOffsetDateTime().Date.ToDateOnly();
            RuleFor(dto => dto.DateEndWork.Date.ToDateOnly()).GreaterThan(currentDate)
                                               .LessThan(MaxDate);

            RuleFor(dto => dto.Status).IsInEnum();
        }
    }
}
