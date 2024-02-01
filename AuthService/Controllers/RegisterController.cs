using AccessManager.Models.Requests.Auth;
using AuthService.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace AuthService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RegisterController : ControllerBase
    {
        private readonly ILogger<RegisterController> _logger;
        private readonly IUserManagerProvider _provider;

        public RegisterController(ILogger<RegisterController> logger,
            IUserManagerProvider provider)
        {
            _logger = logger;
            _provider = provider;
        }

        [HttpPost("RegisterUser")]
        public async Task<IActionResult> RegisterNewCustomer(RegisterUserRequest request)
        {
            _logger.LogInformation($"Request for register the new user, id: {request.EmployeeId}'");

            try
            {
                var token = await _provider.Register(request);

                _logger.LogInformation($"Succesfully registered user '{request.EmployeeId}'");

                HttpContext.Response.Headers.Add("Authorization", token);

                return new ObjectResult(request.EmployeeId);
            }
            catch (ArgumentException e)
            {
                _logger.LogError($"Error during registering user: '{e.Message}'");

                return StatusCode(StatusCodes.Status400BadRequest, e.Message);
            }
            catch (AmbiguousMatchException e)
            {
                _logger.LogError($"Error during registering user: '{e.Message}'");

                return StatusCode(StatusCodes.Status409Conflict, e.Message);
            }
            catch (Exception e)
            {
                _logger.LogError($"Error during registering user: '{e.Message}'");

                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }
    }
}
