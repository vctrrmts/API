using FluentValidation;

namespace Users.Application.Command.DeleteUser
{
    public class DeleteUserCommandValidator : AbstractValidator<DeleteUserCommand>
    {
        public DeleteUserCommandValidator() 
        {
            RuleFor(u => u.Id).GreaterThan(0);
        }
    }
}
