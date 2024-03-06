using FluentValidation;
using Users.Service.Dto;

namespace Users.Service.Validators
{
    public class CreateUserDtoValidator : AbstractValidator<CreateUserDto>
    {
        public CreateUserDtoValidator()
        {
            RuleFor(x => x.Name).Length(3, 50);
        }
    }
}
