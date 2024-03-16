using FluentValidation;
using Users.Service.Dto;

namespace Users.Service.Validators
{
    public class UpdateUserDtoValidator : AbstractValidator<UpdateUserDto>
    {
        public UpdateUserDtoValidator()
        {
            RuleFor(x => x.Login).Length(3, 50).NotEmpty();
        }
    }
}
