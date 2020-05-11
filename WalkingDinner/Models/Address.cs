using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WalkingDinner.Models {
    public class Address : DatabaseRecord<Address> {

        [Required]
        [Display( Name = "Straat" )]
        public string Street { get; set; }

        [Required]
        [Display( Name = "Nummer" )]
        public int Number { get; set; }

        [Display( Name = "Toevoeging" )]
        public string NumberSuffix { get; set; }

        [Required]
        [Display( Name = "Postcode" )]
        [DataType( DataType.PostalCode )]
        public string ZipCode { get; set; }

        [Required]
        [Display( Name = "Plaats" )]
        public string Place { get; set; }

        [DataType( DataType.MultilineText )]
        [Display( Name = "Extra adres regel" )]
        public string Extra { get; set; }

        public override void CopyFrom( Address source ) {

            if ( source == null ) {
                return;
            }

            Street = source.Street;
            Number = source.Number;
            NumberSuffix = source.NumberSuffix;
            ZipCode = source.ZipCode;
            Place = source.Place;
            Extra = source.Extra;
        }

        public string GetAsLine() {

            string result = $"{ Street } {Number}";
            if ( !string.IsNullOrEmpty( NumberSuffix ) ) {
                result += NumberSuffix.ToUpper();
            }

            return result;
        }
    }

    public class CoupleAddress : Address {

        public Couple Couple { get; set; }
        public int CoupleID { get; }
    }

    public class DinnerAddress : Address {

        public Dinner Dinner { get; set; }
        public int DinnerID { get; }
    }
}
