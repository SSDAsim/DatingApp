using API.Data;
using API.Entities;
using API.Interfaces;
using API.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using API.Extensions;

namespace API.Controllers
{
    [Authorize]
    public class MembersController(IMemberRepository memberRepository) : BaseApiController
    {
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<Member>>> GetMembers()
        {
            return Ok(memberRepository.GetMembersAsync());
        }

        [HttpGet("{id}")]  // localhost:5001/api/members/{user-id}
        public async Task<ActionResult<Member>> GetMember(string id)
        {
            var member = await memberRepository.GetMemberByIdAsync(id);

            if(member == null) return NotFound();

            return member;
        }

        [HttpGet("{id}/photos")] //url/api/members/{user-id}/photos
        public async Task<ActionResult<IReadOnlyList<Photo>>> GetMemberPhotos(string id)
        {
            return Ok(await memberRepository.GetPhotosForMemberAsync(id));
        }

        // Update a user 
        [HttpPut]
        public async Task<ActionResult> UpdateMember(MemberUpdateDto memberUpdateDto)
        {
            // extract the id of the logged in user from the jwt token 
            var memberId = User.GetMemberId();

            var member = await memberRepository.GetMemberForUpdate(memberId);

            if (member == null) return BadRequest("Could not get member");

            // update the member 
            member.DisplayName = memberUpdateDto.DisplayName ?? member.DisplayName;
            member.Description = memberUpdateDto.Description ?? member.Description;
            member.City = memberUpdateDto.City ?? member.City;
            member.Country = memberUpdateDto.Country ?? member.Country;

            // update the related user name
            member.User.DisplayName = memberUpdateDto.DisplayName ?? member.DisplayName;

            memberRepository.Update(member); // optional - prevents BadRequest response user has sent a request with no updated values

            if (await memberRepository.SaveAllAsync()) return NoContent();
            // NoContent() sends a 204 status code. and since it is a put request we are not supposed to return any content

            return BadRequest("Failed to update member");

        }

    }
}
