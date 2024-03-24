using FluentValidation;

namespace Todos.Application.Command.UpdateTodo
{
    public class UpdateTodoCommandValidator : AbstractValidator<UpdateTodoCommand>
    {
        public UpdateTodoCommandValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0);
            RuleFor(x => x.Label).Length(3, 200);
            RuleFor(x => x.Label).NotEmpty();
            RuleFor(x => x).NotNull();
        }
    }
}
