using GulBaharWeb_Client.Service.Iservice;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace GulBaharWeb_Client.Pages.Authentication
{
    public partial class RedirectToLogin
    {
        [CascadingParameter]
        public Task<AuthenticationState> _authState { get; set; }

        [Inject]
        NavigationManager _navigationManger { get; set; }

        bool notAuthorized { get; set; } = false;

        protected override async Task OnInitializedAsync()
        {
            var authState = await _authState;
            if (authState?.User.Identities is null || !authState.User.Identity.IsAuthenticated)
            {
                var returnUrl = _navigationManger.ToBaseRelativePath(_navigationManger.Uri);
                if (string.IsNullOrEmpty(returnUrl))
                {
                    _navigationManger.NavigateTo("login");
                }
                else
                {
                    _navigationManger.NavigateTo($"login?returnUrl={returnUrl}");
                }
            }
            else
            {
                notAuthorized = true;
            }

            
        }


    }
}
