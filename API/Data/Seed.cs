using System;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using API.DTOs;
using System.Security.Cryptography;
using API.Entities;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Build.Evaluation;


namespace API.Data;

public class Seed
{
	public static async Task SeedUsers(UserManager<AppUser> userManager)
	{
		// this method will be run whenever the app is going to be started, so we apply a check if the 'users' table already has some data, do not run this method.

		if (await userManager.Users.AnyAsync()) return;

		// seed user data 
		// read from the data file
		var memberData = await File.ReadAllTextAsync("Data/UserSeedData.json");
		// the above method is going to give us a string. we need to deserialize string data
		// create a DTO and define the properties 
		var members = JsonSerializer.Deserialize<List<SeedUserDto>>(memberData);

		if (members == null)
		{
			Console.WriteLine("No members in seed data");
			return;
		}

		// map data from seed data file to the properties of our entities 
		foreach (var member in members)
		{
            var user = new AppUser
			{
				Id = member.Id,
				Email = member.Email,
				UserName = member.Email,
				DisplayName = member.DisplayName,
				ImageUrl = member.ImageUrl,
				Member = new Member
				{
					Id = member.Id,
					DisplayName = member.DisplayName,
					Description = member.Description,
					DateOfBirth = member.DateOfBirth,
					ImageUrl = member.ImageUrl,
					Gender = member.Gender,
					City = member.City,
					Country = member.Country,
					LastActive = member.LastActive,
					Created = member.Created,
				}
			};

			user.Member.Photos.Add(new Photo
			{
				Url = member.ImageUrl!,
				MemberId = member.Id,
			});

			var result = await userManager.CreateAsync(user, "Pa$$w0rd");
			if (!result.Succeeded)
			{
				Console.WriteLine(result.Errors.First().Description);
			}
			await userManager.AddToRoleAsync(user, "Member");
		}

		var admin = new AppUser
		{
			UserName = "admin@test.com",
			Email = "admin@test.com",
			DisplayName = "Admin"
		};

		await userManager.CreateAsync(admin, "Pa$$w0rd");
		await userManager.AddToRolesAsync(admin, ["Admin", "Moderator"]);
	}
}
