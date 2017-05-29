using IdeaMarket.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IdeaMarket.Models
{
    /// <summary>
    /// Модель покупателя
    /// </summary>
    public class BuyerModel
    {
        public string Name { get; set; }
        public List<Category> Categories { get; set; }
    }
}