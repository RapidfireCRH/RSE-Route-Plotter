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
        static findroute.star_st start = new findroute.star_st();
        static findroute.star_st end = new findroute.star_st();
        static findroute fr = new findroute();
        static int ver_major = 0;
        static int ver_minor = 1;
        static DateTime create_date = new DateTime(2017, 12, 10);
        static int dev = 20;
        static int min = 0;
        static void Main(string[] args)
        {
            if (!scanargs(args))
                return;
            if (end.name == null || start.name == null)
                return;
            findroute.star_st ret = start;
            List<findroute.star_st> list = new List<findroute.star_st>();
            list.Add(start);
            Console.Clear();
            Console.WriteLine(ret.name);
            Console.WriteLine("X: " + ret.coord.x);
            Console.WriteLine("Y: " + ret.coord.y);
            Console.WriteLine("Z: " + ret.coord.z);
            double totaldist = 0;
            while ((ret = fr.findnext(ret, end, dev, min)).name != end.name)
            {
                Console.Clear();
                Console.WriteLine((list.Count + 1) + "." + ret.name);
                Console.WriteLine("X: " + ret.coord.x);
                Console.WriteLine("Y: " + ret.coord.y);
                Console.WriteLine("Z: " + ret.coord.z);
                Console.WriteLine("Distance from previous: " + Math.Floor(Math.Sqrt(Math.Pow(list[list.Count - 1].coord.x - ret.coord.x, 2) + Math.Pow(list[list.Count - 1].coord.y - ret.coord.y, 2) + Math.Pow(list[list.Count - 1].coord.z - ret.coord.z, 2))) + "ly");
                totaldist += Math.Sqrt(Math.Pow(list[list.Count - 1].coord.x - ret.coord.x, 2) + Math.Pow(list[list.Count - 1].coord.y - ret.coord.y, 2) + Math.Pow(list[list.Count - 1].coord.z - ret.coord.z, 2));
                list.Add(ret);
            }
            totaldist += Math.Sqrt(Math.Pow(list[list.Count - 1].coord.x - end.coord.x, 2) + Math.Pow(list[list.Count - 1].coord.y - end.coord.y, 2) + Math.Pow(list[list.Count - 1].coord.z - end.coord.z, 2));
            list.Add(end);

            StringBuilder bldr = new StringBuilder();
            bldr.AppendLine("As the Crow Flys: " + Math.Sqrt(Math.Pow(start.coord.x - end.coord.x, 2) + Math.Pow(start.coord.y - end.coord.y, 2) + Math.Pow(start.coord.z - end.coord.z, 2)) + " and total: " + totaldist + " | RSE stars eliminated: " + (list.Count -2));
            foreach (findroute.star_st x in list)
            {
                bldr.AppendLine(x.name + ", " + x.coord.x + ", " + x.coord.y + ", " + x.coord.z);
            }
            File.WriteAllText(start.name + "-" + end.name + "route.txt", bldr.ToString());
            Console.WriteLine();
            Console.WriteLine("This is also available here: " + start.name + "-" + end.name + "route.txt");
        }
        static bool scanargs(string[] args)
        {
            if (args.Length < 4)
            {
                gethelp();
                return false;
            }
            switch (args[0].ToLower())
            {
                case "-u":
                    start = fr.currentlocation(args[1]);
                    if (start.name == null)
                        Console.WriteLine("Error parcing " + args[1]);
                    break;
                case "-s":
                    start = fr.searchbyname(args[1]);
                    if (start.name == null)
                        Console.WriteLine("Error parcing " + args[1]);
                    break;
                case "-h":
                case "-help":
                case "/help":
                    gethelp();
                    return false;
                default:
                    Console.WriteLine("Error parcing " + args[0]);
                    return false;
            }
            switch (args[2].ToLower())
            {
                case "-u":
                    end = fr.currentlocation(args[3]);
                    if (end.name == null)
                        Console.WriteLine("Error parcing " + args[3]);
                    break;
                case "-s":
                    end = fr.searchbyname(args[3]);
                    if (end.name == null)
                        Console.WriteLine("Error parcing " + args[3]);
                    break;
                default:
                    Console.WriteLine("Error parcing " + args[2]);
                    return false;
            }
            if (args.Length > 4)
            {
                switch (args[4].ToLower())
                {
                    case "-min":
                        try
                        {
                            min = Int32.Parse(args[5]);
                        }
                        catch { }
                        break;
                    case "-dev":
                        try
                        {
                            dev = Int32.Parse(args[5]);
                        }
                        catch { }
                        break;
                    default:
                        return true;
                }
            }
            if(args.Length > 6)
            {
                switch (args[6].ToLower())
                {
                    case "-min":
                        try
                        {
                            min = Int32.Parse(args[7]);
                        }
                        catch { }
                        break;
                    case "-dev":
                        try
                        {
                            dev = Int32.Parse(args[7]);
                        }
                        catch { }
                        break;
                    default:
                        return true;
                }
                }
            return true;
        }
        static void gethelp()
        {
            Console.WriteLine("nextinroute V" + ver_major + "." + ver_minor + " created:" + create_date.ToShortDateString());
            Console.WriteLine("Used to plot route between two points using systemsWithoutCoordinates.sqlite. That can be located here:");
            Console.WriteLine("https://www.dropbox.com/s/zs3k89e4sl07gzc/systemsWithoutCoordinates.sqlite?dl=0");
            Console.WriteLine("Usage:");
            Console.WriteLine("    -u username : Search by user location (Requires EDSM Public profile)");
            Console.WriteLine("    -s starname : search by starname");
            Console.WriteLine("nextinroute.exe -u slowice -s colonia");
            Console.WriteLine("Please create an issue on github with any issues you might have:");
            Console.WriteLine("https://github.com/RapidfireCRH/nextinroute/issues");
        }
    }
}


