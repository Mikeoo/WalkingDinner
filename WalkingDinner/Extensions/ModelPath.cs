using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace WalkingDinner.Extensions {

    public static class ModelPath {

        private const string PAGE_ROOT_NAMESPACE = "WalkingDinner.Pages";
        private static readonly int PAGE_ROOT_NAMESPACE_LEN = PAGE_ROOT_NAMESPACE.Length + 1;

        public static string Get<T>() where T : PageModel {

            Type pageType   = typeof(T);

            string fullName = pageType.FullName;
            int fullNameLen = fullName.Length;

            // Subtract 'Model' from type name
            fullNameLen -= 5;

            string absoluteName = '/' + fullName.Substring( PAGE_ROOT_NAMESPACE_LEN, fullNameLen - PAGE_ROOT_NAMESPACE_LEN  );

            return absoluteName.Replace( '.', '/' );
        }

        private static object AuthorizedRouteValue( int Id, string AdminCode ) {

            return new { Id = Id, AdminCode = AdminCode };
        }

        public static string Get<T>( object routeValues ) where T : PageModel {

            StringBuilder result = new StringBuilder( Get<T>() + '?' );

            if ( routeValues != null ) {

                Type valueType              = routeValues.GetType();
                PropertyInfo[] properties   = valueType.GetProperties();

                foreach ( PropertyInfo property in properties ) {

                    result.Append( $"{ property.Name }={ property.GetValue( routeValues ) }&" );
                }
            }

            return result.ToString().TrimEnd( '?', '&' );
        }

        public static string GetAbsolutePath<T>( HostString RequestHost, object routeValues ) where T : PageModel {

            return $"https://{ RequestHost }{ Get<T>( routeValues )}";
        }

        public static string GetAbsolutePath<T>( HostString RequestHost, int Id, string AdminCode ) where T : PageModel {

            return GetAbsolutePath<T>( RequestHost, AuthorizedRouteValue( Id, AdminCode ) );
        }

        public static string GetAbsolutePathWithAuthorization<T>( HostString RequestHost, int Id, string AdminCode ) where T : PageModel {

            StringBuilder result = new StringBuilder( GetAbsolutePath<Pages.LoginModel>( RequestHost, Id, AdminCode ) );

            result.Append( $"&RedirectURL={ HttpUtility.HtmlDecode( Get<T>() ) }" );

            return result.ToString();
        }
    }
}
