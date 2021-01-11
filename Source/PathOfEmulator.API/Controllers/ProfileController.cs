using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PathOfEmulator.API.Config;
using PathOfEmulator.API.Extensions;
using PathOfEmulator.API.Models;

namespace PathOfEmulator.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProfileController : ControllerBase
    {
        private PathOfEmulatorConfig Config { get; }
        public ProfileController(PathOfEmulatorConfig config)
        {
            Config = config ?? throw new ArgumentNullException(nameof(config));
        }

        /// <summary>
        /// Gets the profile for the Authorized user. Requires Authorization for scope "profile"
        /// </summary>
        [HttpGet("")]
        [Authorize(Policy = "ProfileScope")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserProfile))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ErrorResponse))]
        public UserProfile Get()
        {
            var userName = User.GetLoggedInUserId();
            var userConfig = Config.Data.Users.First(u => u.Name == userName);

            return UserProfile.FromConfig(userConfig);
        }
    }
}
