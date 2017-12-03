using System;
using System.Collections.Generic;
using System.IO;
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
            findroute.star_st ret = start;
            List<findroute.star_st> list = new List<findroute.star_st>();
            list.Add(start);
            Console.Clear();
            Console.WriteLine(ret.name);
            Console.WriteLine("X: " + ret.coord.x);
            Console.WriteLine("Y: " + ret.coord.y);
            Console.WriteLine("Z: " + ret.coord.z);
            while ((ret = fr.findnext(ret, end)).name != end.name)
            {
                Console.Clear();
                Console.WriteLine((list.Count + 1) + "." + ret.name);
                Console.WriteLine("X: " + ret.coord.x);
                Console.WriteLine("Y: " + ret.coord.y);
                Console.WriteLine("Z: " + ret.coord.z);
                Console.WriteLine("Distance from previous: " + Math.Floor(Math.Sqrt(Math.Pow(list[list.Count - 1].coord.x - ret.coord.x, 2) + Math.Pow(list[list.Count - 1].coord.y - ret.coord.y, 2) + Math.Pow(list[list.Count - 1].coord.z - ret.coord.z, 2))));
                list.Add(ret);
            }
            int y = 0;
            foreach (findroute.star_st x in list)
                Console.WriteLine(++y + ": " + x.name + " X:" + x.coord.x + " Y:" + x.coord.y + " Z:" + x.coord.z);

            StringBuilder bldr = new StringBuilder();
            foreach (findroute.star_st x in list)
            {
                bldr.AppendLine(x.name + ", " + x.coord.x + ", " + x.coord.y + ", " + x.coord.z);
            }
            File.WriteAllText("temp.txt", bldr.ToString());
            Console.Read();
        }
    }
}


