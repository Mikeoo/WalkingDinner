using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using WalkingDinner.Database;
using WalkingDinner.Extensions;
using WalkingDinner.Models;
using WalkingDinner.Mollie;
using WalkingDinner.Mollie.Models;

namespace WalkingDinner.Pages.Couples {
    public class PaymentModel : DataBoundModel {


        public PaymentModel( DatabaseContext context ) : base( context ) {
        }

        public Couple Couple { get; set; }

        public string PaymentStatus { get; set; }

        public async Task<IActionResult> OnGetAsync() {

            Couple = await GetAuthorizedCouple();
            if ( Couple == null ) {

                return NotFound();
            }

            if ( !Couple.Dinner.HasPrice ) {

                return Redirect( ModelPath.Get<Couples.EditCoupleModel>() );
            }

            var status = await Couple.UpdatePaymentStatus();
            if ( status.Changed ) {

                await Database.SaveChangesAsync();
            }

            PaymentStatus = status.NewStatus;

            return Page();
        }

        public async Task<IActionResult> OnPostPay() {

            Couple = await GetAuthorizedCouple();
            if ( Couple == null ) {

                return NotFound();
            }

            if ( !Couple.Dinner.HasPrice ) {
                return BadRequest();
            }

            PaymentStatus = await MollieAPI.GetPaymentStatus( Couple.PaymentId );

            switch ( PaymentStatus ) {

                case "paid":
                case "pending":
                    ViewData[ "error" ] = "Deze bestelling is al betaald.";
                    return Page();
            }

            var response = await MollieAPI.PaymentRequest(
                new Payment(){
                    Amount = new Amount( Currency.EUR, Couple.Dinner.Price ),
                    Description = "Betaling voor het dinner",
                    RedirectUrl = ModelPath.GetAbsolutePathWithAuthorization<RedirectPaymentModel>( Request.Host, Couple.ID, Couple.AdminCode ),
                }
            );

            string link = response?.GetLink("checkout");
            if ( string.IsNullOrEmpty( link ) ) {

                ViewData[ "error" ] = "De betaling kan nu niet worden gedaan. Probeer het later opnieuw.";
                return Page();
            }

            // Save the paymentId
            Couple.PaymentId = response.Id;
            await Database.SaveChangesAsync();

            // Go to mollie
            return Redirect( link );
        }

        public async Task<IActionResult> OnPostCancel() {

            Couple = await GetAuthorizedCouple();
            if ( Couple == null ) {

                return NotFound();
            }

            if ( !Couple.Dinner.HasPrice ) {
                return BadRequest();
            }

            PaymentStatus = await MollieAPI.GetPaymentStatus( Couple.PaymentId );

            switch ( PaymentStatus ) {

                case "paid":
                case "pending":
                    break;
                default:
                    ViewData[ "error" ] = "Er staat geen betaling open.";
                    return Page();
            }

            var refundResponse = await MollieAPI.CreateRefund( Couple.PaymentId, Couple.Dinner.Price );
            if ( refundResponse == null ) {

                ViewData[ "error" ] = "Kan betaling nu niet annuleren. Probeer het later opnieuw.";
                return Page();
            }

            Couple.PaymentId        = null;
            Couple.PaymentStatus    = null;
            await Database.SaveChangesAsync();

            PaymentStatus           = null;
            ViewData[ "status" ]    = "Annulering is aangemaakt.";

            return Page();
        }
    }
}
