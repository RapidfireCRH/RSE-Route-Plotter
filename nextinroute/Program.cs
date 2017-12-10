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
            findroute.star_st start = fr.searchbyname("PYRIE EUQ EW-E C11-0");
            findroute.star_st end = fr.searchbyname("BLU AOC SS-L B21-1");
            if (end.name == null || start.name == null)
                throw new Exception("unknown name");
            findroute.star_st ret = start;
            List<findroute.star_st> list = new List<findroute.star_st>();
            list.Add(start);
            Console.Clear();
            Console.WriteLine(ret.name);
            Console.WriteLine("X: " + ret.coord.x);
            Console.WriteLine("Y: " + ret.coord.y);
            Console.WriteLine("Z: " + ret.coord.z);
            double totaldist = 0;
            while ((ret = fr.findnext(ret, end)).name != end.name)
            {
                Console.Clear();
                Console.WriteLine((list.Count + 1) + "." + ret.name);
                Console.WriteLine("X: " + ret.coord.x);
                Console.WriteLine("Y: " + ret.coord.y);
                Console.WriteLine("Z: " + ret.coord.z);
                Console.WriteLine("Distance from previous: " + Math.Floor(Math.Sqrt(Math.Pow(list[list.Count - 1].coord.x - ret.coord.x, 2) + Math.Pow(list[list.Count - 1].coord.y - ret.coord.y, 2) + Math.Pow(list[list.Count - 1].coord.z - ret.coord.z, 2)))+ "ly");
                totaldist += Math.Sqrt(Math.Pow(list[list.Count - 1].coord.x - ret.coord.x, 2) + Math.Pow(list[list.Count - 1].coord.y - ret.coord.y, 2) + Math.Pow(list[list.Count - 1].coord.z - ret.coord.z, 2));
                list.Add(ret);
            }
            totaldist += Math.Sqrt(Math.Pow(list[list.Count - 1].coord.x - end.coord.x, 2) + Math.Pow(list[list.Count - 1].coord.y - end.coord.y, 2) + Math.Pow(list[list.Count - 1].coord.z - end.coord.z, 2));
            list.Add(end);

            StringBuilder bldr = new StringBuilder();
            bldr.AppendLine("As the Crow Flys: " + Math.Sqrt(Math.Pow(start.coord.x - end.coord.x, 2) + Math.Pow(start.coord.y - end.coord.y, 2) + Math.Pow(start.coord.z - end.coord.z, 2)) +" and total: "+totaldist);
            foreach (findroute.star_st x in list)
            {
                bldr.AppendLine(x.name + ", " + x.coord.x + ", " + x.coord.y + ", " + x.coord.z);
            }
            File.WriteAllText(start.name+"-"+end.name+"route.txt", bldr.ToString());
        }
    }
}


