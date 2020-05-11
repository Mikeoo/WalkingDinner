using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WalkingDinner.Calculation.Models;
using WalkingDinner.Database;
using WalkingDinner.Extensions;
using WalkingDinner.Models;
using WalkingDinner.PDF;

namespace WalkingDinner.Pages.Management {

    public class OverviewModel : DataBoundModel {

        public OverviewModel( DatabaseContext context ) : base( context ) {
        }

        public Couple Couple { get; set; }

        public IEnumerable<KeyValuePair<string, bool>> MenuItems { get; } = new[] {

            new KeyValuePair<string, bool>( "Aperitief met amuse",  false ),
            new KeyValuePair<string, bool>( "Koud voorgerecht",     true ),
            new KeyValuePair<string, bool>( "Warm voorgerecht",     false ),
            new KeyValuePair<string, bool>( "Hoofdgerecht",         true ),
            new KeyValuePair<string, bool>( "Nagerecht",            true ),
        };

        [BindProperty]
        public Dictionary<string, bool> CourseSelection { get; set; }

        public int CoupleCount { get; set; }

        private async Task<IActionResult> GetCoupleAsync() {

            Couple = await GetAuthorizedCouple();
            if ( Couple == null ) {

                return NotFound();
            }

            if ( Couple.Dinner.SubscriptionStop > DateTime.Now ) {

                return Redirect( ModelPath.Get<Management.EditDinnerModel>() );
            }

            if ( Couple.Dinner.Date < DateTime.Now ) {

                return Redirect( ModelPath.Get<Setup.CreateDinnerModel>() );
            }

            await Database.GetDinnerAsync( Couple.Dinner.ID );

            return null;
        }

        public async Task<IActionResult> OnGetAsync() {

            var getResult = await GetCoupleAsync();
            if ( getResult != null ) {

                return getResult;
            }

            CoupleCount = 0;
            foreach ( Couple couple in Couple.Dinner.Couples ) {

                if ( !Couple.Dinner.HasPrice || couple.HasPayed ) {

                    CoupleCount++;
                }
            }

            CourseSelection = MenuItems.ToDictionary( x => x.Key, x => x.Value );

            return Page();
        }

        public async Task<IActionResult> OnPostAsync() {

            var getResult = await GetCoupleAsync();
            if ( getResult != null ) {

                return getResult;
            }

            Couple[] allCouples = ( await Database.GetCouplesAsync( Couple.Dinner.ID ) ).ToArray();
            if ( Couple.Dinner.HasPrice ) {
                allCouples = allCouples.Where( o => o.HasPayed ).ToArray();
            }
            CoupleCount = allCouples.Length;

            if ( !ModelState.IsValid ) {
                return Page();
            }

            int courseCount = 0;
            foreach ( var entry in CourseSelection ) {

                if ( !entry.Value ) {
                    continue;
                }

                courseCount++;
            }

            if ( courseCount < 2 ) {
                ModelState.AddModelError( "CourseSelection", "Kies minimaal 2 gangen!" );
                return Page();
            }

            int couplesPerGroup     = courseCount;
            int parallelMealCount   = CoupleCount / couplesPerGroup;

            // Rounding errors:
            CoupleCount     = parallelMealCount * couplesPerGroup;
            Schema schema   = Schema.GenerateSchema( allCouples, courseCount );

            if ( ( schema == null ) || !Schema.ValidSchema( schema, allCouples, CoupleCount ) ) {

                // ERROR

                ViewData[ "Schema-error" ] =  "Huidige indeling is niet mogelijk.";
                return Page();
            }

            // Add the remaing couples
            int remainingCoupleCount = allCouples.Length - CoupleCount;
            if ( remainingCoupleCount > 0 ) {

                Couple[] remainingCouples = new Couple[ parallelMealCount ];
                Array.Copy( allCouples, allCouples.Length - remainingCoupleCount, remainingCouples, 0, remainingCoupleCount );

                foreach ( Course course in schema.Courses ) {

                    for ( int i = 0; i < parallelMealCount; i++ ) {

                        course.Meals[ i ].Couples[ course.Meals[ i ].Couples.Length - 1 ] = remainingCouples[ i ];
                    }

                    // Inverse shift
                    var temp = remainingCouples[0];
                    for ( int i = 1; i < remainingCouples.Length; i++ ) {

                        remainingCouples[ i-1 ] = remainingCouples[ i ];
                    }

                    remainingCouples[ remainingCouples.Length - 1 ] = temp;
                }

            }

            // Send emails
            for ( int i = 0; i < schema.Courses.Length; i++ ) {

                foreach ( Meal meal in schema.Courses[ i ].Meals ) {

                    Couple chef = meal.Couples[ i ];

                    StringBuilder emailBody = new StringBuilder();

                    emailBody.Append( $"Hoi { chef.PersonMain.FirstName }" );
                    if ( chef.PersonGuest != null ) {

                        emailBody.Append( $" en { chef.PersonGuest.FirstName }" );
                    }
                    emailBody.AppendLine( "," );

                    int count = 0;
                    foreach ( Couple couple in meal.Couples ) {

                        if ( couple == null ) {
                            continue;
                        }

                        count++;
                        if ( couple.PersonGuest != null ) {
                            count++;
                        }
                    }

                    emailBody.AppendLine( $"Jij/jullie gaan koken in ronde { i + 1 }! Jullie maken een '{ CourseSelection.Keys.ElementAt( i ) }' voor { count } mensen (inclusief jezelf)." );
                    emailBody.AppendLine( "De volgende dieetwensen zijn opgegeven:" );

                    bool hasGuidelines = false;
                    foreach ( Couple couple in meal.Couples ) {

                        if ( couple == null ) {
                            continue;
                        }

                        if ( couple == chef ) {
                            continue;
                        }

                        string guidelines = couple.DietaryGuidelines?.Trim();
                        if ( !string.IsNullOrEmpty( guidelines ) ) {

                            emailBody.AppendLine( guidelines );
                            hasGuidelines = true;
                        }
                    }

                    if ( !hasGuidelines ) {
                        emailBody.AppendLine( "Geen." );
                    }

                    emailBody.AppendLine( "Veel plezier!" );

                    EmailServer.SendEmail( chef.EmailAddress, "Walking dinner!", emailBody.ToString() );
                }
            }

            List<Route> routes = new List<Route>();

            foreach ( Couple couple in allCouples ) {

                for ( int i = 0; i < schema.Courses.Length; i++ ) {

                    Meal meal   = schema.Courses[i].GetMealForCouple( couple );
                    Couple chef = meal.Couples[i];

                    routes.Add( new Route( couple, chef, i ) );
                }
            }

            byte[] pdf = Letter.Generate( routes, courseCount, Couple.Dinner.Date );

            return File( pdf, System.Net.Mime.MediaTypeNames.Application.Pdf, "schema.pdf" );

            //ViewData[ "message" ] = "Schema is opgeslagen.";
            //return Page();
        }
    }
}