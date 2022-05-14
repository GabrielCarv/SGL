using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Web.Mvc.Filters;

namespace MVC_Library
{
    public class CustomAuthorization : System.Web.Mvc.AuthorizeAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext authorization)
        {
            string session = Convert.ToString(authorization.HttpContext.Session.GetString("UserID"));
           
            if(session != null)
            {
                return;
            }
            else
            {
                authorization.Result = new UnauthorizedResult();
            }
        }
    }
}
