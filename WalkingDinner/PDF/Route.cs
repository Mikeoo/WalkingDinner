using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WalkingDinner.Models;

namespace WalkingDinner.PDF {

    public class Route {

        public Couple Subject { get; set; }
        public Couple To { get; set; }
        public int CourseIndex { get; set; }

        public Route( Couple subject, Couple to, int courseIndex ) {

            Subject = subject;
            To      = to;
            CourseIndex = courseIndex;
        }
    }
}
