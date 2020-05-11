using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WalkingDinner.Mollie.Models {
    public class Payment {

        public Amount Amount { get; set; }
        public string Description { get; set; }

        public string RedirectUrl { get; set; }
    }
}
