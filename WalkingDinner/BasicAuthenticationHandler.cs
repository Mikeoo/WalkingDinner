using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using WalkingDinner.Database;
using WalkingDinner.Models;

namespace WalkingDinner {

    public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions> {

        private readonly DatabaseContext Db;
        private delegate T GetSessionValue<T>( string Key );

        public BasicAuthenticationHandler( IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock, DatabaseContext context ) : base( options, logger, encoder, clock ) {

            Db = context;
        }

        protected override Task HandleChallengeAsync( AuthenticationProperties properties ) {

            return base.HandleChallengeAsync( properties );
        }

        private T TryGetSubmittedValue<T>( string valueName, GetSessionValue<T> getSessionValue ) {

            //if ( !Request.RouteValues.TryGetValue( valueName, out object value ) ) {
            //if ( !Request.Query.TryGetValue( valueName, out StringValues values ) ) {
            try {

                return getSessionValue( valueName );
            } catch {

                return default( T );
            }
        }

        private int? TryGetCoupleId() {

            return TryGetSubmittedValue( "Id", Context.Session.GetInt32 );
        }

        private string TryGetAdminCode() {

            return TryGetSubmittedValue( "AdminCode", Context.Session.GetString );
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync() {

            string adminCode    = TryGetAdminCode();
            int? coupleId       = TryGetCoupleId();

            if ( adminCode == null || coupleId == null ) {
                return await Task.FromResult( AuthenticateResult.NoResult() );
            }

            Couple couple = await Db.Couples.FindAsync( coupleId.Value );
            if ( couple == null ) {
                return await Task.FromResult( AuthenticateResult.Fail( "Not found" ) );
            }

            if ( couple.AdminCode != adminCode ) {

                return await Task.FromResult( AuthenticateResult.Fail( "Admincode is incorrect." ) );
            }

            var claims = new[] {
                new Claim( ClaimTypes.NameIdentifier, coupleId.ToString() ),
                new Claim( ClaimTypes.Role, couple.IsAdmin ? "Admin" : "User" ),
        };

            var identity    = new ClaimsIdentity(claims, Scheme.Name);
            var principal   = new ClaimsPrincipal(identity);
            var ticket      = new AuthenticationTicket(principal, Scheme.Name);

            return await Task.FromResult( AuthenticateResult.Success( ticket ) );
        }
    }
}
