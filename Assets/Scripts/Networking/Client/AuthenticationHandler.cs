using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

public static class AuthenticationHandler
{
    public static AuthenticationState authState { get; private set; } = AuthenticationState.notAuthenticated;
    public static async Task<AuthenticationState> DoAuth(int maxTries = 5)
    {
        if (authState == AuthenticationState.authenticated) return authState;
        
        if (authState == AuthenticationState.authenticating)
        {
            Debug.LogWarning($"Already Authenticating");
            await Authenticating();
            return authState;
        }
        await SignInAnonymously(maxTries);
        return authState; 
    }
    private static async Task<AuthenticationState> Authenticating()
    {
        while (authState == AuthenticationState.authenticating || authState==AuthenticationState.notAuthenticated)
        {
            await Task.Delay(200);
        }
        return authState;
    }
    private static async Task SignInAnonymously(int maxTries)
    {
        int retries = 0;
        authState = AuthenticationState.authenticating;
        while (authState == AuthenticationState.authenticating && retries < maxTries)
        {
            try
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
                if (AuthenticationService.Instance.IsSignedIn && AuthenticationService.Instance.IsAuthorized)
                {
                    authState = AuthenticationState.authenticated;
                    break;
                }
                
            }
            catch(AuthenticationException ex)
            {
                authState = AuthenticationState.Error;
            }
            catch (RequestFailedException ex1)
            {
                authState = AuthenticationState.Error;
            }
            retries++;
            await Task.Delay(1000);
        }
        if (authState != AuthenticationState.authenticated)
        {
            MyDebug.Log($"Failed to signin After {retries} retries");
            authState = AuthenticationState.Timeout;
        }
    }
}
public enum AuthenticationState
{
    notAuthenticated,
    authenticating,
    authenticated,
    Error,
    Timeout
}
