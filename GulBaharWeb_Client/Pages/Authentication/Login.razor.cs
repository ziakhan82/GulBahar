using GulBahar_Models_Lib;
using GulBaharWeb_Client.Service.Iservice;
using Microsoft.AspNetCore.Components;
using System.Web;

namespace GulBaharWeb_Client.Pages.Authentication
{
    public partial class Login
    {
        private SignInRequestDTO SignInRequest = new();
        public bool IsProcessing { get; set; } = false;
        public bool ShowSignInErrors { get; set; }
        public string Errors { get; set; }
        public string returnUrl { get; set; }

        [Inject]
        public IAuthenticationService _authService { get; set; }

        [Inject]
        NavigationManager _navigationManger { get; set; }

        private async Task LoginUser()
        {
            ShowSignInErrors = false;
            IsProcessing = true;
            var result = await _authService.Login(SignInRequest);

            if (result.IsAuthSuccessful)
            {
                var absoluteURi = new Uri(_navigationManger.Uri);
                var qureryParam = HttpUtility.ParseQueryString(absoluteURi.Query);
                returnUrl = qureryParam["returnUrl"];

                if (string.IsNullOrEmpty(returnUrl)) { 
                _navigationManger.NavigateTo("/");
            }
            else
            {
                _navigationManger.NavigateTo("/" + returnUrl);
            }
        }
            else
            {
                Errors = result.ErrorMessage;
                ShowSignInErrors = true;
            }
            IsProcessing = false;

        }
    }
}

