using Microsoft.AspNetCore.Http;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using WalkingDinner.Mollie.Models;

namespace WalkingDinner.Mollie {

    // API:
    // https://docs.mollie.com/payments/overview

    public static partial class MollieAPI {

        private static readonly HttpClient client;
        public static readonly string HostURL = $"https://api.mollie.com/v2/";

        private static readonly JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings(){
            ContractResolver = new DefaultContractResolver(){
                NamingStrategy = new CamelCaseNamingStrategy(),
            },
        };

        static MollieAPI() {

            client    = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue( "bearer", Key );

            client.BaseAddress = new Uri( HostURL );
        }

        private static StringContent JSONContent( object content ) {

            string data = JsonConvert.SerializeObject( content, jsonSerializerSettings );
            return new StringContent( data, Encoding.UTF8, "application/json" );
        }

        public static async Task<T> PostAsync<T>( string path, object Object ) where T : class {

            try {
                StringContent content = JSONContent( Object );
                HttpResponseMessage response = await client.PostAsync( path, content );

                bool success    = response.IsSuccessStatusCode;
                string body     = await response.Content.ReadAsStringAsync();

                if ( !success ) {
                    return null;
                }

                return JsonConvert.DeserializeObject<T>( body, jsonSerializerSettings );

            } catch ( Exception exp ) {
                Console.WriteLine( exp.Message );
                return null;
            }
        }

        // private static 

        public static async Task<MollieResponse> TestRequest() {

            Payment payment = new Payment(){
                Amount = new Amount( Currency.EUR, 100.00 ),
                Description = "Een test betaling",
                RedirectUrl = "https://www.google.com",
                // Testmode = true,
            };

            return await PaymentRequest( payment );
        }

        public static async Task<MollieResponse> PaymentRequest( Payment payment ) {

            return await PostAsync<MollieResponse>( "payments", payment );
        }

        public static async Task<string> GetPaymentStatus( string paymentId ) {

            if ( string.IsNullOrEmpty( paymentId ) ) {

                return string.Empty;
            }

            dynamic result = await PostAsync<dynamic>( $"payments/{ paymentId }", new {} );
            if ( result == null ) {
                return null;
            }

            string status = result.status;
            return status?.ToLower();
        }

        public static async Task<MollieResponse> CreateRefund( string paymentId, double amount ) {

            if ( string.IsNullOrEmpty( paymentId ) ) {

                return null;
            }

            return await PostAsync<MollieResponse>( $"payments/{ paymentId }/refunds", new { Amount = new Amount( Currency.EUR, amount ) } );
        }

        public static string TranslateStatus( string paymentStatus ) {

            switch ( paymentStatus ) {

                case "canceled":
                    return "Geannuleerd";

                case "expired":
                    return "Verlopen";

                case "failed":
                    return "Mislukt";

                case "pending":
                    return "Is behandeling";

                case "paid":
                    return "Betaald";
            }

            return "Geen";
        }

        public static bool TransactionPending( string paymentStatus ) {

            switch ( paymentStatus ) {

                case "paid":
                case "pending":
                    return true;
                default:
                    return false;
            }
        }
    }
}
