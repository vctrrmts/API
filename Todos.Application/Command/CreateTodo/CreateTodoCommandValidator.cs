using FluentValidation;

namespace Todos.Application.Command.CreateTodo
{
    public class CreateTodoCommandValidator : AbstractValidator<CreateTodoCommand>
    {
        public CreateTodoCommandValidator() 
        {
            RuleFor(x => x.Label).Length(3, 200);
            RuleFor(x => x.Label).NotEmpty();
            RuleFor(x => x).NotNull();
        }
    }
}
