using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WalkingDinner.Database;
using WalkingDinner.Extensions;
using WalkingDinner.Models;

namespace WalkingDinner.Pages.Couples {
    public class RemoveModel : DataBoundModel {

        public RemoveModel( DatabaseContext context ) : base( context ) {
        }

        public Couple Couple { get; set; }

        public async Task<IActionResult> OnGetAsync() {

            Couple = await GetAuthorizedCouple();
            if ( Couple == null ) {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync() {

            Couple = await GetAuthorizedCouple();
            if ( Couple == null ) {
                return NotFound();
            }

            // IF HAS PAYED:
            // REFUND PAYMENT
#warning Payment

            Couple.PaymentId        = null;
            Couple.PaymentStatus    = null;
            Couple.Validated        = false;
            await Database.SaveChangesAsync();

            return Redirect( ModelPath.Get<Couples.SeeInvitationModel>() );
        }
    }
}
