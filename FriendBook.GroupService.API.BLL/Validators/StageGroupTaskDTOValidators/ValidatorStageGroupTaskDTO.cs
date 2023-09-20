using FluentValidation;
using FriendBook.GroupService.API.Domain.DTO.DocumentGroupTaskDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriendBook.GroupService.API.Domain.Validators.StageGroupTaskDTOValidators
{
    public class ValidatorStageGroupTaskDTO : AbstractValidator<UpdateStageGroupTaskDTO>
    {
        public ValidatorStageGroupTaskDTO() 
        {
            RuleFor(dto => dto.StageId).NotEmpty();

            RuleFor(dto => dto.Name).NotEmpty()
                                    .NotNull()
                                    .Length(2, 50);

            RuleFor(dto => dto.IdGroupTask).NotEmpty();

            RuleFor(dto => dto.Text).Length(0,300);
        }
    }
}
