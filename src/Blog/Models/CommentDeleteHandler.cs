using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Models
{
    public class CommentDeleteHandler:AuthorizationHandler<EditDeleteRequirement, CommentPostAuthors>

    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            EditDeleteRequirement requirement,
            CommentPostAuthors resource)
        {
            if (resource.CommentAuthor == context.User.Identity.Name || resource.PostAuthor==context.User.Identity.Name)
            {
                context.Succeed(requirement);
            }

            return Task.FromResult(0);
        }

    }
    
}
