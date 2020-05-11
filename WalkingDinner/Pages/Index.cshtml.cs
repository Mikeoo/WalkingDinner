using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using WalkingDinner.Database;
using WalkingDinner.Models;

namespace WalkingDinner.Pages {

    public class IndexModel : DataBoundModel {

        public IndexModel( DatabaseContext context ) : base( context ) {
        }

        public IActionResult OnGet() {

            PersonMain personA = new PersonMain(){
                ID = 10,
            };

            PersonMain personB = new PersonMain(){
                ID = 10,
            };

            bool x = (Person)personA == (Person)personB;

            return Page();
        }
    }
}
