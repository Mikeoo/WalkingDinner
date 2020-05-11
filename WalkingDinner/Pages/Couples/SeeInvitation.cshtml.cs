using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WalkingDinner.Database;
using WalkingDinner.Extensions;
using WalkingDinner.Models;

namespace WalkingDinner.Pages.Couples {

    public class SeeInvitationModel : DataBoundModel {

        public SeeInvitationModel( DatabaseContext context ) : base( context ) {
        }

        /**
         * 
         * Accept
         * 
         */

        public Couple Couple { get; set; }

        public async Task<IActionResult> OnGetAsync() {

            Couple = await GetAuthorizedCouple();
            if ( Couple == null ) {

                return NotFound();
            }

            if ( Couple.Validated ) {

                return Redirect( ModelPath.Get<Couples.EditCoupleModel>() );
            }

            return Page();
        }
    }
}
