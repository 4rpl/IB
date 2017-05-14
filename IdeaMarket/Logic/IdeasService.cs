using IdeaMarket.DataModel;
using IdeaMarket.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IdeaMarket.Logic
{
    public static class IdeasService
    {
        public static List<IdeaShortInfo> GetIdeasList()
        {
            using( var db = new MainDB() )
            {
                var q = db.Ideas
                    .Select( i => new IdeaShortInfo
                    {
                        Id = i.ID,
                        Name = i.Name,
                        Rating = i.Rating,
                        Status = i.Status,
                        Volume = $"{i.FullDescription.Split( ' ' ).Length} слов, {i.IdeaFiles.Count()} вложений"
                    } ).OrderBy( i => i.Name );
                var items = q.ToList();
                foreach( var item in items )
                {
                    item.Categories = db.IdeaCategories
                        .Where( i => i.IdeaId == item.Id )
                        .Select( i => i.Category )
                        .ToList();
                }
                return items;
            }
        }
    }
}