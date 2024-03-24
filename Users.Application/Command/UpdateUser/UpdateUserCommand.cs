using MediatR;
using Users.Application.Dtos;

namespace Users.Application.Command.UpdateUser
{
    public class UpdateUserCommand : IRequest<GetUserDto>
    {
        public int Id { get; set; }
        public string Login { get; set; } = default!;
    }
}
