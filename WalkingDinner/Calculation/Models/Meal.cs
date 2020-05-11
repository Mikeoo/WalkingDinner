using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WalkingDinner.Models;

namespace WalkingDinner.Calculation.Models {

    public class Meal {

        public Couple[] Couples { get; }
        public int CoupleCount { get; }

        public Meal( int count ) {

            Couples     = new Couple[ count + 1 ];
            CoupleCount = count;
        }
    }
}
