using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WalkingDinner.Models {

    public interface IDatabaseRecord {

        public int ID { get; set; }
    }

    public abstract class DatabaseRecord<T> : IDatabaseRecord {

        [DatabaseGenerated( DatabaseGeneratedOption.Identity ), Key]
        public int ID { get; set; }

        public abstract void CopyFrom( T source );

        public override bool Equals( object obj ) {

            if ( obj == null ) {
                return false;
            }

            if ( GetType() != obj.GetType() ) {
                return false;
            }

            return ID == ( obj as IDatabaseRecord )?.ID;
        }

        public override int GetHashCode() {
            return HashCode.Combine( ID );
        }

        public static bool operator ==( DatabaseRecord<T> A, DatabaseRecord<T> B ) {

            if ( (object)A == null ) {

                return (object)B == null;
            }

            return A.Equals( B );
        }

        public static bool operator !=( DatabaseRecord<T> A, DatabaseRecord<T> B ) {
            return !( A == B );
        }
    }
}
