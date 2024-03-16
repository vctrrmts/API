using AuthService.Dto;
using Common.Api.Exceptions;
using Common.Api.Utils;
using Common.Domain;
using Common.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AuthService
{
    public class AuthService : IAuthService
    {
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<UserRole> _userRoleRepository;
        private readonly IRepository<RefreshToken> _refreshTokenRepository;
        private readonly IConfiguration _configuration;

        public AuthService(
            IRepository<User> userRepository, 
            IRepository<UserRole> userRoleRepository,
            IRepository<RefreshToken> refreshTokenRepository,
            IConfiguration configuration)
        {
            _userRepository = userRepository;
            _userRoleRepository = userRoleRepository;
            _refreshTokenRepository = refreshTokenRepository;
            _configuration = configuration;
        }

        public async Task<JwtTokenDto> GetJwtTokenAsync(AuthUserDto authDto, CancellationToken cancellationToken)
        {
            var user = await _userRepository.SingleOrDefaultAsync(x => x.Login == authDto.Login.Trim(), cancellationToken);
            if (user is null)
            {
                throw new NotFoundException($"User with login {authDto.Login} doesn't exist");
            }

            if (!PasswordHashUtil.VerifyPassword(authDto.Password, user.PasswordHash))
            {
                throw new ForbiddenException(); 
            }

            var role = await _userRoleRepository.SingleAsync(r => r.Id == user.UserRoleId, cancellationToken);

            var claims = new List<Claim>
            {
                new (ClaimTypes.Name, authDto.Login ),
                new (ClaimTypes.NameIdentifier, user.Id.ToString()),
                new (ClaimTypes.Role, role.Name)
            };

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);
            var dateExpires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Jwt:ExpiresMinutes"]));
            var tokenDescriptor = new JwtSecurityToken(_configuration["Jwt:Issuer"]!, _configuration["Jwt:Audience"]!
                , claims, expires: dateExpires, signingCredentials: credentials);
            var token = new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
            var refreshToken = await _refreshTokenRepository.AddAsync(new RefreshToken() { UserId = user.Id }, cancellationToken);

            return new JwtTokenDto
            {
                JwtToken = token,
                RefreshToken = refreshToken.Id,
                Expires = dateExpires
            };
        }

        public async Task<JwtTokenDto> GetJwtTokenByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken)
        {
            var refreshTokenFromDB = await _refreshTokenRepository.SingleOrDefaultAsync(x => x.Id == refreshToken, cancellationToken);
            if (refreshTokenFromDB is null) 
            {
                throw new ForbiddenException();
            }

            var user = await _userRepository.SingleAsync(x => x.Id == refreshTokenFromDB.UserId, cancellationToken);
            var role = await _userRoleRepository.SingleAsync(r => r.Id == user.UserRoleId, cancellationToken);

            var claims = new List<Claim>
            {
                new (ClaimTypes.Name, user.Login),
                new (ClaimTypes.NameIdentifier, user.Id.ToString()),
                new (ClaimTypes.Role, role.Name)
            };

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);
            var dateExpires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Jwt:ExpiresMinutes"]));
            var tokenDescriptor = new JwtSecurityToken(_configuration["Jwt:Issuer"]!, _configuration["Jwt:Audience"]!
                , claims, expires: dateExpires, signingCredentials: credentials);
            var token = new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);

            return new JwtTokenDto
            {
                JwtToken = token,
                RefreshToken = refreshTokenFromDB.Id,
                Expires = dateExpires
            };
        }
    }
}
