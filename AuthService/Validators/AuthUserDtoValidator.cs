using AuthService.Dto;
using FluentValidation;

namespace AuthService.Validators
{
    public class AuthUserDtoValidator : AbstractValidator<AuthUserDto>
    {
        public AuthUserDtoValidator()
        {
            RuleFor(x => x.Login).Length(3, 50).NotEmpty();
            RuleFor(x => x.Password).Length(8, 50).NotEmpty();
        }
    }
}
