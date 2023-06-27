using FluentValidation;
using FriendBook.GroupService.API.Domain.DTO.GroupTaskDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriendBook.GroupService.API.Domain.Validators.GroupTaskDTOValidators
{
    public class ValidatorRequestGroupTaskNew : AbstractValidator<RequestGroupTaskNew>
    {
        public ValidatorRequestGroupTaskNew()
        {
            RuleFor(dto => dto.GroupId)
                .NotEmpty().WithMessage("GroupId is required.");

            RuleFor(dto => dto.Name)
                .NotEmpty().WithMessage("Name is required.")
                .Length(2, 50).WithMessage("Name must be between 2 and 50 characters.");

            RuleFor(dto => dto.Description)
                .Length(0, 100).WithMessage("Description must be between 0 and 100 characters.");

            var date = DateTime.Now.AddDays(-1);
            RuleFor(dto => dto.DateEndWork)
                .NotEmpty().WithMessage("DateEndWork is required.")
                .GreaterThan(date).WithMessage("DateEndWork must be a future date.");
        }
    }
}



