using IdeaMarket.DataModel;
using IdeaMarket.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IdeaMarket.Controllers
{
    public class BaseController : Controller
    {
        public int? UserId { get; set; }

        protected override void OnActionExecuting( ActionExecutingContext filterContext )
        {
            base.OnActionExecuting( filterContext );

            using( var db = new MainDB() )
            {
                UserId = AuthService.GetUserId( Request.Cookies["token"]?.Value );
            }
        }
    }
}