using System;
using System.Text;
using System.Web.Http;
using Microsoft.Owin;
using Microsoft.Owin.Security.Jwt;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.DataHandler.Encoder;
using Microsoft.Owin.Security.DataProtection;
using Microsoft.Owin.Security.OAuth;
using Owin;
using Microsoft.IdentityModel.Tokens;

[assembly: OwinStartup(typeof(ProjectManagement.Startup))]  // 👈 required

namespace ProjectManagement
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var issuer = "ProjectManagementAPI";
            var audience = "ProjectManagementClients";
            var secret = System.Configuration.ConfigurationManager.AppSettings["JwtSecret"];

            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));

            // Configure JWT Bearer Authentication
            app.UseJwtBearerAuthentication(new JwtBearerAuthenticationOptions
            {
                AuthenticationMode = AuthenticationMode.Active,
                TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = issuer,

                    ValidateAudience = true,
                    ValidAudience = audience,

                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = signingKey,

                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromMinutes(5)
                }
            });

            // Web API setup
            HttpConfiguration config = new HttpConfiguration();
            WebApiConfig.Register(config);
            app.UseWebApi(config);
        }
    }
}
