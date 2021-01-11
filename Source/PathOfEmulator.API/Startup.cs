using System;
using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using PathOfEmulator.API.Config;
using PathOfEmulator.API.Middleware;
using PathOfEmulator.API.Swagger;

namespace PathOfEmulator.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var config = Configuration.Get<PathOfEmulatorConfig>();
            services.AddSingleton(config);

            services.AddControllers();

            services.AddAuthorization(auth =>
            {
                auth.AddPolicy("ProfileScope", policy => policy.RequireClaim("SCOPE", "profile"));
                auth.AddPolicy("FilterScope", policy => policy.RequireClaim("SCOPE", "item_filter"));
            });

            services.AddSwaggerGen(gen =>
            {
                gen.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Path of Emulator API",
                    Description = "Simple emulator for PathOfExile.com API, including OAuth support",
                    TermsOfService = new Uri("https://raw.githubusercontent.com/SteffenBlake/PathOfEmulator/main/LICENSE"),
                    License = new OpenApiLicense
                    {
                        Name = "Use under MIT License",
                        Url = new Uri("https://raw.githubusercontent.com/SteffenBlake/PathOfEmulator/main/LICENSE")
                    },
                    Contact = new OpenApiContact
                    {
                        Name = "Steffen Blake",
                        Email = "steffen.cole.blake@gmail.com",
                        Url = new Uri("https://github.com/SteffenBlake/PathOfEmulator"),
                    },
                });

                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                gen.IncludeXmlComments(xmlPath);

                // Auto append "access_token" property to authorize queries
                gen.OperationFilter<AuthAccessTokenOperationFilter>();
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseDeveloperExceptionPage();

            app.UseHttpsRedirection();

            app.UseSwagger();

            app.UseSwaggerUI(swagger =>
            {
                swagger.SwaggerEndpoint("/swagger/v1/swagger.json", "Path of Emulator API");
                swagger.RoutePrefix = "";
            });

            app.UseStaticFiles();

            app.UseRouting();

            app.UseMiddleware<OAuthTokenMiddleWare>();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
