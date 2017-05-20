using IdeaMarket.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IdeaMarket.Models
{
    /// <summary>
    /// Краткое представление идеи для списка
    /// </summary>
    public class IdeaShortInfo
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Category> Categories { get; set; }
        public string Volume { get; set; }
        public IdeaStatus Status { get; set; }
        public double Rating { get; set; }
        public double? Cost { get; set; }
    }
}