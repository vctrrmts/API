using FluentValidation;
using Todos.Service.Dto;

namespace Todos.Service.Validators
{
    public class CreateToDoValidator : AbstractValidator<CreateToDoDto>
    {
        public CreateToDoValidator()
        {
            RuleFor(x => x.OwnerId).GreaterThan(0).WithMessage("Owner Id Error");
            RuleFor(x => x.Label).Length(3, 200);
            RuleFor(x => x).NotNull();
        }
    }
}
