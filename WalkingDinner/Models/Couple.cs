using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using WalkingDinner.Mollie;
using WalkingDinner.Mollie.Models;

namespace WalkingDinner.Models {

    public class Couple : DatabaseRecord<Couple> {

        public Dinner Dinner { get; set; }
        public int DinnerID { get; }

        [Required]
        public PersonMain PersonMain { get; set; }

        public PersonGuest PersonGuest { get; set; }

        [Required]
        [Display( Name = "Emailadres" )]
        public string EmailAddress { get; set; }

        [DataType( DataType.MultilineText )]
        [Display( Name = "Dieetwensen / Allergieën" )]
        public string DietaryGuidelines { get; set; }

        [DataType( DataType.PhoneNumber )]
        [Display( Name = "Telefoonnummer" )]
        public string PhoneNumber { get; set; }

        [Display( Name = "Rekeningnummer" )]
        public string IBAN { get; set; }

        public CoupleAddress Address { get; set; }

        public string PaymentId { get; set; }

        public string PaymentStatus { get; set; }

        [NotMapped]
        public bool HasPayed { get => PaymentStatus?.ToLower() == "paid"; }

        [Display( Name = "Profiel ingevuld" )]
        public bool Validated { get; set; }

        public bool IsAdmin { get; set; }

        public string AdminCode { get; set; }

        public override void CopyFrom( Couple source ) {

            if ( source == null ) {
                return;
            }

            PersonMain.CopyFrom( source.PersonMain );
            if ( PersonGuest != null ) {

                if ( source.PersonGuest != null ) {

                    PersonGuest.CopyFrom( source.PersonGuest );
                } else {

                    PersonGuest = null;
                }
            } else {
                PersonGuest = source.PersonGuest;
            }

            EmailAddress        = source.EmailAddress;
            DietaryGuidelines   = source.DietaryGuidelines;
            PhoneNumber         = source.PhoneNumber;
            IBAN                = source.IBAN;

            if ( Address != null ) {

                if ( source.Address != null ) {

                    Address.CopyFrom( source.Address );
                } else {

                    Address = null;
                }
            } else {
                Address = source.Address;
            }
        }

        public string GetName() {

            string result = PersonMain.FirstName;
            if ( PersonGuest != null ) {
                result += $" en { PersonGuest.FirstName }";
            }

            return result;
        }

        public struct PaymentUpdate {
            public string OldStatus { get; set; }
            public string NewStatus { get; set; }
            public bool Changed { get => OldStatus != NewStatus; }
        }

        public async Task<PaymentUpdate> UpdatePaymentStatus() {

            PaymentUpdate update = new PaymentUpdate();

            if ( string.IsNullOrEmpty( PaymentId ) ) {
                return update;
            }

            update.OldStatus = PaymentStatus;

            if ( HasPayed ) {

                update.NewStatus = update.OldStatus;
                return update;
            }

            update.NewStatus = await MollieAPI.GetPaymentStatus( PaymentId );

            if ( update.Changed ) {
                PaymentStatus = update.NewStatus;
            }

            return update;
        }
    }
}
