using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;


namespace Blog.Models
{
    public class PostEditDeleteHandler: AuthorizationHandler<EditDeleteRequirement, Post>

    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            EditDeleteRequirement requirement,
            Post resource)
        {
            if (resource.Author == context.User.Identity.Name)
            {
                context.Succeed(requirement);
            }

            return Task.FromResult(0);
        }

    }
}
