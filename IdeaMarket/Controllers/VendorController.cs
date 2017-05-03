using IdeaMarket.Dto;
using IdeaMarket.Dto.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IdeaMarket.Controllers
{
    public class VendorController : Controller
    {
        // GET: Vendor
        public ActionResult Index()
        {
            return RedirectToAction( "List" );
        }

        [HttpGet]
        public ActionResult List()
        {
            return View();
        }

        public class Idea
        {
            public string Name { get; set; }
            public string ShortDescription { get; set; }
            public string FullDescription { get; set; }
            public string Status { get; set; }
            public int Cost { get; set; }
        }

        [HttpGet]
        public ActionResult EditIdea()
        {
            return View( new Idea
            {
                Name = "Lorem Ipsum",
                ShortDescription = "Краткое описание. Blah-blah-blah...",
                FullDescription = "Полное описание. Blah-blah-blah...",
                Status = "Опубликована",
                Cost = 12
            } );
        }

        [HttpGet]
        public ActionResult Settings()
        {
            var model = new VendorSettings
            {
                Name = "Илья Юдов",
                Login = "iyudov",
                Description = "Лучший продавец на раёне",
                Status = VendorStatus.Ready,
                Visibility = VendorVisibility.All,
                Categories = new List<IdeasCategory>
                {
                    IdeasCategory.Medicine,
                    IdeasCategory.Sport
                }
            };
            return View( model );
        }

        [HttpPost]
        public ActionResult Settings( VendorSettings settings )
        {
            return View();
        }

        [HttpGet]
        public ActionResult Comments()
        {
            return View();
        }
    }
}