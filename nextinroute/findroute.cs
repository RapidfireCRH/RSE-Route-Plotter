using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nextinroute
{
    class findroute
    {
        /// <summary>
        ///               N
        ///               |
        ///               |
        ///       Q3      |       Q2
        ///      (-+)     |      (++)
        ///               |
        /// W----------------------------E
        ///               |
        ///               |
        ///       Q4      |       Q1
        ///      (--)     |      (+-)
        ///               |
        ///               S
        /// Directions require a quadrent with an emphasis to make sure dev follows correct path
        /// to determine emphasis, find longest distance of sepration (x, z). The other becomes the dev axis
        /// </summary>

        enum emphasis { north_south, east_west };
        enum direct { Q1, Q2, Q3, Q4 };
        public struct coord_st
        {
            public float x;
            public float y;
            public float z;
        }
        public struct star_st
        {
            public string name;
            public coord_st coord;
        }
        public static star_st findnext(star_st curr, star_st dest)
        {
            float dist = (float)Math.Sqrt(Math.Pow(curr.coord.x - dest.coord.x, 2) + Math.Pow(curr.coord.y - dest.coord.y, 2) + Math.Pow(curr.coord.z - dest.coord.z, 2));
            float dev = dist / 3;
            direct dir = direct.Q1;
            if (curr.coord.x > dest.coord.x)//-?
            {
                if (curr.coord.z > dest.coord.z)//--
                    dir = direct.Q4;
                else//-+
                    dir = direct.Q3;
            }
            else//+?
            {

                if (curr.coord.z > dest.coord.z)//+-
                    dir = direct.Q1;
                else//++
                    dir = direct.Q2;
            }
            emphasis emp = (Math.Abs(curr.coord.x - dest.coord.x) > Math.Abs(curr.coord.z - dest.coord.z)) ? emphasis.east_west : emphasis.north_south;
            return new star_st();
        }
    }
}