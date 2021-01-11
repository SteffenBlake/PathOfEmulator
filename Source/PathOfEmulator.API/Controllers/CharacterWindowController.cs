using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PathOfEmulator.API.Config;
using PathOfEmulator.API.Models;

namespace PathOfEmulator.API.Controllers
{
    [ApiController]
    [Route("character-window")]
    public class CharacterWindowController : ControllerBase
    {
        private PathOfEmulatorConfig Config { get; }

        public CharacterWindowController(PathOfEmulatorConfig config)
        {
            Config = config ?? throw new ArgumentNullException(nameof(config));
        }

        /// <summary>
        /// Gets all the characters for a user profile
        /// </summary>
        /// <param name="accountName">The Account Name for the user</param>
        /// <returns>tThe characters for said user</returns>
        [HttpGet("get-characters")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CharacterConfig[]))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorCodeResponse))]
        public IActionResult GetCharacters(string accountName)
        {
            var account = Config.Data.Users.SingleOrDefault(u => u.Name == accountName);
            if (account == null)
            {
                return NotFound(ErrorCodeResponse.NotFound());
            }

            return Ok(account.Characters);
        }
    }
}
