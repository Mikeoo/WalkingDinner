using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace WalkingDinner.Mollie.Models {

    public enum Currency {

        EUR,
        USD,
    }

    public class Amount {

        public string Currency { get; set; }
        public string Value { get; set; }

        public Amount() { }

        public Amount( Currency currency, double value ) {
            Currency    = currency.ToString();
            Value       = Math.Round( value, 2 ).ToString( "#.00", System.Globalization.CultureInfo.InvariantCulture );
        }

        public override string ToString() {

            return $"{ Value } { Currency }";
        }
    }
}
