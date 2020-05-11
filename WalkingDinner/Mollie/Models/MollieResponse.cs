using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WalkingDinner.Mollie.Models {


    public class MollieResponse {

        public class Link {

            public string Href { get; set; }

            public string Type { get; set; }
        }

        public string Resource { get; set; }

        public string Id { get; set; }

        public string Mode { get; set; }

        public DateTime CreatedAt { get; set; }

        public Amount Amount { get; set; }

        public string Description { get; set; }

        public dynamic Method { get; set; }

        public dynamic MetaData { get; set; }

        public string PaymentId { get; set; }

        public string Status { get; set; }

        public bool IsCancelable { get; set; }

        public DateTime ExpiresAt { get; set; }

        public dynamic Details { get; set; }

        public string ProfileId { get; set; }

        public string SequenceType { get; set; }

        public string RedirectUrl { get; set; }

        public string WebhookUrl { get; set; }

        public Dictionary<string, Link> _links { get; set; }

        public string GetLink( string key ) {

            if ( _links == null ) {
                return null;
            }

            if ( !_links.ContainsKey( key ) ) {

                return null;
            }

            return _links[ key ].Href;
        }
    }
}
