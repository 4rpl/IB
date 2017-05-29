using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IdeaMarket.Models
{
    /// <summary>
    /// Модель регистрации
    /// </summary>
    public class RegistrationModel
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public bool IsVendor { get; set; }
    }
}