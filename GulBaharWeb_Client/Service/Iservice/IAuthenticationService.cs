using GulBahar_Models_Lib;

namespace GulBaharWeb_Client.Service.Iservice
{
    public interface IAuthenticationService
    {
       Task<SignUpResponseDTO> RegisterUser(SignUpRequestDTO signUpRequestDTO);
       Task<SignInResponseDTO> Login(SignInRequestDTO signInRequestDTO);
       Task LogOut();
    }
}
