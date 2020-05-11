using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using WalkingDinner.Models;

namespace WalkingDinner.Database {

    public class DatabaseContext : DbContext {

        public DatabaseContext( [NotNull] DbContextOptions options ) : base( options ) {

            string[] commandLineArgs    = Environment.GetCommandLineArgs();
            string executable           = commandLineArgs[0];
            executable                  = System.IO.Path.GetFileName( executable ).ToLower();

            if ( executable == "ef.dll" && commandLineArgs[ 1 ]?.ToLower() == "database" && commandLineArgs[ 2 ]?.ToLower() == "update" ) {

                Console.Out.WriteLine( "\nDatabase Update Detected\n" );
                this.Database.EnsureDeleted();
            }

            this.Database.EnsureCreated();
        }

        public DbSet<Dinner> Dinners { get; set; }
        public DbSet<Couple> Couples { get; set; }
        public DbSet<Person> Persons { get; set; }
        public DbSet<Address> Addresses { get; set; }

        protected override void OnModelCreating( ModelBuilder modelBuilder ) {

            modelBuilder.Entity<Person>( table => {

                table.HasIndex( o => o.CoupleID ).IsUnique( false );
            } );

            modelBuilder.Entity<Dinner>( table => {

                table.HasOne( o => o.Address )
                     .WithOne( o => o.Dinner )
                     .HasForeignKey<DinnerAddress>( o => o.DinnerID )
                     .OnDelete( DeleteBehavior.ClientCascade )
                     ;
            } );

            modelBuilder.Entity<Couple>( table => {

                table.HasOne( o => o.PersonMain )
                     .WithOne( o => o.Couple )
                     .HasForeignKey<PersonMain>( o => o.CoupleID )
                     .OnDelete( DeleteBehavior.ClientCascade )
                     ;

                table.HasOne( o => o.PersonGuest )
                     .WithOne( o => o.Couple )
                     .HasForeignKey<PersonGuest>( o => o.CoupleID )
                     .OnDelete( DeleteBehavior.ClientCascade )
                     ;

                table.HasOne( o => o.Dinner )
                     .WithMany( o => o.Couples )
                     .HasForeignKey( o => o.DinnerID )
                     ;

                table.HasOne( o => o.Address )
                     .WithOne( o => o.Couple )
                     .HasForeignKey<CoupleAddress>( o => o.CoupleID )
                     ;
            } );

            base.OnModelCreating( modelBuilder );
        }
    }
}
