using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace MovieBox.Authentication
{
    public class CustomAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly ProtectedSessionStorage? _sessionStorage;
        private ClaimsPrincipal _anonymous = new ClaimsPrincipal(new ClaimsIdentity());

        public CustomAuthenticationStateProvider(ProtectedSessionStorage? sessionStorage)
        {
            _sessionStorage = sessionStorage;
        }
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            Console.WriteLine("GetAuthenticationStateAsync START----------------------------------------------d-----------------------------------------");
            try
            {
                var userSessionStorageResult = await _sessionStorage!.GetAsync<UserSession>("UserSession");
                var userSession = userSessionStorageResult.Success ? userSessionStorageResult.Value : null;
                if (userSession == null || string.IsNullOrEmpty(userSession.Username))
                {
                    Console.WriteLine("userSession is null");

                    return await Task.FromResult(new AuthenticationState(_anonymous));
                }
                var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, userSession.Id?.ToString() ?? string.Empty),
                    new Claim(ClaimTypes.Name, userSession.Username ?? string.Empty),
                    new Claim(ClaimTypes.Email, userSession.Email ?? string.Empty),
                    new Claim(ClaimTypes.Role, userSession.Role ?? "User")
                }, "CustomAuth"));
                Console.WriteLine($"Loaded UserSession: Id={userSession.Id}, Username={userSession.Username}, Email={userSession.Email}, Role={userSession.Role}");

                return await Task.FromResult(new AuthenticationState(claimsPrincipal));
            }
            catch   (Exception ex)
            {
                Console.WriteLine($"Error in GetAuthenticationStateAsync: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                
                return await Task.FromResult(new AuthenticationState(_anonymous));
            }
        }

        public async Task UpdateAuthenticationState(UserSession? userSession)
        {
            ClaimsPrincipal claimsPrincipal;

            if (userSession != null && !string.IsNullOrEmpty(userSession.Username))
            {
                await _sessionStorage!.SetAsync("UserSession", userSession);
                claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, userSession.Id?.ToString() ?? string.Empty),
                    new Claim(ClaimTypes.Name, userSession.Username),
                    new Claim(ClaimTypes.Email, userSession.Email ?? string.Empty),
                    new Claim(ClaimTypes.Role, userSession.Role ?? "User")
                }, "CustomAuth"));
                    Console.WriteLine($"Set UserSession: Id={userSession.Id}, Username={userSession.Username}, Email={userSession.Email}, Role={userSession.Role}");

            }
            else
            {
                await _sessionStorage!.DeleteAsync("UserSession");
                claimsPrincipal = _anonymous;
            }
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(claimsPrincipal)));
        }
    }
}