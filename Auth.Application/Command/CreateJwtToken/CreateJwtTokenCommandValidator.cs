using FluentValidation;

namespace Auth.Application.Command.CreateJwtToken
{
    public class CreateJwtTokenCommandValidator : AbstractValidator<CreateJwtTokenCommand>
    {
        public CreateJwtTokenCommandValidator() 
        {
            RuleFor(x => x.Login).Length(3, 50).NotEmpty();
            RuleFor(x => x.Password).Length(8, 50).NotEmpty();
        }
    }
}
