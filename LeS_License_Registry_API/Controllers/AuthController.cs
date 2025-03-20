using LeS_License_Registry_API.Data;
using LeS_License_Registry_API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace LeS_License_Registry_API.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly TokenService _tokenService;
        private readonly ILogger<AuthController> _logger;
        private readonly LesLicenseRegistryContext _dataAccess;
        public AuthController(TokenService tokenService, ILogger<AuthController> logger,LesLicenseRegistryContext context)
        {
            _tokenService = tokenService;
            _logger = logger;
            _dataAccess = context;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            API_Response response = new API_Response();
            response.isSuccess =false;
            try
            {
                if(!string.IsNullOrEmpty(request.Username)&&!string.IsNullOrWhiteSpace(request.Username)&& !string.IsNullOrEmpty(request.Password) && !string.IsNullOrWhiteSpace(request.Password))
                {

                    var user = _dataAccess.les_license_control_users?.Where(x => x.username.ToLower()==request.Username.ToLower()).FirstOrDefault();
                    if (user != null)
                    {
                        if (!LeS.Core.HashPassword.VerifyPassword(request.Password ?? "".ToLower(), user.hashed_password ?? "", user.hash_salt ?? ""))
                        {
                            response.isSuccess = false;
                            response.message = "Password is incorrect!";
                            

                        }
                        else
                        {
                            var token = _tokenService.GenerateToken(request.Username);
                            response.isSuccess = true;
                            response.message = "User successfully logged in!";
                            response.data = token;
                           

                        }
                    }
                    else
                    {
                        response.isSuccess = false;
                        response.message = "No such user found!";

                    }
                }
                else
                {
                    response.isSuccess=false;
                    response.message = "Please enter valid username and password!";
                }
               
            }
            catch( Exception ex)
            {
                _logger.LogError(ex, "Error occurred while log in user !");
                response.isSuccess = false;
                response.message = "Internal server error!";
            }
            return Ok(response);

            
        }
    }

    // Login Request Model
    public class LoginRequest
    {
        public string? Username { get; set; }
        public string? Password { get; set; }
    }
}
