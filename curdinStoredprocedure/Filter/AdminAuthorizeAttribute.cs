using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace curdinStoredprocedure.Filter
{
    public class AdminAuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var role = context.HttpContext.Session.GetString("UserRole")
                       ?? context.HttpContext.Request.Cookies["Role"]; 

            if (string.IsNullOrEmpty(role) || role != "admin")
            {
                context.Result = new ForbidResult();
            }
        }
    }
    //public class UserAuthorizeAttribute : Attribute, IAuthorizationFilter
    //{
    //    public void OnAuthorization(AuthorizationFilterContext context)
    //    {
    //        var role = context.HttpContext.Session.GetString("UserRole")
    //                   ?? context.HttpContext.Request.Cookies["Role"]; 

    //        if (string.IsNullOrEmpty(role) || role != "admin")
    //        {
    //            context.Result = new ForbidResult();
    //        }
    //    }
    //}


}
