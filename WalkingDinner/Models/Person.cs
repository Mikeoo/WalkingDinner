using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WalkingDinner.Models {

    public class Person : DatabaseRecord<Person> {

        public Couple Couple { get; set; }
        public int CoupleID { get; }

        [Required]
        [Display( Name = "Voornaam" )]
        public string FirstName { get; set; }

        [Display( Name = "Tussenvoegsel" )]
        public string Preposition { get; set; }

        [Required]
        [Display( Name = "Achternaam" )]
        public string LastName { get; set; }

        public override void CopyFrom( Person source ) {

            if ( source == null ) {
                return;
            }

            FirstName = source.FirstName;
            Preposition = source.Preposition;
            LastName = source.LastName;
        }

        public override string ToString() {

            string result = FirstName + ' ';
            if ( !string.IsNullOrWhiteSpace( Preposition ) ) {
                result += Preposition + ' ';
            }

            return result + LastName;
        }
    }

    public class PersonMain : Person {

    }

    public class PersonGuest : Person {

    }
}
