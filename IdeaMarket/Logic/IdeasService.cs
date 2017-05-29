using IdeaMarket.DataModel;
using IdeaMarket.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IdeaMarket.Logic
{
    /// <summary>
    /// Сервис доступа к идеям
    /// </summary>
    public static class IdeasService
    {
        // TODO: по-хорошему, нужно вынести сюда всю работу с бд из контроллера
        public static List<IdeaShortInfo> GetIdeasList( int userId )
        {
            using( var db = new MainDB() )
            {
                var q = db.Ideas
                    .Where( i => i.OwnerId == userId )
                    .Select( i => new IdeaShortInfo
                    {
                        Id = i.ID,
                        Name = i.Name,
                        Rating = i.Rating,
                        Status = i.Status,
                        Volume = $"{i.FullDescription.Split( ' ' ).Length} слов, {i.IdeaFiles.Count()} вложений",
                        Cost = i.Cost
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