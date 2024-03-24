using FluentValidation;

namespace Users.Application.Command.CreateUser
{
    public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
    {
        public CreateUserCommandValidator()
        {
            RuleFor(x => x.Login).Length(3, 50).NotEmpty();
            RuleFor(x => x.Password).Length(8, 50).NotEmpty();
        }
    }
}
