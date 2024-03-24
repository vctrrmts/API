using MediatR;
using Users.Application.Dtos;

namespace Users.Application.Command.CreateUser
{
    public class CreateUserCommand : IRequest<GetUserDto>
    {
        public string Login { get; set; } = default!;
        public string Password { get; set; } = default!;
    }
}
