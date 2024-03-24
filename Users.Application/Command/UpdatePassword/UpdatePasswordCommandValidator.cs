using FluentValidation;

namespace Users.Application.Command.UpdatePassword
{
    public class UpdatePasswordCommandValidator : AbstractValidator<UpdatePasswordCommand>
    {
        public UpdatePasswordCommandValidator() 
        {
            RuleFor(p => p.Id).GreaterThan(0);
            RuleFor(p => p.Password).Length(8, 50).NotEmpty();
        }
    }
}
