using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using Humanizer;
using Microsoft.EntityFrameworkCore;
using System;

namespace API.Data;

public class MemberRepository(AppDbContext context) : IMemberRepository
{
	public async Task<Member?> GetMemberByIdAsync(string id)
	{
		return await context.Members.FindAsync(id);
		// context.Members => 'Members' is the name of the table
	}

    public async Task<Member?> GetMemberForUpdate(string id)
    {
		return await context.Members
			.Include(x => x.User)
			.Include(x => x.Photos)
			.SingleOrDefaultAsync(x => x.Id == id);

        // we could not include(x => x.User) with FindAsync(id) becuase special method that only retrieves the entity by its primary key and does not support loading related data.
        // SELECT * FROM Members JOIN Users ON Users.Id = Members.UserId WHERE Members.Id = @id
    }

    public async Task<PaginatedResult<Member>> GetMembersAsync(MemberParams memberParams)
	{
		var query = context.Members.AsQueryable();

		// return all members except the current member
		query = query.Where(x => x.Id != memberParams.CurrentMemberId);

		if(memberParams.Gender != null)
		{
			query = query.Where(x => x.Gender == memberParams.Gender);
		}

		var minDob = DateOnly.FromDateTime(DateTime.Today.AddYears(-memberParams.MaxAge - 1));
		var maxDob = DateOnly.FromDateTime(DateTime.Today.AddYears(-memberParams.MinAge));

		query = query.Where(x => x.DateOfBirth >= minDob && x.DateOfBirth <= maxDob);

		// last active 
		query = memberParams.OrderBy switch
		{
			"created" => query.OrderByDescending(x => x.Created),
			_ => query.OrderByDescending(x => x.LastActive)
		};

		return await PaginationHelper.CreateAsync(query, memberParams.PageNumber, memberParams.PageSize);
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
