using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hyme.Application.Commands.Projects
{
    public class UpdateProjectInfoCommandValidator : AbstractValidator<UpdateProjectInfoCommand>
    {
        public UpdateProjectInfoCommandValidator()
        {
            RuleFor(u => u.Title).MinimumLength(100);
            RuleFor(u => u.ShortDescription).MaximumLength(1000);
            RuleFor(u => u.ProjectDescription).MaximumLength(5000);
        }
    }
}
