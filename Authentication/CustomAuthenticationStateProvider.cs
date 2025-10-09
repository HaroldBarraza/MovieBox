using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace MovieBox.Authentication
{
    public class CustomAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly ProtectedSessionStorage _sessionStorage;
        private readonly string _userSessionKey = "UserSession";
        private ClaimsPrincipal _anonymous = new ClaimsPrincipal(new ClaimsIdentity());
        

        public CustomAuthenticationStateProvider(ProtectedSessionStorage sessionStorage)
        {
            _sessionStorage = sessionStorage;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                var userSessionResult = await _sessionStorage.GetAsync<UserSession>(_userSessionKey);
                var userSession = userSessionResult.Success ? userSessionResult.Value : null;

                if (userSession == null || string.IsNullOrEmpty(userSession.UserName))
                {
                    return new AuthenticationState(_anonymous);
                }

                var ClaimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, userSession.UserName),
                    new Claim(ClaimTypes.Email, userSession.Email ?? ""),
                    new Claim(ClaimTypes.Role, userSession.Role ?? "User"),
                    new Claim("UserId", userSession.UserId?.ToString() ?? "")
                }, "CustomAuth"));
                return await Task.FromResult(new AuthenticationState(ClaimsPrincipal));
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Error retrieving authentication state: {ex.Message}");
                return new AuthenticationState(_anonymous);
            }
        }

        public async Task UpdateAuthenticationState(UserSession userSession)
        {
            ClaimsPrincipal claimsPrincipal;

            if (userSession != null)
            {
                await _sessionStorage.SetAsync(_userSessionKey, userSession);
                claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
                {
                    new Claim(ClaimTypes.Name, userSession.UserName ?? ""),
                    new Claim(ClaimTypes.Email, userSession.Email ?? ""),
                    new Claim(ClaimTypes.Role, userSession.Role ?? "User"),
                    new Claim("UserId", userSession.UserId?.ToString() ?? "")
                }, "CustomAuth"));

            }
            else
            {
                await _sessionStorage.DeleteAsync(_userSessionKey);
                claimsPrincipal = _anonymous;
            }
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(claimsPrincipal)));
        }
    }
}