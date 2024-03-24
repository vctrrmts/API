using FluentValidation;
using Users.Application.Command.UpdateUser;

namespace Users.Service.Validators
{
    public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
    {
        public UpdateUserCommandValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0);
            RuleFor(x => x.Login).Length(3, 50).NotEmpty();
        }
    }
}
