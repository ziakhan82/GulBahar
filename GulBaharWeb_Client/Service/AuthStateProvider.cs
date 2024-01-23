using Blazored.LocalStorage;
using GulBahar_Common_Func_Lib;
using GulBaharWeb_Client.Helper;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace GulBaharWeb_Client.Service
{
    // based on the headers that i have in http client, set the default authorization to the token
    public class AuthStateProvider : AuthenticationStateProvider
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorage;

        public AuthStateProvider(HttpClient httpClient, ILocalStorageService localStorage)
        {
            _httpClient = httpClient;
            _localStorage = localStorage;
        }
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            // getting jwt token
            var token = await _localStorage.GetItemAsync<string>(SD.Local_Token);
            if (token==null)
            {
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())); // null identiy claim
             }
            // adding token gloably
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(JwtParser.ParseClaimsFromJwt(token), "jwtAuthType")));// from jwt toekn adding all the claims
        }
        // insted of force load
        public void NotifyUserLoggedIn(string token)
        {
            // creating authen user 
            var authenticatedUser = new ClaimsPrincipal(new ClaimsIdentity(JwtParser.ParseClaimsFromJwt(token), "jwtAuthType"));
            var authState = Task.FromResult(new AuthenticationState(authenticatedUser));
            NotifyAuthenticationStateChanged(authState);// raises authentication change event for auth state provider
        }

        public void NotifyUserLogout()
        {
            var authState = Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())));
            NotifyAuthenticationStateChanged(authState);// notify auth state with empty auth when user log out
        }
    }

}

