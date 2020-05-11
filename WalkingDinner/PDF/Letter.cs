using DinkToPdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WalkingDinner.Calculation.Models;
using WalkingDinner.Models;

namespace WalkingDinner.PDF {

    public static class Letter {

        private static string GetTemplatePath( string templatename ) {

            return Path.Combine( Directory.GetCurrentDirectory(), "PDF", "template", templatename );
        }

        private static string GetTemplate( string templatename ) {

            return File.ReadAllText( GetTemplatePath( templatename ) );
        }

        private static string GenerateTemplate( string template, object templateValues ) {

            if ( templateValues != null ) {

                Type valueType              = templateValues.GetType();
                PropertyInfo[] properties   = valueType.GetProperties();

                foreach ( PropertyInfo property in properties ) {

                    template = template.Replace( $"%{ property.Name.ToUpper() }%", property.GetValue( templateValues ).ToString() );
                }
            }

            return template;
        }

        private static IEnumerable<Route> GetRoutes( List<Route> routes, Route route, int courseCount ) {

            List<Route> result = new List<Route>();

            if ( route.CourseIndex >= courseCount - 1 ) {
                return result;
            }

            IEnumerable<Route> Guests = routes.Where( o => o.CourseIndex == route.CourseIndex && o.To == route.Subject );

            foreach ( Route guest in Guests ) {

                Route next = routes.Where( o => ( o.CourseIndex == route.CourseIndex + 1) && ( o.Subject == guest.Subject ) ).FirstOrDefault();
                if ( next == null ) {
                    continue;
                }

                if ( next.To == next.Subject ) {

                    result.AddRange( GetRoutes( routes, next, courseCount ) );
                }

                result.Add( next );
            }

            return result;
        }

        private static string GenerateHTML( List<Route> routes, int courseCount, DateTime startTime ) {

            string document = GetTemplate("document.html");
            string block = GetTemplate("document.block.html");
            string stack = GetTemplate("document.stack.html");

            int index = 0;

            // DateTime baseTime = new DateTime( 2000, 1, 1, 18, 0, 0 );

            StringBuilder content = new StringBuilder();
            foreach ( Route route in routes ) {

                string item;

                if ( route.Subject == route.To ) {

                    // For the chef
                    item = GenerateTemplate( stack, new {

                        Subject = route.Subject.GetName(),
                        List = string.Join( "\n", GetRoutes( routes, route, courseCount ).Select( o => $"<li>{ o.Subject.GetName() } in ronde { o.CourseIndex }</li>" ) ),
                        Course = ( route.CourseIndex == 0 ) ? "'start'" : ( route.CourseIndex ).ToString(),
                        //Course = route.CourseIndex + 1,
                    } );

                } else {

                    item = GenerateTemplate( block, new {
                        Subject = route.Subject.GetName(),
                        Course = ( route.CourseIndex == 0 ) ? "'start'" : ( route.CourseIndex ).ToString(),
                        To = route.To.GetName(),
                        Street = route.To.Address.GetAsLine(),
                        Time = startTime.AddMinutes( 30 * route.CourseIndex ).ToString( "HH:mm" )
                    } );
                }

                content.AppendLine( item );

                if ( ++index >= 3 ) {

                    // Break page
                    content.AppendLine( "<p style=\"page-break-before: always\"></p>" );
                    index = 0;
                }
            }

            document = GenerateTemplate( document, new { Content = content.ToString() } );

            return document;
        }

        public static byte[] Generate( List<Route> routes, int courseCount, DateTime startTime ) {

            string HTML = GenerateHTML( routes, courseCount, startTime );

            var globalSettings = new GlobalSettings
            {
                ColorMode = ColorMode.Color,
                Orientation = Orientation.Portrait,
                PaperSize = PaperKind.A4,
                DPI = 300,
                DocumentTitle = "Schema",
                // Out = @"X:\test.pdf"
            };

            var objectSettings = new ObjectSettings
            {
                HtmlContent = HTML,
                WebSettings     = { DefaultEncoding = "utf-8", UserStyleSheet =  GetTemplatePath( "document.css" ) },
            };

            var doc = new HtmlToPdfDocument()
            {
                GlobalSettings = globalSettings,
                Objects = { objectSettings }
            };

            File.WriteAllText( @"X:\test.html", HTML );

            var converter = new BasicConverter(new PdfTools());
            return converter.Convert( doc );
        }
    }
}
