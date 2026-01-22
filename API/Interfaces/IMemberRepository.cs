using System;
using API.Entities;
using API.DTOs;

namespace API.Interfaces;

public interface IMemberRepository
{
	void Update(Member member);
	Task<bool> SaveAllAsync();
	Task<Member?> GetMemberByIdAsync(string id);
	Task<IReadOnlyList<Member>> GetMembersAsync();
	Task<IReadOnlyList<Photo>> GetPhotosForMemberAsync(string memberId);
	Task<Member?> GetMemberForUpdate(string id);
	
	// Members that return a Task,we typicall use Async in their name to remind us that we have to await the response of this method.
}
