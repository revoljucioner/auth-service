using AccessManager.Models.Enum;
using AccessManager.Sso.Attributes;
using AccessManager.Sso.Attributes.FromClaimAttribute;
using AuthService.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace AuthService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserManagementController : ControllerBase
    {
        private readonly ILogger<UserManagementController> _logger;
        private readonly IUserManagerProvider _managerProvider;

        public UserManagementController(ILogger<UserManagementController> logger,
            IUserManagerProvider managerProvider)
        {
            _logger = logger;
            _managerProvider = managerProvider;
        }

        [HttpPut("SetUserRole")]
        [AuthorizeCustom(UserRole.Admin)]
        public async Task<IActionResult> PutStatus([BindRequired] Guid userId, [BindRequired] UserRole newRole)
        {
            try
            {
                await _managerProvider.SetUserRole(userId, newRole);

                _logger.LogInformation($"Succesfuly updated role as '{newRole}' for user '{userId}'");

                return StatusCode(StatusCodes.Status200OK);
            }
            catch (ArgumentOutOfRangeException e)
            {
                _logger.LogError($"Cannot find user '{userId}' during updating role");

                return StatusCode(StatusCodes.Status404NotFound, e.Message);
            }
            catch (Exception e)
            {
                _logger.LogError($"Error during updating role for user '{userId}': '{e.Message}'");

                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }

        [HttpGet("UserInfo")]
        [AuthorizeCustom(UserRole.Admin)]
        public async Task<IActionResult> GetUserInfo([BindRequired] Guid userId)
        {
            try
            {
                var result = await _managerProvider.GetUserInfo(userId);

                _logger.LogInformation($"Succesfuly get user info for user '{userId}'");

                return StatusCode(StatusCodes.Status200OK);
            }
            catch (ArgumentOutOfRangeException e)
            {
                _logger.LogError($"Cannot find user '{userId}'");

                return StatusCode(StatusCodes.Status404NotFound, e.Message);
            }
            catch (Exception e)
            {
                _logger.LogError($"Error during getting info for user '{userId}': '{e.Message}'");

                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }

        [HttpGet("MyUserInfo")]
        [AuthorizeCustom(UserRole.Staff, UserRole.Admin)]
        public async Task<IActionResult> GetMyUserInfo([FromClaim("Id")] Guid userId)
        {
            try
            {
                var result = await _managerProvider.GetUserInfo(userId);

                _logger.LogInformation($"Succesfuly get user info for user '{userId}'");

                return StatusCode(StatusCodes.Status200OK);
            }
            catch (ArgumentOutOfRangeException e)
            {
                _logger.LogError($"Cannot find user '{userId}'");

                return StatusCode(StatusCodes.Status404NotFound, e.Message);
            }
            catch (Exception e)
            {
                _logger.LogError($"Error during getting info for user '{userId}': '{e.Message}'");

                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }
    }
}
