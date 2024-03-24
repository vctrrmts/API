using FluentValidation;

namespace Todos.Application.Command.DeleteTodo
{
    public class DeleteTodoCommandValidator : AbstractValidator<DeleteTodoCommand>
    {
        public DeleteTodoCommandValidator() 
        {
            RuleFor(x=>x.Id).GreaterThan(0);
        }
    }
}
