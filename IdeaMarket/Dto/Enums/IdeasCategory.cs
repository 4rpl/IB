using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IdeaMarket.Dto.Enums
{
    public enum IdeasCategory
    {
        [Display(Name="Спорт")]
        Sport = 1,
        [Display( Name = "IT" )]
        IT = 2,
        [Display( Name = "Медицина" )]
        Medicine = 3,
        [Display( Name = "Медицина" )]
        Media = 4,
        Auto = 5,
        Finance = 6,
        Wear = 7,
        Home = 8,
        Communication = 9,
        Games = 10,
        Other = 11
    }
}