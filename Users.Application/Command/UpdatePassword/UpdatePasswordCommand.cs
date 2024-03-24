using MediatR;

namespace Users.Application.Command.UpdatePassword
{
    public class UpdatePasswordCommand : IRequest
    {
        public int Id { get; set; }
        public string Password { get; set; } = default!;
    }
}
