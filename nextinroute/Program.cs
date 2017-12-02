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
            findroute.star_st start = new findroute.star_st();
            findroute.star_st end = new findroute.star_st();
            start.name = "Sol";
            end.name = "Colonia";
            end.coord.x = -9530.5;
            end.coord.y = -910.28125;
            end.coord.z = 19808.125;
            findroute.star_st ret = new findroute.star_st();
            List<findroute.star_st> list = new List<findroute.star_st>();
            while((ret = fr.findnext(ret,end)).name!= end.name)
                list.Add(ret);
            int y = 0;
            foreach (findroute.star_st x in list)
                Console.WriteLine(++y + ": " + x.name + " X:" + x.coord.x + " Y:" + x.coord.y + " Z:" + x.coord.z);
            Console.Read();
        }
    }
}


