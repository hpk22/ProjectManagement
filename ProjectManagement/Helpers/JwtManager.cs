// 1. JwtManager.cs - Utility class to generate JWT
using System;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace ProjectManagement.Security
{
    public static class JwtManager
    {
        public static string GenerateToken(string username, int userId, int roleId, int expireMinutes = 60)
        {
            var key = ConfigurationManager.AppSettings["JwtSecret"];
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, username),
                new Claim("UserID", userId.ToString()),
                new Claim(ClaimTypes.Role, roleId.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: "ProjectManagementAPI",
                audience: "ProjectManagementClients",
                claims: claims,
                expires: DateTime.Now.AddMinutes(expireMinutes),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
