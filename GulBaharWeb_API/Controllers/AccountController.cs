﻿using GulBahar_Common_Func_Lib;
using GulBahar_DataAcess_Lib;
using GulBahar_Models_Lib;
using GulBaharWeb_API.Helper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace GulBaharWeb_API.Controllers
{
    [Route("api/[controller]/[action]")] // action, becuase both sing in and sign up are http post method,refence the name with the rout
    [ApiController]
    public class AccountController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly APISettings _aPISettings;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            IOptions<APISettings> options)

        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _aPISettings = options.Value; // based on this insdie the api settings I will get all the values from appsetting.json

        }
        [HttpPost]
        public async Task<IActionResult> SignUp([FromBody] SignUpRequestDTO signUpRequestDTO)
        {
            if (signUpRequestDTO==null || !ModelState.IsValid)
            {
                return BadRequest();
            }
            //create user from signUpDTO
            var user = new ApplicationUser
            {
                UserName = signUpRequestDTO.Email,
                Email = signUpRequestDTO.Email,
                Name = signUpRequestDTO.Name,
                PhoneNumber = signUpRequestDTO.PhoneNumber,
                EmailConfirmed = true
            };
            // first parameter is the instance of application user, and second is the user pass
            var result = await _userManager.CreateAsync(user, signUpRequestDTO.Password);
            // if not successed 
            if (!result.Succeeded)
            {
                // create an object of response 
                return BadRequest(new SignUpResponseDTO()
                {
                    IsRegistrationSuccessful = false,
                    Errors = result.Errors.Select(u => u.Description)
                });
            }
            // if the result is successful assign roles to the user 
            var roleResult = await _userManager.AddToRoleAsync(user, SD.Role_Customer);
            if (!roleResult.Succeeded)
            {
                return BadRequest(new SignUpResponseDTO()
                {
                    IsRegistrationSuccessful = false,
                    Errors = result.Errors.Select(u => u.Description)
                });
            }

            return StatusCode(201); // created
        }

        [HttpPost]
        public async Task<IActionResult> SignIn([FromBody] SignInRequestDTO signInRequestDTO)
        {
            if (signInRequestDTO==null || !ModelState.IsValid)
            {
                return BadRequest();
            }
            // if the model state is valid call sign manager that i have with dependency injection
            var result = await _signInManager.PasswordSignInAsync(signInRequestDTO.UserName,signInRequestDTO.Password,false,false);
            if (result.Succeeded)
            {
                var user = await _userManager.FindByNameAsync(signInRequestDTO.UserName);
                if(user==null)
                {
                    return Unauthorized(new SignInResponseDTO
                    {
                        IsAuthSuccessful = false,
                        ErrorMessage = "Invalid authentication"
                    });
                }
                //everything is valid you can login/ once the login is successful

                var singedCredentials = GetSingingCredentials(); // get sing in credentials
                var claims = await GetClaims(user); // retreving all the claims 

                // creating JWT token
                var tokenOptions = new JwtSecurityToken(
                    issuer: _aPISettings.ValidIssuer,
                    audience: _aPISettings.ValidAudience,
                    claims: claims,
                    expires: DateTime.Now.AddDays(30),
                    signingCredentials:singedCredentials);

                // creating final token
                var token = new JwtSecurityTokenHandler().WriteToken(tokenOptions);
                return Ok(new SignInResponseDTO()
                {
                    IsAuthSuccessful = true,
                    Token = token,
                    UserDTO = new UserDTO //assing the properties based on the user object
                    {
                        Name = user.Name,
                        Id = user.Id,
                        Email = user.Email,
                        PhoneNumber = user.PhoneNumber,
                    }
                    });

            }
            else
            {
                return Unauthorized(new SignInResponseDTO
                {
                    IsAuthSuccessful = false,
                    ErrorMessage = "Invalid authentication"
                });
            }

            return StatusCode(201); // created
        }

    private SigningCredentials GetSingingCredentials()
        {
            // provide the key inside an array of byte. in order to convert 
            var secret = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_aPISettings.SecretKey));

            // passing the secure key and the algorithm
            return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
        }

        // returns a list of claim 
        private async Task<List<Claim>> GetClaims(ApplicationUser user)
        {
            // new list of claim
            var claims = new List<Claim>
            {
                // type and value
                new Claim(ClaimTypes.Name,user.Email), //default
                new Claim(ClaimTypes.Email,user.Email), // defult  
                new Claim("Id",user.Id), // custom claim

            };
            // find user pass that to get role by async and inside role user get the role that is assinged from Db
            var userRoles= await _userManager.GetRolesAsync(await _userManager.FindByEmailAsync(user.Email));


            foreach (var role in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role)); // adding claim of role to the user
            }
            return claims;
        }
    }
}