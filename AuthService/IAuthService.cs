using AuthService.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService
{
    public interface IAuthService
    {
        Task<JwtTokenDto> GetJwtTokenAsync(AuthUserDto authDto, CancellationToken cancellationToken);
        Task<JwtTokenDto> GetJwtTokenByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken);
    }
}
