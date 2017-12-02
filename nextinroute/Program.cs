using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nextinroute
{
    class Program
    {
        static void Main(string[] args)
        {
            findroute fr = new findroute();
            fr.findnext(new findroute.star_st(), new findroute.star_st());
        }
    }
}
