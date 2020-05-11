using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WalkingDinner.Models;

namespace WalkingDinner.Calculation.Models {

    public class Course {

        public Meal[] Meals { get; set; }
        public int CouplesPerMeal { get; }

        public static Course SeedCourse( Couple[] allCouples, int mealCount, int couplesPerMeal ) {

            Course course = new Course( mealCount, couplesPerMeal );

            int index = 0;
            foreach ( Meal meal in course.Meals ) {

                for ( int i = 0; i < meal.CoupleCount; i++ ) {

                    meal.Couples[ i ] = allCouples[ index ];
                    index++;
                }
            }

            return course;
        }

        public static Course CopyFrom( Course source ) {

            Course course = new Course( source.Meals.Length, source.CouplesPerMeal );
            for ( int i = 0; i < source.Meals.Length; i++ ) {

                Array.Copy( source.Meals[ i ].Couples, course.Meals[ i ].Couples, source.Meals[ i ].CoupleCount );
            }

            return course;
        }

        public void Shift( int whichCouple, int howMuch ) {

            howMuch %= Meals.Length;
            if ( howMuch < 1 ) {
                return;
            }

            // Setup an array
            Couple[] array = new Couple[Meals.Length];
            for ( int i = 0; i < Meals.Length; i++ ) {
                array[ i ] = Meals[ i ].Couples[ whichCouple ];
            }

            // Break up the array
            Couple[] partA = new Couple[howMuch];
            Couple[] partB = new Couple[Meals.Length - howMuch];

            Array.Copy( array, 0, partA, 0, howMuch );
            Array.Copy( array, howMuch, partB, 0, Meals.Length - howMuch );

            // Reconstruct the array
            Array.Copy( partB, 0, array, 0, partB.Length );
            Array.Copy( partA, 0, array, partB.Length, partA.Length );

            // Copy the results back
            for ( int i = 0; i < Meals.Length; i++ ) {
                Meals[ i ].Couples[ whichCouple ] = array[ i ];
            }
        }

        public Meal GetMealForCouple( Couple couple ) {

            foreach ( Meal meal in Meals ) {

                if ( meal.Couples.Any( o => o == couple ) ) {
                    return meal;
                }
            }

            return null;
        }

        public Course( int count, int couplesPerMeal ) {

            CouplesPerMeal  = couplesPerMeal;
            Meals           = new Meal[ count ];

            for ( int i = 0; i < Meals.Length; i++ ) {

                Meals[ i ] = new Meal( couplesPerMeal );
            }
        }
    }
}
