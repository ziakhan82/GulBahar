using GulBahar_Models_Lib;
using GulBaharWeb_Client.Service.Iservice;
using Microsoft.AspNetCore.Components;
using System;

namespace GulBaharWeb_Client.Pages.Authentication
{
    public partial class Register
    {
        private SignUpRequestDTO SignUpRequest = new();
        public bool IsProcessing { get; set; } = false;
        public bool ShowRegistrationError { get; set; }
        public IEnumerable<string> Errors { get; set; }

            [Inject] 
           public IAuthenticationService _authService { get; set; }

            [Inject]
            NavigationManager _navigationManger { get; set; }       

        private async Task RegisterUser()
        {
            ShowRegistrationError = false;
            IsProcessing = true;
            var result = await _authService.RegisterUser(SignUpRequest);

            if (result.IsRegistrationSuccessful)
            {
                _navigationManger.NavigateTo("/login");
            }
            else
            {
                Errors = result.Errors;
                ShowRegistrationError = true;
            }
            IsProcessing = false;

        }
    }
}
