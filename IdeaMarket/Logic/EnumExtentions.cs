using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Web;

namespace IdeaMarket.Logic
{
    public static class EnumExtensions
    {
        public static string GetDisplayName( this Enum enumValue )
        {
            // Получить значение DisplayAttribute.Name
            // Нужно для вьюхъ
            return enumValue.GetType()
                            .GetMember( enumValue.ToString() )
                            .First()
                            .GetCustomAttribute<DisplayAttribute>()
                            .GetName();
        }
    }
}