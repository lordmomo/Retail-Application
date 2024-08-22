//using DemoWebApplication.Models;
//using Microsoft.AspNetCore.Authorization;

//namespace DemoWebApplication.Helpers
//{
    //[ExcludeFromCodeCoverage]

//    public class RoleAuthorizationHandler : AuthorizationHandler<RoleRequirement>
//    {
//        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, RoleRequirement requirement)
//        {
//            foreach (var role in requirement.Roles)
//            {
//                if (context.User.IsInRole(role))
//                {
//                    context.Succeed(requirement);
//                }
//            }

//            return Task.CompletedTask;
//        }
//    }
//}
