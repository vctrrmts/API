using Auth.Application.Dtos;
using Common.Application.Abstractions.Persistence;
using Common.Application.Exceptions;
using Common.Application.Utils;
using Common.Domain;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Auth.Application.Command.CreateJwtToken
{
    public class CreateJwtTokenCommandHandler : IRequestHandler<CreateJwtTokenCommand, JwtTokenDto>
    {
        private readonly IRepository<ApplicationUser> _users;
        private readonly IRepository<RefreshToken> _refreshTokens;
        private readonly IConfiguration _configuration;

        public CreateJwtTokenCommandHandler(
            IRepository<ApplicationUser> users,
            IRepository<RefreshToken> refreshTokens,
            IConfiguration configuration) 
        {
            _users = users;
            _refreshTokens = refreshTokens;
            _configuration = configuration;
        }

        public async Task<JwtTokenDto> Handle(CreateJwtTokenCommand request, CancellationToken cancellationToken)
        {
            var user = await _users.SingleOrDefaultAsync(x => x.Login == request.Login.Trim(), cancellationToken);
            if (user is null)
            {
                throw new NotFoundException($"User with login {request.Login} doesn't exist");
            }

            if (!PasswordHashUtil.VerifyPassword(request.Password, user.PasswordHash))
            {
                throw new ForbiddenException();
            }

            var claims = new List<Claim>
            {
                new (ClaimTypes.Name, request.Login ),
                new (ClaimTypes.NameIdentifier, user.Id.ToString()),
            };

            foreach (var role in user.Roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role.ApplicationUserRole.Name));
            }

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);
            var dateExpires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Jwt:ExpiresMinutes"]));
            var tokenDescriptor = new JwtSecurityToken(_configuration["Jwt:Issuer"]!, _configuration["Jwt:Audience"]!
                , claims, expires: dateExpires, signingCredentials: credentials);
            var token = new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
            var refreshToken = await _refreshTokens.AddAsync(new RefreshToken() { UserId = user.Id }, cancellationToken);

            return new JwtTokenDto
            {
                JwtToken = token,
                RefreshToken = refreshToken.Id,
                Expires = dateExpires
            };
        }
    }
}
