using FluentValidation;

namespace Auth.Application.Command.CreateJwtTokenByRefreshToken
{
    public class CreateJwtTokenByRefreshTokenCommandValidator : AbstractValidator<CreateJwtTokenByRefreshTokenCommand>
    {
        public CreateJwtTokenByRefreshTokenCommandValidator() 
        {
            RuleFor(r => r.RefreshToken).NotEmpty();
        }
    }
}
