using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WalkingDinner.Models;

namespace WalkingDinner.Calculation.Models {

    public class Schema {

        private static Random random = new Random();

        private static void ShuffleArray<T>( ref T[] array ) {

            for ( int i = 0; i < array.Length; i++ ) {

                int newIndex = random.Next( 0, array.Length );

                // Swap
                T temp              = array[ i ];
                array[ i ]          = array[ newIndex ];
                array[ newIndex ]   = temp;
            }
        }

        private static bool IsValidShuffle( int index, int courseCount, int parallelCount ) {

            int shuffle = 0;

            for ( int i = 1; i < courseCount; i++ ) {

                shuffle = ( shuffle + index ) % parallelCount;
                if ( shuffle == 0 ) {
                    return false;
                }
            }

            return true;
        }

        private static IEnumerable<int> GetValidShuffles( int courseCount, int parallelCount ) {

            // '0' is always valid
            // yield return 0;

            for ( int i = 1; i < parallelCount; i++ ) {

                if ( !IsValidShuffle( i, courseCount, parallelCount ) ) {
                    continue;
                }

                yield return i;
            }
        }

        public static Schema GenerateSchema( Couple[] allCouples, int courseCount ) {

            ShuffleArray( ref allCouples );
            int coupleCount     = allCouples.Length;

            int parallelCount   = coupleCount / courseCount;
            int couplesPerMeal  = courseCount;
            int totalCouples    = couplesPerMeal * parallelCount;

            int newPeoplePerGroup = couplesPerMeal - 1;
            if ( newPeoplePerGroup * courseCount > totalCouples ) { // impossible
                return null;
            }

            int[] validShuffles = GetValidShuffles( courseCount, parallelCount ).ToArray();
            if ( validShuffles.Length + 1 < courseCount ) { // '0' is always valid

                return null;
            }

            Schema schema       = new Schema( courseCount, couplesPerMeal );
            schema.Courses[ 0 ] = Course.SeedCourse( allCouples, parallelCount, couplesPerMeal );

            for ( int i = 1; i < courseCount; i++ ) {

                Course course = Course.CopyFrom( schema.Courses[ 0 ] );

                for ( int j = 1; j < couplesPerMeal; j++ ) {

                    int shiftcount = (j * validShuffles[i-1]) % parallelCount;

                    course.Shift( j, shiftcount );
                }

                schema.Courses[ i ] = course;
            }

            return schema;
        }

        private static bool ValidSchema( Schema schema, Couple couple ) {

            List<Couple> metCouples = new List<Couple>();

            for ( int i = 0; i < schema.Courses.Length; i++ ) {

                Meal meal = schema.Courses[i].GetMealForCouple( couple );
                if ( meal == null ) {
                    throw new Exception( "Cannot locate couple in a course." );
                }

                foreach ( Couple otherCouple in meal.Couples ) {

                    if ( otherCouple == null ) {
                        continue;
                    }

                    if ( couple == otherCouple ) {
                        continue;
                    }

                    if ( metCouples.Any( o => o == otherCouple ) ) {

                        return false;
                    }

                    metCouples.Add( otherCouple );
                }
            }

            return true;
        }

        public static bool ValidSchema( Schema schema, Couple[] allCouples, int coupleCount ) {

            for ( int i = 0; i < coupleCount; i++ ) {

                if ( !ValidSchema( schema, allCouples[ i ] ) ) {

                    return false;
                }
            }

            return true;
        }

        public Course[] Courses { get; }
        public int CouplesPerMeal { get; }

        public Schema( int count, int couplesPerMeal ) {

            CouplesPerMeal  = couplesPerMeal;
            Courses         = new Course[ count ];
        }
    }
}
