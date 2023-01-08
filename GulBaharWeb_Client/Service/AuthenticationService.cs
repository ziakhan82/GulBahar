using Blazored.LocalStorage;
using GulBahar_Common_Func_Lib;
using GulBahar_Models_Lib;
using GulBaharWeb_Client.Service.Iservice;
using Microsoft.AspNetCore.Components.Authorization;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace GulBaharWeb_Client.Service
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorageService;
        private readonly AuthenticationStateProvider _authenticationStateProvider;

        public AuthenticationService(HttpClient httpClient, ILocalStorageService localStorageService, AuthenticationStateProvider authenticationStateProvider)
        {
            _httpClient = httpClient;
            _localStorageService = localStorageService;
            _authenticationStateProvider = authenticationStateProvider;
        }

        public async Task<SignInResponseDTO> Login(SignInRequestDTO signInRequest)
        {
            var content =JsonConvert.SerializeObject(signInRequest);
            var bodycontent = new StringContent(content, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("api/account/signin", bodycontent);
            var contentTemp = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<SignInResponseDTO>(contentTemp);

            if (response.IsSuccessStatusCode)
            {
                await _localStorageService.SetItemAsync(SD.Local_Token, result.Token);
                await _localStorageService.SetItemAsync(SD.Local_UserDetails, result.UserDTO);
                ((AuthStateProvider)_authenticationStateProvider).NotifyUserLoggedIn(result.Token);
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", result.Token);
                return new SignInResponseDTO() { IsAuthSuccessful = true };
            }
            else
            {

                return result;

            }
            }

        public async Task LogOut()
        {
            await _localStorageService.RemoveItemAsync(SD.Local_Token);
            await _localStorageService.RemoveItemAsync(SD.Local_UserDetails);

            ((AuthStateProvider)_authenticationStateProvider).NotifyUserLogout();

            _httpClient.DefaultRequestHeaders.Authorization = null;
        }

        public async Task<SignUpResponseDTO>RegisterUser(SignUpRequestDTO signUpRequestDTO)
        {
            var content = JsonConvert.SerializeObject(signUpRequestDTO);
            var bodycontent = new StringContent(content, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("api/account/signup", bodycontent);
            var contentTemp = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<SignUpResponseDTO>(contentTemp);

            if (response.IsSuccessStatusCode)
            {
               return new SignUpResponseDTO { IsRegistrationSuccessful= true };
            }
            else
            {
                Debug.WriteLine(response);

                return new SignUpResponseDTO { IsRegistrationSuccessful = false,Errors=result.Errors};
                

            }
        }
    }
}
