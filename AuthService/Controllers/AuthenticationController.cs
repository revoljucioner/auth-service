using AccessManager.Models.Requests.Auth;
using AuthService.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace AuthService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthenticationController : ControllerBase
    {
        private readonly ILogger<UserManagementController> _logger;
        private readonly ILoginProvider _loginProvider;

        public AuthenticationController(ILogger<UserManagementController> logger,
            ILoginProvider loginProvider)
        {
            _logger = logger;
            _loginProvider = loginProvider;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> LoginAsync(LoginRequest request)
        {
            try
            {
                _logger.LogInformation($"Success login by: '{request.Email}'");

                var token = await _loginProvider.Login(request);
                return Ok(token);
            }
            catch (UnauthorizedAccessException e)
            {
                _logger.LogError($"Error during login: '{e.Message}'");

                return Unauthorized();
            }
            catch (Exception e)
            {
                _logger.LogError($"Error during login: '{e.Message}'");

                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }
    }
}
