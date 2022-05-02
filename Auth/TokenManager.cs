using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using TenantSearchAPI.Auth.Model;
using TenantSearchAPI.Data;
using TenantSearchAPI.Data.DTOs.Auth;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TenantSearchAPI;

namespace RelicsAPI.Auth
{
    public interface ITokenManager
    {
        Task<AuthResult> CreateAccessTokenAsync(User user);
        Task<AuthResult> VerifyAndGenerateToken(TokenRequest tokenRequest);
    }

    public class TokenManager : ITokenManager
    {
        private readonly SymmetricSecurityKey _authSigningKey;
        private readonly UserManager<User> _userManager;
        private readonly string _issuer;
        private readonly string _audience;
        private readonly TokenValidationParameters _tokenValidationParams;
        private readonly TenantSearchContext _context;

        public TokenManager(IConfiguration configuration, UserManager<User> userManager, TokenValidationParameters tokenValidationParams, TenantSearchContext context)
        {
            _authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Secret"]));
            _userManager = userManager;
            _issuer = configuration["JWT:ValidIssuer"];
            _audience = configuration["JWT:ValidAudience"];
            _tokenValidationParams = tokenValidationParams;
            _context = context;
        }

        public async Task<AuthResult> CreateAccessTokenAsync(User user)
        {
            var userRoles = await _userManager.GetRolesAsync(user);

            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(CustomClaims.UserId, user.Id.ToString()),
            };

            authClaims.AddRange(userRoles.Select(userRole => new Claim("roles", userRole)));

            var accessSecurityToken = new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                expires: DateTime.UtcNow.AddMinutes(120), // 5-10 mins
                claims: authClaims,
                signingCredentials: new SigningCredentials(_authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            var jwtToken = new JwtSecurityTokenHandler().WriteToken(accessSecurityToken);

            var refreshToken = new RefreshToken
            {
                JwtId = accessSecurityToken.Id,
                IsUsed = false,
                IsRevoked = false,
                UserId = user.Id,
                AddedDate = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddMonths(6),
                Token = RandomString(35) + Guid.NewGuid()
            };

            await _context.RefreshTokens.AddAsync(refreshToken);
            await _context.SaveChangesAsync();

            return new AuthResult()
            {
                Token = jwtToken,
                Success = true,
                RefreshToken = refreshToken.Token
            };
        }

        public async Task<AuthResult> VerifyAndGenerateToken(TokenRequest tokenRequest)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();

            try
            {
                // validation 1 - validate JWT token format
                var tokenInVerification = jwtTokenHandler.ValidateToken(tokenRequest.Token, _tokenValidationParams, out var validatedToken);

                // validation 2 - validate encryption alg
                if(validatedToken is JwtSecurityToken jwtSecurityToken)
                {
                    var result = jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCulture);

                    if (!result)
                        return null;
                }

                // validation 3 - validate expiry date
                var utcExpiryDate = long.Parse(tokenInVerification.Claims.FirstOrDefault(t => t.Type == JwtRegisteredClaimNames.Exp).Value);

                var expiryDate = DateTime.UnixEpoch.AddSeconds(utcExpiryDate).ToUniversalTime();

                if (expiryDate > DateTime.UtcNow)
                {
                    return new AuthResult
                    {
                        Success = false,
                        Errors = new List<string>()
                        { 
                            "Token has not yet expired"
                        }
                    };
                }

                // validation 4 - validate existance of token
                var storedToken = await _context.RefreshTokens.FirstOrDefaultAsync(r => r.Token == tokenRequest.RefreshToken);

                if(storedToken == null)
                {
                    return new AuthResult
                    {
                        Success = false,
                        Errors = new List<string>()
                        {
                            "Token does not exist"
                        }
                    };
                }

                // validation 5 - validate if it used
                if(storedToken.IsUsed)
                {
                    return new AuthResult
                    {
                        Success = false,
                        Errors = new List<string>()
                        {
                            "Token has been used"
                        }
                    };
                }

                // validation 6 - validate if revoked
                if(storedToken.IsRevoked)
                {
                    return new AuthResult
                    {
                        Success = false,
                        Errors = new List<string>()
                        {
                            "Token has been revoked"
                        }
                    };
                }

                // validation 7 - validate id
                var jti = tokenInVerification.Claims.FirstOrDefault(r => r.Type == JwtRegisteredClaimNames.Jti).Value;

                if(storedToken.JwtId != jti)
                {
                    return new AuthResult
                    {
                        Success = false,
                        Errors = new List<string>()
                        {
                            "Token does not match"
                        }
                    };
                }

                // update current token
                storedToken.IsUsed = true;
                _context.RefreshTokens.Update(storedToken);
                await _context.SaveChangesAsync();

                // generate new token
                var dbUser = await _userManager.FindByIdAsync(storedToken.UserId);

                return await CreateAccessTokenAsync(dbUser);
            }
            catch (Exception) // do i need this
            {
                return null;
            }
        }

        private string RandomString(int length)
        {
            var random = new Random();
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

            return new string(Enumerable.Repeat(chars, length).Select(x => x[random.Next(x.Length)]).ToArray());
        }
    }
}
