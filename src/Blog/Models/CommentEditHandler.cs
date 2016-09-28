using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Models
{

    public class CommentEditHandler : AuthorizationHandler<EditDeleteRequirement, Comment>

    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            EditDeleteRequirement requirement,
            Comment resource)
        {
            if (resource.CommentAuthor == context.User.Identity.Name)
            {
                context.Succeed(requirement);
            }

            return Task.FromResult(0);
        }

    }

}
