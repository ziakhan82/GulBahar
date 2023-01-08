using GulBaharWeb_Client.Service.Iservice;
using Microsoft.AspNetCore.Components;

namespace GulBaharWeb_Client.Pages.Authentication
{
    public partial class Logout
    {
        [Inject]
        public IAuthenticationService _authSerivce { get; set; }
        [Inject]
        public NavigationManager _navigationManager { get; set; }

        protected async override Task OnInitializedAsync()
        {
            await _authSerivce.LogOut();
            _navigationManager.NavigateTo("/",forceLoad:true);
        }
    }
}
