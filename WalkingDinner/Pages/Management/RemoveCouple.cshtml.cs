using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using WalkingDinner.Database;
using WalkingDinner.Extensions;
using WalkingDinner.Models;

namespace WalkingDinner.Pages.Management {

    public class RemoveCoupleModel : DataBoundModel {

        public RemoveCoupleModel( DatabaseContext context ) : base( context ) {
        }

        public Couple Couple { get; set; }

        public async Task<IActionResult> OnGetAsync( int? IdToRemove ) {

            if ( IdToRemove == null ) {

                return NotFound();
            }

            if ( IdToRemove == AuthorizedID ) {

                // Cannot remove self
                return BadRequest();
            }

            Couple adminCouple = await GetAuthorizedCouple();
            if ( adminCouple == null ) {

                return NotFound();
            }

            Couple = await Database.GetCoupleAsync( IdToRemove.Value );
            if ( Couple == null ) {

                return NotFound();
            }

            if ( Couple.Dinner.ID != adminCouple.Dinner.ID ) {

                return BadRequest();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync( int? IdToRemove ) {

            if ( IdToRemove == null ) {

                return NotFound();
            }

            if ( IdToRemove == AuthorizedID ) {

                // Cannot remove self
                return BadRequest();
            }

            Couple adminCouple = await GetAuthorizedCouple();
            if ( adminCouple == null ) {

                return NotFound();
            }

            Couple = await Database.GetCoupleAsync( IdToRemove.Value );
            if ( Couple == null ) {

                return NotFound();
            }

            if ( Couple.Dinner.ID != adminCouple.Dinner.ID ) {

                return BadRequest();
            }

            await Database.RemoveCoupleAsync( IdToRemove.Value );

            return Redirect( ModelPath.Get<Management.EditDinnerModel>() );
        }
    }
}
