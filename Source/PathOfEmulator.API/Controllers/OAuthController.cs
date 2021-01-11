using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using PathOfEmulator.API.Config;

namespace PathOfEmulator.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OAuthController : ControllerBase
    {
        private PathOfEmulatorConfig Config { get; }

        public OAuthController(PathOfEmulatorConfig config)
        {
            Config = config ?? throw new ArgumentNullException(nameof(config));
        }

        /// <summary>
        /// OAuth 2.0 Endpoint used to prompt the user to login
        /// </summary>
        /// <param name="client_id">Client Id for your Application, Provided by GGG in production, set in appsettings.json here</param>
        /// <param name="response_type">Should always be code</param>
        /// <param name="scope">Space delimited set of Scopes your app needs to access. Known scopes are "profile" and "item_filter"</param>
        /// <param name="redirect_uri">Url your application listens on for the user to be redirected to with the "code" query param</param>
        /// <param name="state">Persistent state to be passed as a query param "state" back to your redirect URL</param>
        /// <returns>302 Redirect to the 'redirect_uri' uri provided, with "scope" and "state" params provided</returns>
        [HttpGet("Authorize")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status302Found)]
        public IActionResult Authorize(string client_id, string response_type, string scope, string redirect_uri, string state)
        {
            if (Config.Client.Id != client_id)
            {
                return BadRequest(new {error = "invalid_client", error_description = "The client id supplied is invalid" });
            }

            if (response_type != "code")
            {
                return BadRequest(new { error = "response_type", error_description = "The response type supplied is invalid" });
            }

            var scopes = scope.Split(" ");
            if (scopes.Length < 1)
            {
                return BadRequest(new { error = "scope", error_description = "The scope supplied is invalid" });
            }

            var accessCode = Guid.NewGuid().ToString().Remove('-');

            Config.GetActiveUser().Claims.Add(new ClaimConfig
            {
                Code = accessCode,
                Scopes = scopes
            });

            // Build up query params to append to the provided redirect Uri
            var queryParams = new Dictionary<string, string>
            {
                {"code", accessCode},
            };

            if (!string.IsNullOrEmpty(state))
            {
                queryParams["state"] = state;
            }

            var finalRedirectUri = QueryHelpers.AddQueryString(redirect_uri, queryParams);

            return RedirectToRoute(finalRedirectUri);
        }


        /// <summary>
        /// Endpoint to generate (Temporary? Permanent? Requires confirmation by GGG) Access Tokens for API usage
        /// </summary>
        /// <param name="client_id">Client Id for your Application, Provided by GGG in production, set in appsettings.json here</param>
        /// <param name="client_secret">Client Secret for your Application, Provided by GGG in production, set in appsettings.json here</param>
        /// <param name="code">Access Code for the user (obtained via the Authorize Endpoint)</param>
        /// <param name="grant_type">Should always be "authorization_code"</param>
        /// <param name="redirect_uri">Url your application listens on for the user to be redirected to with the following query params</param>
        /// <returns>
        /// "access_token" - Access Token for user
        /// "scope" - space delimited scopes this access token applies to
        /// "expires_in" (optional) - Non expiring tokens wont have this param. Int value for how many seconds until the token expires.
        /// </returns>
        [HttpGet("Token")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status302Found)]
        public IActionResult Token(string client_id, string client_secret, string code, string grant_type, string redirect_uri)
        {
            if (Config.Client.Id != client_id)
            {
                return BadRequest(new { error = "invalid_client", error_description = "The client id supplied is invalid" });
            }
            if (Config.Client.Secret != client_secret)
            {
                return BadRequest(new { error = "client_secret", error_description = "The client secret supplied is invalid" });
            }
            if (grant_type != "authorization_code")
            {
                return BadRequest(new { error = "grant_type", error_description = "The grant type supplied is invalid" });
            }

            var user = Config.Data.Users.SingleOrDefault(u => u.Claims.Any(c => c.Code == code));
            if (user == null)
            {
                return BadRequest(new { error = "Code", error_description = "The code supplied is invalid" });
            }

            var claim = user.Claims.SingleOrDefault(c => c.Code == code);
            if (claim == null)
            {
                return BadRequest(new { error = "Code", error_description = "The code supplied is invalid" });
            }

            var token = new TokenConfig{Value = Guid.NewGuid().ToString().Remove('-') };
            claim.Tokens.Add(token);

            // Build up query params to append to the provided redirect Uri
            var queryParams = new Dictionary<string, string>
            {
                {"access_token", token.Value},
                {"scope", string.Join(" ", claim.Scopes)},
                {"token_type", "bearer"}
            };
            if (Config.OAuth.AccessTokenLifetimeSeconds > 0)
            {
                token.Expires = DateTime.Now.AddSeconds(Config.OAuth.AccessTokenLifetimeSeconds);
                queryParams["expires_in"] = Config.OAuth.AccessTokenLifetimeSeconds.ToString();
            }

            var finalRedirectUri = QueryHelpers.AddQueryString(redirect_uri, queryParams);

            return RedirectToRoute(finalRedirectUri);
        }

    }
}
