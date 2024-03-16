using FluentValidation;
using Todos.Service.Dto;

namespace Todos.Service.Validators
{
    public class UpdateToDoDtoValidator : AbstractValidator<UpdateToDoDto>
    {
        public UpdateToDoDtoValidator() 
        {
            RuleFor(x => x.Label).Length(3, 200);
            RuleFor(x => x.Label).NotEmpty();
            RuleFor(x => x).NotNull();
        }
    }
}
