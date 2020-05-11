using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WalkingDinner.Database;
using WalkingDinner.Extensions;
using WalkingDinner.Models;
using WalkingDinner.Mollie;

namespace WalkingDinner.Pages.Couples {

    public class EditCoupleModel : DataBoundModel {

        public EditCoupleModel( DatabaseContext context ) : base( context ) {
        }

        [BindProperty]
        public Couple Couple { get; set; }

        public async Task<IActionResult> OnGetAsync() {

            Couple = await GetAuthorizedCouple();
            if ( Couple == null ) {

                return NotFound();
            }

            var status = await Couple.UpdatePaymentStatus();
            if ( status.Changed ) {

                await Database.SaveChangesAsync();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync() {

            var coupleData = Couple;
            // Reload database
            Couple = await GetAuthorizedCouple();

            if ( !ModelState.IsValid ) {
                return Page();
            }

            if ( Couple.Dinner.HasPrice && string.IsNullOrEmpty( coupleData.IBAN ) ) {

                ModelState.AddModelError( "IBAN", "Bankrekening is verplicht!" );
                return Page();
            }

            if ( await Database.UpdateCoupleAsync( AuthorizedID, coupleData ) == null ) {

                ModelState.AddModelError( "Couple", "Kan nu niet opslaan, probeer het later opnieuw." );
                return Page();
            }

            ViewData[ "status" ] = "Wijzigingen opgeslagen!";

            return Page();
        }

    }
}
