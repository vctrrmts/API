using Auth.Application.Dtos;
using MediatR;

namespace Auth.Application.Command.CreateJwtTokenByRefreshToken
{
    public class CreateJwtTokenByRefreshTokenCommand : IRequest<JwtTokenDto>
    {
        public string RefreshToken { get; set; } = default!;
    }
}
