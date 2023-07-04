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
            RuleFor(dto => dto.GroupId).NotEmpty();

            RuleFor(dto => dto.Name).NotEmpty()
                                    .Length(2, 50);

            RuleFor(dto => dto.Description).Length(0, 100);

            var date = DateTime.Now.AddDays(-1);
            RuleFor(dto => dto.DateEndWork).NotEmpty()
                                           .GreaterThan(date);
        }
    }
}



