using IdeaMarket.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IdeaMarket.Models
{
    /// <summary>
    /// Модель настройки профиля продавца
    /// </summary>
    public class VendorSettings
    {
        public string Name { get; set; }
        public string Login { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public Visibility Visibility { get; set; }
        public IEnumerable<Category> Categories { get; set; }
        public string Email { get; set; }
        public string NewPassword { get; set; }
        public string OldPassword { get; set; }
    }
}