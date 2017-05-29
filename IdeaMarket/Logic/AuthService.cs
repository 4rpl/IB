using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IdeaMarket.Logic
{
    /// <summary>
    /// Сервис авторизации
    /// </summary>
    public static class AuthService
    {
        public static Dictionary<string, int> Users = new Dictionary<string, int>();

        public static int? GetUserId( string token )
        {
            int id;
            return token != null && Users.TryGetValue( token, out id ) ? (int?)id : null;
        }

        public static string Login( int id )
        {
            var token = Guid.NewGuid().ToString();
            Users.Add( token, id );
            return token;
        }

        public static void Logoff( string token )
        {
            Users.Remove( token );
        }
    }
}