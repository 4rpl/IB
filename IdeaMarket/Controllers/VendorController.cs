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
    public class VendorController : Controller
    {
        public ActionResult Index()
        {
            return RedirectToAction( "List" );
        }

        [HttpGet]
        public ActionResult List()
        {
            using( var db = new MainDB() )
            {
                var ideas = IdeasService.GetIdeasList();
                return View( ideas );
            }
        }

        [HttpGet]
        public ActionResult EditIdea( int id )
        {
            using (var db = new MainDB() )
            {
                var idea = db.Ideas
                    .Where( i => i.ID == id )
                    .FirstOrDefault();
                if( idea == null )
                {
                    return HttpNotFound();
                }
                var files = db.IdeaFiles
                    .Where( i => i.IdeaId == id )
                    .Select( i => new
                    {
                        Name = i.File.Name,
                        Id = i.File.ID
                    } );
                idea.IdeaFiles = files.Select( i => new IdeaFile
                {
                    File = new File
                    {
                        ID = i.Id,
                        Name = i.Name
                    }
                } );

                return View( idea );
            }
        }

        [HttpPost]
        public ActionResult EditIdea( Idea idea, bool publish, List<Category> categories, List<int> files, List<HttpPostedFileBase> upload )
        {
            using( var db = new MainDB() )
            {
                var filesToDelete = db.IdeaFiles
                    .Where( i => !files.Contains( i.FileId ) && i.IdeaId == idea.ID )
                    .Select( i => i.FileId )
                    .ToList();
                db.IdeaFiles.Delete( i => filesToDelete.Contains( i.FileId ) );
                db.Files.Delete( i => filesToDelete.Contains( i.ID ) );
                IdeaStatus status;
                if( publish )
                {
                    status = IdeaStatus.WaitsForApproval;
                }
                else
                {
                    status = IdeaStatus.Draft;
                }
                db.Ideas.Where( i => i.ID == idea.ID )
                    .Set( i => i.Name, idea.Name )
                    .Set( i => i.ShortDescription, idea.ShortDescription ?? "" )
                    .Set( i => i.FullDescription, idea.FullDescription ?? "" )
                    .Set( i => i.Status, status )
                    .Update();

                var currCategories = db.IdeaCategories
                    .Where( i => i.IdeaId == idea.ID )
                    .Select( i => i.Category )
                    .ToList();

                categories = categories ?? new List<Category>();
                foreach(var cat in categories.Where( i => !currCategories.Contains( i ) ) )
                {
                    db.IdeaCategories.Insert( () => new IdeaCategory
                    {
                        IdeaId = idea.ID,
                        Category = cat
                    } );
                }

                foreach( var cat in currCategories.Where( i => !categories.Contains( i ) ) )
                {
                    db.IdeaCategories.Delete( i => i.IdeaId == idea.ID && i.Category == cat );
                }

                for( var i = 0; i < upload.Count; i++ )
                {
                    var file = upload[i];
                    if( file == null )
                    {
                        continue;
                    }
                    byte[] content = new byte[file.ContentLength];
                    file.InputStream.Read( content, 0, file.ContentLength );
                    var fileId = (int)(Int64)db.Files.InsertWithIdentity( () => new File
                    {
                        Name = file.FileName,
                        Content = content,
                        ContentType = file.ContentType
                    } );
                    db.IdeaFiles.Insert( () => new IdeaFile
                    {
                        FileId = fileId,
                        IdeaId = idea.ID
                    } );
                }
            }
            return RedirectToAction( "List" );
        }

        [HttpGet]
        public ActionResult NewIdea()
        {
            var idea = new Idea();
            return View( idea );
        }

        [HttpPost]
        public ActionResult NewIdea( Idea idea, bool publish, List<Category> categories, List<HttpPostedFileBase> upload )
        {
            using( var db = new MainDB() )
            {
                if( publish )
                {
                    idea.Status = IdeaStatus.WaitsForApproval;
                }
                else
                {
                    idea.Status = IdeaStatus.Draft;
                }
                db.Ideas.InsertWithIdentity( () => new Idea
                {
                    Name = idea.Name,
                    Cost = idea.Cost,
                    ShortDescription = idea.ShortDescription ?? "",
                    FullDescription = idea.FullDescription ?? "",
                    OwnerId = 1,
                    Rating = 0,
                    Status = idea.Status
                } );

                categories = categories ?? new List<Category>();
                foreach( var cat in categories )
                {
                    db.IdeaCategories.Insert( () => new IdeaCategory
                    {
                        IdeaId = idea.ID,
                        Category = cat
                    } );
                }

                for( var i = 0; i < upload.Count; i++ )
                {
                    var file = upload[i];
                    if( file == null )
                    {
                        continue;
                    }
                    byte[] content = new byte[file.ContentLength];
                    file.InputStream.Read( content, 0, file.ContentLength );
                    var fileId = (int)(Int64)db.Files.InsertWithIdentity( () => new File
                    {
                        Name = file.FileName,
                        Content = content,
                        ContentType = file.ContentType
                    } );
                    db.IdeaFiles.Insert( () => new IdeaFile
                    {
                        FileId = fileId,
                        IdeaId = idea.ID
                    } );
                }
            }
            return RedirectToAction( "List" );
        }

        [HttpPost]
        public ActionResult DeleteIdea( int id )
        {
            using( var db = new MainDB() )
            {
                var fileIds = db.IdeaFiles.Where( i => i.IdeaId == id ).Select( i => i.FileId ).ToList();
                db.Files.Delete( i => fileIds.Contains( i.ID ) );
                db.IdeaFiles.Delete( i => i.IdeaId == id );
                db.IdeaCategories.Delete( i => i.IdeaId == id );
                db.Ideas.Delete( i => i.ID == id );
            }
            return RedirectToAction( "List" );
        }

        [HttpGet]
        public ActionResult UserProfile()
        {
            using( var db = new MainDB() )
            {
                var vendor = db.Vendors
                    .LoadWith( i => i.User )
                    .LoadWith( i => i.Categories )
                    .Where( i => i.ID == 1 )
                    .Select( i => new VendorSettings
                    {
                        Login = i.User.Login,
                        Description = i.ShortDescription,
                        Name = "asd",
                        Email = i.User.Email,
                        Visibility = i.User.Visibility,
                        Categories = i.Categories.Select( j => j.Category ).ToList()
                    } )
                    .FirstOrDefault();
                return View( vendor );
            }
        }

        [HttpGet]
        public ActionResult Settings()
        {
            using( var db = new MainDB() )
            {
                var vendor = db.Vendors
                    .LoadWith( i => i.User )
                    .LoadWith( i => i.Categories )
                    .Where( i => i.ID == 1 )
                    .Select( i => new VendorSettings
                    {
                        Login = i.User.Login,
                        Description = i.ShortDescription,
                        Name = "asd",
                        Email = i.User.Email,
                        Visibility = i.User.Visibility,
                        Categories = i.Categories
                    } )
                    .FirstOrDefault();
                return View( vendor );
            }
        }

        [HttpPost]
        public ActionResult Settings( object settings )
        {
            return View();
        }

        [HttpGet]
        public ActionResult Comments()
        {
            return View();
        }

        [HttpGet]
        public ActionResult File( int id )
        {
            using( var db = new MainDB() )
            {
                var file = db.Files
                    .Where( i => i.ID == id )
                    .FirstOrDefault();
                return File( file.Content, file.ContentType );
            }
        }
    }
}