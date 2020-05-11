using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WalkingDinner.Database;
using WalkingDinner.Extensions;
using WalkingDinner.Models;

namespace WalkingDinner.Pages.Management {

    public class EditDinnerModel : DataBoundModel {

        public EditDinnerModel( DatabaseContext context ) : base( context ) {
        }

        /**
         * Update Dinner name, description etc
         * 
         * Invite couples by Email and update Dinner. Maybe some administation for the payments?
         * 
         * Remove couples
         * 
         * After Subscriptionstop, see possible combinations and choose one -> send hostes their courses, generate pdf's
         * 
         */

        [BindProperty]
        public Couple Couple { get; set; }

        public class PersonInvite {

            [Required]
            public PersonMain Person { get; set; }

            [Required]
            [DataType( DataType.EmailAddress )]
            public string EmailAddress { get; set; }
        }

        [BindProperty]
        public PersonInvite Invite { get; set; }

        public async Task<IActionResult> OnGetAsync() {

            Couple = await GetAuthorizedCouple();
            if ( Couple == null ) {

                return NotFound();
            }

            if ( Couple.Dinner.SubscriptionStop < DateTime.Now ) {

                return Redirect( ModelPath.Get<Management.OverviewModel>() );
            }

            // Load the dinner itself, including other couples
            await Database.GetDinnerAsync( Couple.Dinner.ID );

            return Page();
        }

        public async Task<IActionResult> OnPostInvite() {

            Couple = await GetAuthorizedCouple();
            if ( Couple == null ) {

                return NotFound();
            }

            if ( !ModelState.IsValid( nameof( Invite ) ) ) {

                return Page();
            }

            Invite.EmailAddress = Invite.EmailAddress.ToLower();

            // Load the dinner itself, including other couples
            await Database.GetDinnerAsync( Couple.Dinner.ID );

            foreach ( Couple storedCouple in Couple.Dinner.Couples ) {

                if ( storedCouple.EmailAddress == Invite.EmailAddress ) {

                    ModelState.AddModelError( nameof( Invite ), $"{ Invite.Person } met emailadres { Invite.EmailAddress } is al uitgenodigd." );
                    return Page();
                }
            }

            // Invite
            Couple invitedCouple = await Database.CreateCoupleAsync( Couple.Dinner.ID, Invite.EmailAddress, Invite.Person );
            if ( invitedCouple == null ) {

                ModelState.AddModelError( nameof( Invite ), $"Kan { Invite.Person } nu niet uitnodigen." );
                return Page();
            }

            EmailServer.SendEmail( invitedCouple.EmailAddress, "Uitnodiging",
                $"U ben uitgenodigd door { Couple.PersonMain } om deel te namen aan een WalkingDinner. Bekijk uit uitnodiging <a href=\"{ ModelPath.GetAbsolutePathWithAuthorization<Couples.SeeInvitationModel>( Request.Host, invitedCouple.ID, invitedCouple.AdminCode ) }\">Hier</a>" );

            ViewData[ "InviteResult" ] = $"{ Invite.Person } is uitgenodigd.";

            // Clear input data
            ModelState.Clear( nameof( Invite ) );
            Invite = null;

            return Page();
        }

        public async Task<IActionResult> OnPostEdit() {

            var dinnerData = Couple.Dinner;

            Couple = await GetAuthorizedCouple();
            if ( Couple == null ) {

                return NotFound();
            }
            await Database.GetDinnerAsync( Couple.Dinner.ID );

            if ( !ModelState.IsValid( "Couple.Dinner" ) ) {

                return Page();
            }

            Couple.Dinner.Address.CopyFrom( dinnerData.Address );
            Couple.Dinner.Title         = dinnerData.Title;
            Couple.Dinner.Description   = dinnerData.Description;

            await Database.SaveChangesAsync();

            ViewData[ "EditResult" ] = $"De wijzigingen zijn opgeslagen.";

            return Page();

        }
    }
}
