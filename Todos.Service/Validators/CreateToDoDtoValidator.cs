using FluentValidation;
using Todos.Service.Dto;

namespace Todos.Service.Validators
{
    public class CreateToDoValidator : AbstractValidator<CreateToDoDto>
    {
        public CreateToDoValidator()
        {
            RuleFor(x => x.Label).Length(3, 200);
            RuleFor(x => x.Label).NotEmpty();
            RuleFor(x => x).NotNull();
        }
    }
}
