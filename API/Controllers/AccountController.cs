
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class AccountController(UserManager<AppUser> userManager, ITokenService tokenService) : BaseApiController
{
    [HttpPost("register")] // api/account/register
    // public async Task<ActionResult<AppUser>> Register (string email, string displayName, string password)
    public async Task<ActionResult<UserDto>> Register (RegisterDto registerDto)
    {
        var user = new AppUser
        {
            DisplayName = registerDto.DisplayName,
            Email = registerDto.Email,
            UserName = registerDto.Email,
            // Register the member info too
            Member = new Member
            {
                DisplayName = registerDto.DisplayName,
                Gender = registerDto.Gender!,
                City = registerDto.City!,
                Country = registerDto.Country,
                DateOfBirth = registerDto.DateOfBirth
            }
        };

        var result = await userManager.CreateAsync(user, registerDto.Password);

        // show validation errors
        if(!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("identity", error.Description);
            }
            // ModelState basically tells you whether the incoming data was correctly mapped to your model.
            // it tells you whether the model satisfies all the validation rules.

            return ValidationProblem();
        }

        // assign role
        var roleResult = await userManager.AddToRoleAsync(user, "Member");

        // setToken
        await SetRefereshTokenCookies(user);

        return await user.ToDto(tokenService);
    }

    // check login credentials of the user 
    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
    {
        // get user using email.
        var user = await userManager.FindByEmailAsync(loginDto.Email);

        if (user == null) return Unauthorized("Invalid Email Address");

        // check if the password is correct by Identity
        var result = await userManager.CheckPasswordAsync(user, loginDto.Password);

        if (!result) return Unauthorized("Invalid Email");

        await SetRefereshTokenCookies(user);

        return await user.ToDto(tokenService);
    }

    [HttpPost("refresh-token")]
    public async Task<ActionResult<UserDto>> RefreshToken()
    {
        var refreshToken = Request.Cookies["refreshToken"];
        if (refreshToken == null) return NoContent();

        var user = await userManager.Users.FirstOrDefaultAsync(x => x.RefreshToken == refreshToken && x.RefreshTokenExpiry > DateTime.UtcNow);

        if (user == null) return Unauthorized();

        await SetRefereshTokenCookies(user);

        return await user.ToDto(tokenService);
    }

    public async Task SetRefereshTokenCookies(AppUser user)
    {
        var refreshToken = tokenService.GenerateRefreshToken();
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
        await userManager.UpdateAsync(user);

        // configure cookie option 
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true, // means this cookie is not accessible by any javascript, not even from our client 
            Secure = true, // the cookie will be sent only on HTTPS connections
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddDays(7)
        };

        Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
    }
}
