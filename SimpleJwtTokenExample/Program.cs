
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using SecurityAlgorithms = System.IdentityModel.Tokens.SecurityAlgorithms;
using SecurityToken = System.IdentityModel.Tokens.SecurityToken;
using SecurityTokenDescriptor = System.IdentityModel.Tokens.SecurityTokenDescriptor;
using SigningCredentials = System.IdentityModel.Tokens.SigningCredentials;
using SymmetricSecurityKey = System.IdentityModel.Tokens.SymmetricSecurityKey;

namespace ConsoleApplication4
{
    class Program
    {
        static void Main(string[] args)
        {
            var plainTextSecurityKey = "This is my shared, not so secret, secret!";

            byte[] bytePlainTextSecurityKey = Encoding.UTF8.GetBytes(plainTextSecurityKey);

            Microsoft.IdentityModel.Tokens.SecurityKey signingKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(bytePlainTextSecurityKey); //new InMemorySymmetricSecurityKey(Encoding.UTF8.GetBytes(plainTextSecurityKey));
            Microsoft.IdentityModel.Tokens.SigningCredentials signingCredentials = new Microsoft.IdentityModel.Tokens.SigningCredentials(
                signingKey, SecurityAlgorithms.HmacSha256Signature);

            var claimsIdentity = new ClaimsIdentity(new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, "myemail@myprovider.com"),
                new Claim(ClaimTypes.Role, "Administrator"),
            }, "Custom");

            Microsoft.IdentityModel.Tokens.SecurityTokenDescriptor securityTokenDescriptor = new Microsoft.IdentityModel.Tokens.SecurityTokenDescriptor()
            {
                Audience = "http://my.website.com",
                Issuer = "http://my.tokenissuer.com",
                Subject = claimsIdentity,
                SigningCredentials = signingCredentials,
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var plainToken = tokenHandler.CreateToken(securityTokenDescriptor);
            var signedAndEncodedToken = tokenHandler.WriteToken(plainToken);

            Console.WriteLine(plainToken.ToString());
            Console.WriteLine(signedAndEncodedToken);
            Console.ReadLine();

            var tokenValidationParameters = new TokenValidationParameters()
            {
                ValidAudiences = new string[]
                {
                    "http://my.website.com",
                    "http://my.otherwebsite.com"
                },
                ValidIssuers = new string[]
                {
                    "http://my.tokenissuer.com",
                    "http://my.othertokenissuer.com"
                },
                IssuerSigningKey = signingKey
            };
            //string token, TokenValidationParameters validationParameters, out Microsoft.IdentityModel.Tokens.SecurityToken validatedToken
            Microsoft.IdentityModel.Tokens.SecurityToken validatedToken;
            tokenHandler.ValidateToken(signedAndEncodedToken,
                tokenValidationParameters, out validatedToken);

            Console.WriteLine(validatedToken.ToString());
            Console.ReadLine();
        }
    }
}
