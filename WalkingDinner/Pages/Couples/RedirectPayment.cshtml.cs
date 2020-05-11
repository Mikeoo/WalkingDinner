using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WalkingDinner.Database;
using WalkingDinner.Extensions;
using WalkingDinner.Models;
using WalkingDinner.Mollie;

namespace WalkingDinner.Pages.Couples {

    public class RedirectPaymentModel : DataBoundModel {

        public RedirectPaymentModel( DatabaseContext context ) : base( context ) {
        }

        [BindProperty]
        public string PaymentStatus { get; set; }

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

            PaymentStatus = status.NewStatus;

            return Page();
        }
    }
}