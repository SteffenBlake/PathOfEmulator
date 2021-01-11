using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using PathOfEmulator.API.Config;
using PathOfEmulator.API.Controllers;
using PathOfEmulator.API.Models;

namespace PathOfEmulator.API.Middleware
{
    public class OAuthTokenMiddleWare : IMiddleware
    {
        private PathOfEmulatorConfig Config { get; }

        public OAuthTokenMiddleWare(PathOfEmulatorConfig config)
        {
            Config = config ?? throw new ArgumentNullException(nameof(config));
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            if (TryAuthorize(context))
            {
                // Auth successful
                await next(context);
                return;
            }

            // Unauthorized and trying to access API
            context.Response.Clear();
            context.Response.StatusCode = (int) HttpStatusCode.Unauthorized;
            await context.Response.WriteAsync(JsonSerializer.Serialize(ErrorResponse.InvalidToken()));
        }

        private bool TryAuthorize(HttpContext context)
        {
            try
            {
                var token = context.Request.Query["access_token"].First();

                var user = Config.Data.Users.SingleOrDefault(u => 
                    u.Claims.Any(c => 
                        c.Tokens.Any(t => 
                            t.Value == token && (t.Expires == null || t.Expires > DateTime.Now)
                        )
                    )
                );

                var scopes = user?.Claims.SingleOrDefault(c =>
                        c.Tokens.Any(t => t.Value == token && (t.Expires == null || t.Expires > DateTime.Now))
                    )?.Scopes;

                if (scopes == null)
                    return false;

                // Identity Principal
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Name),
                    new Claim(ClaimTypes.NameIdentifier, user.Name),
                };

                claims.AddRange(scopes.Select(scope => new Claim("SCOPE", scope.ToLower())));

                var identity = new ClaimsIdentity(claims, "basic");
                context.User = new ClaimsPrincipal(identity);

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
