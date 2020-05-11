using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WalkingDinner.Database;
using WalkingDinner.Extensions;
using WalkingDinner.Models;

namespace WalkingDinner.Pages {

    public class LoginModel : DataBoundModel {

        public LoginModel( DatabaseContext context ) : base( context ) {
        }

        [BindProperty( SupportsGet = true )]
        [Required]
        [Display( Name = "Volgnummer" )]
        public int? Id { get; set; }

        [BindProperty( SupportsGet = true )]
        [Required]
        [Display( Name = "Identificatiecode" )]
        public string AdminCode { get; set; }

        [BindProperty( SupportsGet = true )]
        public string RedirectUrl { get; set; }


        private async Task<IActionResult> ValidateLogin() {

            Couple couple = await Database.GetCoupleAsAdminAsync( Id.Value, AdminCode );
            if ( couple == null ) {

                ModelState.AddModelError( "CustomError", "Incorrect Id / Password " );
                return Page();
            }

            HttpContext.Session.SetInt32( "Id", Id.Value );
            HttpContext.Session.SetString( "AdminCode", AdminCode );

            RedirectUrl = HttpUtility.UrlDecode( RedirectUrl );

            if ( string.IsNullOrEmpty( RedirectUrl ) ) {

                if ( couple.IsAdmin ) {

                    RedirectUrl = ModelPath.Get<Management.EditDinnerModel>();
                } else {

                    RedirectUrl = ModelPath.Get<Couples.EditCoupleModel>();
                }
            }

            return RedirectToPage( RedirectUrl );
        }


        public async Task<IActionResult> OnGetAsync( bool? logout ) {

            if ( logout != null ) {

                HttpContext.Session.Clear();
                Id          = null;
                AdminCode   = null;
            }


            if ( Id == null || AdminCode == null ) {

                ModelState.Clear();
                return Page();
            }

            return await ValidateLogin();
        }

        public async Task<IActionResult> OnPostAsync() {

            if ( !ModelState.IsValid ) {
                return Page();
            }

            return await ValidateLogin();
        }
    }
}