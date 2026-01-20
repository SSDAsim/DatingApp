using System;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using API.DTOs;
using System.Security.Cryptography;
using API.Entities;
using System.Text;


namespace API.Data;

public class Seed
{
	public static async Task SeedUsers(AppDbContext context)
	{
		// this method will be run whenever the app is going to be started, so we apply a check if the 'users' table already has some data, do not run this method.

		if (await context.Users.AnyAsync()) return;

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
            // we need to calculate the hash so
            using var hmac = new HMACSHA512();

            var user = new AppUser
			{
				Id = member.Id,
				Email = member.Email,
				DisplayName = member.DisplayName,
				ImageUrl = member.ImageUrl,
				PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("Pa$$w0rd")),
				PasswordSalt = hmac.Key,
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

			context.Users.Add(user); // this is going to track user in the memory
		}

		// Save changes to the database 
		await context.SaveChangesAsync();
	}
}
