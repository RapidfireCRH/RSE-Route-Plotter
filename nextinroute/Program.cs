using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace getmeoutofhere
{
    class Program
    {
        //starting variables
        static findroute fr = new findroute();
        static findroute.star_st start = new findroute.star_st();
        static findroute.star_st destination = new findroute.star_st();
        static int numofjumps = 0;//Zen only
        static int maxdist = 0;//route only
        static int maxdev = 20;//route only

        //program toggle
        enum state {route = 0, zen = 1 }
        static state prog = state.route;

        //version
        static DateTime create_date = new DateTime(2018, 1, 2);
        static int ver_major = 1;
        static int ver_minor = 2;
        static void Main(string[] args)
        {
            if (!scanargs(args))
                return;

            
            List<findroute.star_st> list = new List<findroute.star_st>();
            list.Add(start);
            Console.Clear();
            Console.WriteLine(start.name);
            Console.WriteLine("X: " + start.coord.x);
            Console.WriteLine("Y: " + start.coord.y);
            Console.WriteLine("Z: " + start.coord.z);
            double totaldist = 0;
            switch (prog)
            {
                case state.route:
                    findroute.star_st ret = start;
                    while ((ret = fr.findnext(ret, destination, maxdev, maxdist)).name != destination.name)
                    {
                        Console.Clear();
                        Console.WriteLine((list.Count + 1) + "." + ret.name);
                        Console.WriteLine("X: " + ret.coord.x);
                        Console.WriteLine("Y: " + ret.coord.y);
                        Console.WriteLine("Z: " + ret.coord.z);
                        Console.WriteLine("Distance from previous: " + list[list.Count - 1].distance(ret) + "ly");
                        totaldist += list[list.Count - 1].distance(ret);
                        list.Add(ret);
                    }
                    totaldist += list[list.Count - 1].distance(destination);
                    list.Add(destination);
                    break;
                case state.zen:
                    findroute.star_st zenret = start;
                    while (list.Count!= numofjumps+1)
                    {
                        zenret = fr.zen(zenret, maxdist == 0 ? 18000 : maxdist);
                        Console.Clear();
                        Console.WriteLine((list.Count + 1) + "." + zenret.name);
                        Console.WriteLine("X: " + zenret.coord.x);
                        Console.WriteLine("Y: " + zenret.coord.y);
                        Console.WriteLine("Z: " + zenret.coord.z);
                        Console.WriteLine("Distance from previous: " + list[list.Count - 1].distance(zenret) + "ly");
                        totaldist += list[list.Count - 1].distance(zenret);
                        list.Add(zenret);
                    }
                    break;
            }
            StringBuilder bldr = new StringBuilder();
            
            if (prog == state.zen)
            {
                bldr.AppendLine("total distance: " + totaldist + " | RSE stars eliminated: " + (list.Count - 1));
                foreach (findroute.star_st x in list)
                {
                    bldr.AppendLine(x.name + ", " + x.coord.x + ", " + x.coord.y + ", " + x.coord.z);
                }
                File.WriteAllText("zen-" + start.name + "route.txt", bldr.ToString());
                Console.WriteLine();
                Console.WriteLine("This is also available here: zen-" + start.name + "route.txt", bldr.ToString());
            }
            if (prog == state.route)
            {
                bldr.AppendLine("As the Crow Flys: " + start.distance(destination) + " and total: " + totaldist + " | RSE stars eliminated: " + (list.Count - 2));
                foreach (findroute.star_st x in list)
                {
                    bldr.AppendLine(x.name + ", " + x.coord.x + ", " + x.coord.y + ", " + x.coord.z);
                }
                File.WriteAllText(start.name + "-" + destination.name + "route.txt", bldr.ToString());
                Console.WriteLine();
                Console.WriteLine("This is also available here: " + start.name + "-" + destination.name + "route.txt");
            }
        }
        static bool scanargs(string[] args)
        {
            if (args.Length < 2)
            {
                gethelp();
                return false;
            }
            for (int i = 0; i != args.Length;)
            {
                try
                {
                    switch (args[i].ToLower())
                    {
                        case "-s":
                            if (start.name != null)
                                destination = fr.searchbyname(args[i + 1].ToLower());
                            else
                                start = fr.searchbyname(args[i + 1].ToLower());
                            i = i + 2;
                            break;
                        case "-u":
                            if (start.name != null)
                                destination = fr.currentlocation(args[i + 1].ToLower());
                            else
                                start = fr.currentlocation(args[i + 1].ToLower());
                            i = i + 2;
                            break;
                        case "-j":
                            numofjumps = Int32.Parse(args[++i].ToLower());
                            i++;
                            break;
                        case "-dev":
                            maxdev = Int32.Parse(args[++i].ToLower());
                            i++;
                            break;
                        case "-dist":
                            maxdist = Int32.Parse(args[++i].ToLower());
                            i++;
                            break;
                        case "-help":
                            gethelp();
                            return false;
                        case "-zen":
                            prog = state.zen;
                            i++;
                            break;
                        case "-route":
                            prog = state.route;
                            i++;
                            break;
                        default:
                            i++;
                            break;
                    }
                }
                catch { i++; }
            }
            if (prog == state.zen)
                if (start.name != null)
                    return true;
            if (prog == state.route)
                if (destination.name != null)
                    return true;
            Console.WriteLine("Invalid or missing args.");
            gethelp();
            return false;
        }
        static void gethelp()
        {
            Console.WriteLine("nextinroute V" + ver_major + "." + ver_minor + " created:" + create_date.ToShortDateString());
            Console.WriteLine("Used to plot route between two points using systemsWithoutCoordinates.sqlite. That can be located here:");
            Console.WriteLine("https://www.dropbox.com/s/zs3k89e4sl07gzc/systemsWithoutCoordinates.sqlite?dl=0");
            Console.WriteLine("Usage:");
            Console.WriteLine("    -zen : Zen mode, plot random 20k plots");
            Console.WriteLine("    -route : Route mode, plot between points");
            Console.WriteLine("nextinroute.exe -route -u slowice -s colonia");
            Console.WriteLine("For full usage information, or to report an issue, please locate us on github:");
            Console.WriteLine("https://github.com/RapidfireCRH/nextinroute");
        }
    }
}
