using API.Data;
using API.Extensions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace API.Helpers
{
    public class LogUserActivity : IAsyncActionFilter
    {
        // An Action filter runs before or after a controller action. 
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var resultContext = await next();

            // check if the user is authenticated 
            if (context.HttpContext.User.Identity?.IsAuthenticated != true) return;

            var memberId = resultContext.HttpContext.User.GetMemberId();

            // get the dbContext 
            var dbContext = resultContext.HttpContext.RequestServices.GetRequiredService<AppDbContext>();

            await dbContext.Members.Where(x => x.Id == memberId).ExecuteUpdateAsync(setters => setters.SetProperty(x => x.LastActive, DateTime.UtcNow));
        }
    }
}
