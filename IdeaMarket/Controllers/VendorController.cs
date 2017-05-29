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
    /// Контроллер с действиями для продавца
    /// </summary>
    public class VendorController : BaseController
    {
        public ActionResult Index()
        {
            return RedirectToAction( "List" );
        }

        /// <summary>
        /// Список идей
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult List()
        {
            if( !UserId.HasValue )
            {
                return RedirectToAction( "Login", "Home" );
            }
            else
            {
                using( var db = new MainDB() )
                {
                    var ideas = IdeasService.GetIdeasList( UserId.Value );
                    return View( ideas );
                }
            }
        }

        /// <summary>
        /// Редактировать идею
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult EditIdea( int id )
        {
            if( !UserId.HasValue )
            {
                return RedirectToAction( "Login", "Home" );
            }
            using (var db = new MainDB() )
            {
                var idea = db.Ideas
                    .Where( i => i.ID == id && i.OwnerId == UserId.Value )
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
                idea.Categories = db.IdeaCategories.Where( i => i.IdeaId == id ).ToList();

                return View( idea );
            }
        }

        /// <summary>
        /// Редактировать идею
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EditIdea( Idea idea, bool publish, List<Category> categories, List<int> files, List<HttpPostedFileBase> upload )
        {
            if( !UserId.HasValue )
            {
                return RedirectToAction( "Login", "Home" );
            }

            using( var db = new MainDB() )
            {
                if( db.Ideas.Where( i => i.ID == idea.ID && i.OwnerId == UserId.Value ).Any() )
                {
                    return HttpNotFound();
                }

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

        /// <summary>
        /// Новая идея
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult NewIdea()
        {
            if( !UserId.HasValue )
            {
                return RedirectToAction( "Login", "Home" );
            }

            var idea = new Idea();
            return View( idea );
        }

        /// <summary>
        /// Новая идея
        /// </summary>
        /// <param name="idea"></param>
        /// <param name="publish"></param>
        /// <param name="categories"></param>
        /// <param name="upload"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult NewIdea( Idea idea, bool publish, List<Category> categories, List<HttpPostedFileBase> upload )
        {
            if( !UserId.HasValue )
            {
                return RedirectToAction( "Login", "Home" );
            }

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

        /// <summary>
        /// Удалить идею
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DeleteIdea( int id )
        {
            if( !UserId.HasValue )
            {
                return RedirectToAction( "Login", "Home" );
            }

            using( var db = new MainDB() )
            {
                if( db.Ideas.Where( i => i.ID == id && i.OwnerId == UserId.Value ).Any() )
                {
                    return HttpNotFound();
                }

                var fileIds = db.IdeaFiles.Where( i => i.IdeaId == id ).Select( i => i.FileId ).ToList();
                db.Files.Delete( i => fileIds.Contains( i.ID ) );
                db.IdeaFiles.Delete( i => i.IdeaId == id );
                db.IdeaCategories.Delete( i => i.IdeaId == id );
                db.Ideas.Delete( i => i.ID == id );
            }
            return RedirectToAction( "List" );
        }

        /// <summary>
        /// Страница профиля
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Profile()
        {
            if( !UserId.HasValue )
            {
                return RedirectToAction( "Login", "Home" );
            }
            using( var db = new MainDB() )
            {
                var settings = db.Vendors
                    .LoadWith( i => i.User )
                    .Where( i => i.ID == UserId.Value )
                    .Select( i => new VendorSettings
                    {
                        Login = i.User.Login,
                        Description = i.ShortDescription,
                        Name = i.User.Name,
                        Email = i.User.Email,
                        Visibility = i.User.Visibility
                    } )
                    .FirstOrDefault();
                settings.Categories = db.VendorCategories
                    .Where( i => i.VendorId == UserId.Value )
                    .Select( i => i.Category )
                    .ToList();
                return View( settings );
            }
        }

        /// <summary>
        /// Редактирование профиля
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Settings()
        {
            if( !UserId.HasValue )
            {
                return RedirectToAction( "Login", "Home" );
            }
            using( var db = new MainDB() )
            {
                var settings = db.Vendors
                    .LoadWith( i => i.User )
                    .Where( i => i.ID == UserId.Value )
                    .Select( i => new VendorSettings
                    {
                        Login = i.User.Login,
                        Description = i.ShortDescription,
                        Name = i.User.Name,
                        Email = i.User.Email,
                        Visibility = i.User.Visibility
                    } )
                    .FirstOrDefault();
                settings.Categories = db.VendorCategories
                    .Where( i => i.VendorId == UserId.Value )
                    .Select( i => i.Category )
                    .ToList();
                return View( settings );
            }
        }

        [HttpPost]
        public ActionResult Settings( VendorSettings settings )
        {
            if( !UserId.HasValue )
            {
                return RedirectToAction( "Login", "Home" );
            }
            using( var db = new MainDB() )
            {
                if( db.Users.Where( i => i.ID == UserId.Value ).FirstOrDefault()?.Password == settings.OldPassword )
                {
                    if( string.IsNullOrWhiteSpace( settings.NewPassword ) )
                    {
                        db.Users
                            .Where( i => i.ID == UserId.Value )
                            .Set( i => i.Visibility, settings.Visibility )
                            .Update();
                    }
                    else
                    {
                        db.Users
                            .Where( i => i.ID == UserId.Value )
                            .Set( i => i.Visibility, settings.Visibility )
                            .Set( i => i.Password, settings.NewPassword )
                            .Update();
                    }
                    db.Vendors
                        .Where( i => i.ID == UserId.Value )
                        .Set( i => i.ShortDescription, settings.Description )
                        .Update();
                    db.VendorCategories
                        .Where( i => i.VendorId == UserId.Value )
                        .Delete();
                    foreach( var cat in settings.Categories )
                    {
                        db.VendorCategories.Insert( () => new VendorCategory
                        {
                            VendorId = UserId.Value,
                            Category = cat
                        } );
                    }
                    return RedirectToAction( "Profile" );
                }
                else
                {
                    settings.NewPassword = "";
                    settings.OldPassword = "";
                    ViewBag.WrongPassword = "Введён неверный пароль";
                    return View( settings );
                }
            }
        }

        /// <summary>
        /// Список покупателей
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Buyers()
        {
            if( !UserId.HasValue )
            {
                return RedirectToAction( "Login", "Home" );
            }
            var buyers = new List<BuyerModel>
            {
                new BuyerModel
                {
                    Name = "Вася",
                    Categories = new List<Category>
                    {
                        Category.Alcohol,
                        Category.IT
                    }
                },
                new BuyerModel
                {
                    Name = "Петя",
                    Categories = new List<Category>
                    {
                        Category.Cosmos,
                        Category.Medicine
                    }
                },
                new BuyerModel
                {
                    Name = "Вова",
                    Categories = new List<Category>
                    {
                        Category.Pozilivers
                    }
                },
                new BuyerModel
                {
                    Name = "Дима",
                    Categories = new List<Category>
                    {
                        Category.IT
                    }
                }
            };
            return View( buyers );
        }

        /// <summary>
        /// Страница с отзывами
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Comments()
        {
            if( !UserId.HasValue )
            {
                return RedirectToAction( "Login", "Home" );
            }
            return View();
        }

        /// <summary>
        /// Получить файл
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult File( int id )
        {
            if( !UserId.HasValue )
            {
                return RedirectToAction( "Login", "Home" );
            }
            using( var db = new MainDB() )
            {
                // TODO: в настоящей системе нужна проверка пользователя, blah-blah-blah
                var file = db.Files
                    .Where( i => i.ID == id )
                    .FirstOrDefault();
                return File( file.Content, file.ContentType );
            }
        }

        /// <summary>
        /// Страница с кошельком
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Purse()
        {
            if( !UserId.HasValue )
            {
                return RedirectToAction( "Login", "Home" );
            }
            return View();
        }
    }
}