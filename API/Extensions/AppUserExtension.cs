using System;
using API.DTOs;
using API.Entities;
using API.Interfaces;

namespace API.Extensions
{
    public static class AppUserExtension
    {
        public async static Task<UserDto> ToDto(this AppUser user, ITokenService tokenService)
        {
            return new UserDto
            {
                Id = user.Id,
                DisplayName = user.DisplayName,
                Email = user.Email!,
                ImageUrl = user.ImageUrl,
                Token = await tokenService.CreateToken(user)
            };
        }
    }
}
