using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using WalkingDinner.Models;

namespace WalkingDinner.Database {

    public class DatabaseManager {

        public readonly DatabaseContext Database;
        private static readonly RNGCryptoServiceProvider RNG = new RNGCryptoServiceProvider();

        private static string EncodeBytes( byte[] data ) {

            return Convert.ToBase64String( data ).Replace( '+', '-' ).Replace( '/', '.' ).TrimEnd( '=' );
        }

        private static string GenerateAccessCode() {

            byte[] data = new byte[32];
            RNG.GetBytes( data );

            return EncodeBytes( data ).Substring( 0, 40 );
        }

        public DatabaseManager( DatabaseContext context ) {

            Database = context;
        }

        public async Task<Dinner> CreateDinnerAsync( Dinner dinner, Couple adminCouple ) {

            adminCouple.EmailAddress    = adminCouple.EmailAddress?.ToLower();
            adminCouple.AdminCode       = GenerateAccessCode();
            adminCouple.IsAdmin         = true;
            adminCouple.Validated       = true;

            dinner.Couples.Add( adminCouple );

            Database.Add( dinner );
            await Database.SaveChangesAsync();

            return dinner;
        }

        public async Task<Dinner> GetDinnerAsync( int Id ) {

            Dinner dinner = await Database.Dinners.Include( o => o.Couples ).ThenInclude( o => o.PersonMain )
                                                  .Include( o => o.Couples ).ThenInclude( o => o.PersonGuest )
                                                  .Include( o => o.Couples ).ThenInclude( o => o.Address )
                                                  .Include( o => o.Address )
                                                  .SingleOrDefaultAsync( o => o.ID == Id );
            if ( dinner == null ) {

                return null;
            }

            return dinner;
        }

        public async Task<Dinner> RemoveDinnerAsync( int Id ) {

            Dinner dinner = await Database.Dinners.FindAsync( Id );
            if ( dinner == null ) {
                return null;
            }

            Database.Remove( dinner );
            await Database.SaveChangesAsync();

            return dinner;
        }

        public async Task<Dinner> UpdateDinnerAsync( int Id, Dinner dinnerData ) {

            Dinner dinner = await GetDinnerAsync( Id );
            if ( dinner == null ) {
                return null;
            }

            dinner.CopyFrom( dinnerData );
            await Database.SaveChangesAsync();

            return dinner;
        }

        public async Task<Couple> CreateCoupleAsync( int dinnerID, string emailAddress, Person person ) {

            Dinner dinner = await Database.Dinners.FindAsync( dinnerID );
            if ( dinner == null ) {
                return null;
            }

            Couple couple = new Couple(){
                EmailAddress    = emailAddress?.ToLower(),
                AdminCode       = GenerateAccessCode(),
                PersonMain      = (PersonMain)person,
            };

            dinner.Couples.Add( couple );
            await Database.SaveChangesAsync();

            return couple;
        }

        public async Task<Couple> GetCoupleAsync( int Id ) {

            if ( Id < 1 ) {

                return null;
            }

            Couple couple = await Database.Couples.Include( o => o.Dinner ).ThenInclude( o => o.Address )
                                                  .Include( o => o.PersonMain )
                                                  .Include( o => o.PersonGuest )
                                                  .Include( o => o.Address )
                                                  .SingleOrDefaultAsync( o => o.ID == Id );
            if ( couple == null ) {
                return null;
            }

            return couple;
        }

        public async Task<Couple> GetCoupleAsAdminAsync( int Id, string AdminCode ) {

            Couple couple = await GetCoupleAsync( Id );
            if ( couple == null ) {

                return null;
            }

            if ( couple.AdminCode != AdminCode ) {

                return null;
            }

            return couple;
        }

        public async Task<IEnumerable<Couple>> GetCouplesAsync( int dinnerId ) {

            return await Database.Couples.Where( o => o.Dinner.ID == dinnerId ).ToListAsync();
        }

        public async Task<Couple> UpdateCoupleAsync( int Id, Couple coupleData ) {

            Couple couple = await GetCoupleAsync( Id );
            if ( couple == null ) {
                return null;
            }

            couple.CopyFrom( coupleData );
            couple.Validated = true;

            await Database.SaveChangesAsync();

            return couple;
        }

        public async Task<Couple> RemoveCoupleAsync( int Id ) {

            Couple couple = await Database.Couples.FindAsync( Id );
            if ( couple == null ) {
                return null;
            }

            Database.Remove( couple );
            await Database.SaveChangesAsync();

            return couple;
        }

        public async Task SaveChangesAsync() {

            await Database.SaveChangesAsync();
        }

    }
}
