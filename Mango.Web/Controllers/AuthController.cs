using Mango.Web.Models;
using Mango.Web.Service.IService;
using Mango.Web.Utlility;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Mango.Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        private readonly ITokenProvider _tokenProvider;
        public AuthController(IAuthService authService, ITokenProvider tokenProvider)
        {
            this._authService = authService;
            this._tokenProvider = tokenProvider;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

     
        public async Task<IActionResult> Logout()
        {
           await HttpContext.SignOutAsync();
            _tokenProvider.ClearToken();
            return RedirectToAction("Index", "Home");

        }

        [HttpGet]
        public IActionResult Registration()
        {
            var roleList = new List<SelectListItem>()
            { new SelectListItem {Text=SD.RoleAdmin,Value =SD.RoleAdmin},
              new SelectListItem {Text=SD.RoleCustomer, Value=SD.RoleCustomer},

            };
            ViewBag.RoleList = roleList;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Registration(RegistrationRequestDto registrationRequestDto)
        {
            var response = await _authService.RegistrationAsync(registrationRequestDto);
            if(response != null && response.IsSuccess)
            {
                if(string.IsNullOrEmpty(registrationRequestDto.Role))
                {
                    registrationRequestDto.Role = SD.RoleCustomer;
                }
                var assignRole = await _authService.AssignRoleAsync(registrationRequestDto);
                if(assignRole != null && assignRole.IsSuccess) {
                    TempData["success"] = "Registration done successfully";
                    return RedirectToAction(nameof(Login));
                
                }
            
            }
            else
            {
                TempData["error"] = response.Message;
            }
            var roleList = new List<SelectListItem>()
            { new SelectListItem {Text=SD.RoleAdmin,Value =SD.RoleAdmin},
              new SelectListItem {Text=SD.RoleCustomer, Value=SD.RoleCustomer},

            };
            ViewBag.RoleList = roleList;

            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Login(LoginRequestDto loginRequestDto)
        {
            var result = await _authService.LoginAsync(loginRequestDto);
            if(result != null && result.IsSuccess)
            {

                var loginDetails = JsonConvert.DeserializeObject<LoginResponseDto>(Convert.ToString(result.Result));
              await  SignInUser(loginDetails);
                _tokenProvider.SetToken(loginDetails.Token);

              return  RedirectToAction("Index", "Home");
            }
            else
            {
                TempData["error"] = result.Message;
                ModelState.AddModelError("CustomError", result.Message);
                return View();
            }
           
        }


        private async Task SignInUser(LoginResponseDto response)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(response.Token);
            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Email, jwt.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Email).Value));
            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Sub, jwt.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Sub).Value));
            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Name, jwt.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Name).Value));
            identity.AddClaim(new Claim(ClaimTypes.Name, jwt.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Name).Value));
            identity.AddClaim(new Claim(ClaimTypes.Role, jwt.Claims.FirstOrDefault(x => x.Type == "role").Value));

            var principle = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principle);
        }
    }
}
