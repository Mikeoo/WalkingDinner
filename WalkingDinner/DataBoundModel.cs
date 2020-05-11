using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WalkingDinner.Database;
using WalkingDinner.Models;

namespace WalkingDinner.Pages {

    public abstract class DataBoundModel : PageModel {

        protected DatabaseManager Database;

        private int? _AuthorizedID;
        protected int AuthorizedID {
            get {

                if ( _AuthorizedID == null ) {

                    string value = User?.FindFirst( ClaimTypes.NameIdentifier )?.Value;
                    if ( !int.TryParse( value, out int result ) ) {
                        return -1;
                    }

                    _AuthorizedID = result;
                }

                return _AuthorizedID.Value;
            }
        }

        protected async Task<Couple> GetAuthorizedCouple() {

            Couple couple = await Database.GetCoupleAsync( AuthorizedID );
            if ( couple == null ) {
                return null;
            }

            return couple;
        }

        public DataBoundModel( DatabaseContext context ) {

            Database = new DatabaseManager( context );
        }
    }
}
