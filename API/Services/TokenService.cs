using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.Entities;
using API.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace API.Services;

public class TokenService(IConfiguration config) : ITokenService
{
    public string CreateToken(AppUser user)
    {
        var tokenKey = config["TokenKey"] ?? throw new Exception("Cannot get Token Key"); // TokenKey is provided by the Server 

        if(tokenKey.Length < 64)
        {
            throw new Exception("Your token key needs to be at least 64 characters long");
        }
        
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey));

        var claims = new List<Claim>
        {
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.NameIdentifier, user.Id)
        };

        // sign the token 
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddDays(7), // usually the expiry is 10 mins and it is refreshed again and again
            SigningCredentials = creds,
        };

        var tokenHandler = new JwtSecurityTokenHandler(); 
        // this is the class which is going to create our token based on what we have created above.

        var token = tokenHandler.CreateToken(tokenDescriptor);
        
        return tokenHandler.WriteToken(token);
    }

    // from NUGET, install System.IdentityModel.Tokens.JWT @Microsoft and Microsoft.IdentityModel.Tokens @Microsoft
    // SymmetricSecurityKey => same key for encrypting the token and for decripting the token (since the key is stored on server and is not going to leave the server)
    // A Claim is something users claim about themselves. You can add Custom Claim Types
}
