using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WalkingDinner.Extensions;
using System.Linq;
using System.Threading.Tasks;

namespace WalkingDinner.Models {

    public class Dinner : DatabaseRecord<Dinner> {

        public const int MIN_DAYS_IN_ADVANCE = 7;

        [Required]
        [StringLength( maximumLength: 20 )]
        [Display( Name = "Title" )]
        public string Title { get; set; }

        [Required]
        [DataType( DataType.MultilineText )]
        [Display( Name = "Beschrijving" )]
        public string Description { get; set; }

        [Required]
        [DataType( DataType.DateTime )]
        [Display( Name = "Datum van dinner" )]
        public DateTime Date { get; set; }

        [Required]
        [DataType( DataType.DateTime )]
        [Display( Name = "Uiterste aanmelddatum" )]
        public DateTime SubscriptionStop { get; set; }

        [Required]
        [DataType( DataType.Currency )]
        [Display( Name = "Prijs per deelnemer" )]
        public double Price { get; set; }

        [Required]
        public DinnerAddress Address { get; set; }

        [Required]
        public IList<Couple> Couples { get; set; }

        [NotMapped]
        public bool HasPrice { get => Price > 0.0; }

        public Dinner() {

            Couples = new List<Couple>();
        }

        public override void CopyFrom( Dinner source ) {

            if ( source == null ) {
                return;
            }

            Title               = source.Title;
            Description         = source.Description;
            Date                = source.Date;
            SubscriptionStop    = source.SubscriptionStop;
            Price               = source.Price;

            Address.CopyFrom( source.Address );
        }
    }
}
