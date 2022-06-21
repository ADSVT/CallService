using CS.Authentication.Contracts.Requests;
using CS.Authentication.Contracts.Responses;
using System.Security.Claims;

namespace CS.Authentication.Services
{
    public interface IIdentityService
    {
        Task<AuthenticationResponse> RegisterAsync(UserRegistrationRequest request);
        Task<AuthenticationResponse> LoginAsync(UserLoginRequest request);
        ClaimsPrincipal GenerateClaimsPrincipal(string email);
        Task<bool> CheckPasswordAsync(string email, string password);
    }
}
