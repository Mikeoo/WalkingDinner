using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using WalkingDinner.Database;
using WalkingDinner.Extensions;
using WalkingDinner.Models;

namespace WalkingDinner.Pages.Setup {

    public class CreateDinnerModel : DataBoundModel {

        public CreateDinnerModel( DatabaseContext context ) : base( context ) {
        }

        /**
         * User setup Title, Description, Location, Date, SubscriptionStop, Price, IBAN, 
         * Admin and AdminEmailAdress
         * 
         * Generate Code for inventations and AdminCode for management
         * 
         * Send AdminCode to AdminEmailAddress
         * 
         */

        [BindProperty]
        public Dinner Dinner { get; set; }

        [BindProperty]
        public Couple Couple { get; set; }

        public IActionResult OnGet() {

            Dinner = new Dinner() {
                Date                = DateTime.Now.AddDays( Dinner.MIN_DAYS_IN_ADVANCE + 7 ).SetTime( 17, 0 ),
                SubscriptionStop    = DateTime.Now.AddDays( Dinner.MIN_DAYS_IN_ADVANCE + 1 ).SetTime( 0, 0 ),
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync() {

            if ( !ModelState.IsValid ) {

                return Page();
            }

            // Strip seconds
            Dinner.Date             = Dinner.Date.SetTime( Dinner.Date.Hour, Dinner.Date.Minute, 0, 0 );
            Dinner.SubscriptionStop = Dinner.SubscriptionStop.SetTime( Dinner.Date.Hour, Dinner.Date.Minute, 0, 0 );

            // Dont allow negative prices
            Dinner.Price = Math.Max( Dinner.Price, 0.0 );

            if ( Dinner.Date < DateTime.Now.AddDays( Dinner.MIN_DAYS_IN_ADVANCE ).SetTime( 0, 0 ) ) {

                ModelState.AddModelError( "Dinner.Date", $"Kies een datum minimaal { Dinner.MIN_DAYS_IN_ADVANCE } dagen in de toekomst." );
                return Page();
            }

            if ( ( Dinner.Date - Dinner.SubscriptionStop ).TotalHours < 24 ) {

                ModelState.AddModelError( "Dinner.SubscriptionStop", "Kies een tijd minimaal 24 uur vóór het diner." );
                return Page();
            }

            if ( Dinner.HasPrice && ( string.IsNullOrWhiteSpace( Couple.IBAN ) ) ) {

                ModelState.AddModelError( "Couple.IBAN", "Een bankrekening is verplicht als het evenement geld kost." );
                return Page();
            }

            if ( await Database.CreateDinnerAsync( Dinner, Couple ) == null ) {

                ModelState.AddModelError( nameof( Dinner ), "Kan dinner nu niet aanmaken." );
                return Page();
            }

            EmailServer.SendEmail( Couple.EmailAddress, "Nieuw dinner",
                $"Nieuw diner aangemaakt, code: <a href=\"{ ModelPath.GetAbsolutePathWithAuthorization<Management.EditDinnerModel>( Request.Host, Couple.ID, Couple.AdminCode )}\">Beheer</a>" );

            return Redirect( ModelPath.Get<AwaitEmailModel>() );
        }

    }
}
