using CS.Authentication.Contracts.Requests;
using CS.Authentication.Contracts.Responses;
using CS.Authentication.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CS.Authentication.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly JwtSettings _jwtSettings;

        public IdentityService(UserManager<IdentityUser> userManager, JwtSettings jwtSettings)
        {
            _userManager = userManager;
            _jwtSettings = jwtSettings;
        }

        public async Task<bool> CheckPasswordAsync(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return false;
            return await _userManager.CheckPasswordAsync(user, password);
        }

        public ClaimsPrincipal GenerateClaimsPrincipal(string email)
        {
            var claimsIdentity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, email),
            }, "Bearer");

            return new ClaimsPrincipal(claimsIdentity);
        }

        public async Task<AuthenticationResponse> RegisterAsync(UserRegistrationRequest request)
        {
            if (await _userManager.FindByEmailAsync(request.Email) != null)
            {
                return new AuthenticationResponse
                {
                    Errors = new List<string>() { "User with this email already exists" }
                };
            }

            var newUser = await _userManager.CreateAsync(new IdentityUser
            {
                UserName = request.Email,
                Email = request.Email,
            },
            request.Password);
            if (!newUser.Succeeded)
            {
                return new AuthenticationResponse
                {
                    Errors = newUser.Errors.Select(x => x.Description),
                    Success = false
                };
            }
            return GetAuthResponse(request.Email);
        }

        private AuthenticationResponse GetAuthResponse(string username)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, username),
                }),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII
                    .GetBytes(_jwtSettings.Secret)), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return new AuthenticationResponse
            {
                Success = true,
                Token = tokenString
            };
        }

        public async Task<AuthenticationResponse> LoginAsync(UserLoginRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                return new AuthenticationResponse
                {
                    Success = false,
                    Errors = new[] { "User doesn't exist" }
                };
            }

            var validPassword = await _userManager.CheckPasswordAsync(user, request.Password);

            if(!validPassword)
            {
                return new AuthenticationResponse
                {
                    Success = false,
                    Errors = new[] { "User/password combination is wrong" }
                };
            }

            return GetAuthResponse(request.Email);
        }
    }
}
