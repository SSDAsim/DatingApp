using System;
using Microsoft.EntityFrameworkCore;
using API.Interfaces;
using API.Entities;
using API.DTOs;

namespace API.Data;

public class MemberRepository(AppDbContext context) : IMemberRepository
{
	public async Task<Member?> GetMemberByIdAsync(string id)
	{
		return await context.Members.FindAsync(id);
		// context.Members => 'Members' is the name of the table
	}

	public async Task<IReadOnlyList<Member>> GetMembersAsync()
	{
		return await context.Members.ToListAsync();
	}

	public async Task<IReadOnlyList<Photo>> GetPhotosForMemberAsync(string memberId)
	{
		return await context.Members
			.Where(x => x.Id == memberId)
			.SelectMany(x => x.Photos)
			.ToListAsync();

		// this is going to list not the members but list of photos of a particular member
	}

	public async Task<bool> SaveAllAsync()
	{
		return await context.SaveChangesAsync() > 0;

		// SaveChangesAsync() is going to return the number of changes made to the database. 
		// if we update the record with the same values we had previously then technically no change is going to happen and this method will return false / Bad request. To avoid that, we have implemented the following Update() method
	}

	public void Update(Member member)
	{
		context.Entry(member).State = EntityState.Modified;
		// this is going to set the state of the entry of the member to modified

		// this tells the EF Core that the member object has been changed, so mark it modified so when the 'SaveChangesAsync()' run, udpate the record in the database.
	}
}
