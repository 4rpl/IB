using IdeaMarket.DataModel;
using IdeaMarket.Logic;
using IdeaMarket.Models;
using LinqToDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IdeaMarket.Controllers
{
    /// <summary>
    /// Контроллер с общими действиями
    /// </summary>
    public class HomeController : BaseController
    {
        [HttpGet]
        public ActionResult Register()
        {
            return View( new RegistrationModel() );
        }
        
        [HttpPost]
        public ActionResult Register( RegistrationModel user )
        {
            using(var db = new MainDB() )
            {
                var id = (int)(long)db.Users.InsertWithIdentity( () => new User
                {
                    Login = user.Login,
                    Name = user.Name,
                    Password = user.Password,
                    Email = user.Email
                } );
                Response.SetCookie( new HttpCookie( "token", AuthService.Login( id ) ) );
                db.Vendors.Insert( () => new Vendor
                {
                    UserId = id
                } );
            }
            return RedirectToAction( "Profile", "Vendor" );
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View( new LoginModel() );
        }

        [HttpPost]
        public ActionResult Login( LoginModel user )
        {
            using( var db = new MainDB() )
            {
                var userId = db.Users.Where( i => i.Login == user.Login && i.Password == user.Password ).Select( i => i.ID ).FirstOrDefault();
                Response.SetCookie( new HttpCookie( "token", AuthService.Login( userId ) ) );
                return RedirectToAction( "Profile", "Vendor" );
            }
        }

        [HttpGet]
        public ActionResult Logoff()
        {
            AuthService.Logoff( Request.Cookies["token"].Value );
            Response.SetCookie( new HttpCookie( "token", "" ) );
            return RedirectToAction( "Login" );
        }
    }
}