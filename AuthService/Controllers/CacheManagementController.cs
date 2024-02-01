using AccessManager.Models.Enum;
using AccessManager.Sso.Attributes;
using AuthService.Providers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace AuthService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CacheManagementController : ControllerBase
    {
        private readonly ILogger<CacheManagementController> _logger;
        private readonly UserCacheProvider _provider;

        public CacheManagementController(ILogger<CacheManagementController> logger,
            UserCacheProvider provider)
        {
            _logger = logger;
            _provider = provider;
        }

        [HttpGet]
        [AuthorizeCustom(UserRole.Admin)]
        public async Task<IActionResult> Get()
        {
            _logger.LogInformation($"Request for Get cache");

            try
            {
                var cache = await _provider.GetCache();

                _logger.LogInformation($"Succesfull getting cache");

                return new ObjectResult(cache);
            }
            catch (Exception e)
            {
                _logger.LogError($"Error during getting cache");

                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }

        [HttpDelete]
        [AuthorizeCustom(UserRole.Admin)]
        public async Task<IActionResult> Delete()
        {
            _logger.LogInformation($"Request for reset cache");

            try
            {
                await _provider.ResetCache();

                _logger.LogInformation($"Succesfull reseting cache");

                return StatusCode(StatusCodes.Status200OK);
            }
            catch (Exception e)
            {
                _logger.LogError($"Error during reseting cache");

                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }
    }
}
