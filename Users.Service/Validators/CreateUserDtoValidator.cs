using FluentValidation;
using Users.Service.Dto;

namespace Users.Service.Validators
{
    public class CreateUserDtoValidator : AbstractValidator<CreateUserDto>
    {
        public CreateUserDtoValidator()
        {
            RuleFor(x => x.Login).Length(3, 50).NotEmpty();
            RuleFor(x => x.Password).Length(8,50).NotEmpty();
        }
    }
}
