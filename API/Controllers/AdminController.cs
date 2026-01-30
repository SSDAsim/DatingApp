using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AdminController(UserManager<AppUser> userManager) : BaseApiController
    {
        [Authorize(Policy = "RequireAdminRole")]
        [HttpGet("users-with-roles")]
        public async Task<ActionResult> GetUserWithRoles()
        {
            // get all users
            var users = await userManager.Users.ToListAsync();
            var userList = new List<object>();

            foreach (var user in users)
            {
                var roles = await userManager.GetRolesAsync(user);
                userList.Add(new
                {
                    user.Id,
                    user.Email,
                    Roles = roles.ToList()
                });
            }

            return Ok(userList);
        }

        // add only the provided roles to the user. also delete the roles that are not provided on the user
        [Authorize(Policy = "RequireAdminRole")]
        [HttpPost("edit-roles/{userId}")]
        public async Task<ActionResult<IList<string>>> EditRoles(string userId, [FromQuery]string roles)
        {
            // if roles are empty 
            if (string.IsNullOrEmpty(roles)) return BadRequest("You must select at least one role");

            var selectedRoles = roles.Split(",").ToArray();

            // get the user 
            var user = await userManager.FindByIdAsync(userId);

            if (user == null) return BadRequest("Could not retreive user");

            // get user roles 
            var userRoles = await userManager.GetRolesAsync(user);

            // assign roles to the user but only the ones that are not assigned already
            var result = await userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles));

            if (!result.Succeeded) return BadRequest("Failed to add to roles");

            // remove roles from the user that are not provided 
            result = await userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles));

            if (!result.Succeeded) return BadRequest("Failed to remove from roles");

            return Ok(await userManager.GetRolesAsync(user));
        }

        [Authorize(Policy = "ModeratePhotoRole")]
        [HttpGet("photos-to-moderate")]
        public ActionResult GetPhotosForModeration()
        {
            return Ok("Admins or Moderators can see this");
        }
    }
}
