using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Threading;
using Npgsql;

namespace getmeoutofhere
{
    class findroute
    {
        int version_major = 2;
        int version_minor = 0;
        bool beta = false;
        string version_date = "21-JUL-2020";

        NpgsqlConnection conn = new NpgsqlConnection();
        List<star_st> database = new List<star_st>();
        bool firstrun = false;
        public struct coord_st
        {
            public double x;
            public double y;
            public double z;
        }
        public struct star_st
        {
            public string name;
            public coord_st coord;
            public double distance(star_st other)
            {
                return Math.Sqrt(Math.Pow(this.coord.x - other.coord.x, 2) + Math.Pow(this.coord.y - other.coord.y, 2) + Math.Pow(this.coord.z - other.coord.z, 2));
            }
            public double angle(star_st other, star_st reference)
            {
                dist_block_st distblk = new dist_block_st();
                distblk.c = this.distance(reference);
                distblk.b = this.distance(other);
                distblk.a = other.distance(reference);
                return Math.Acos((Math.Pow(distblk.a, 2) - Math.Pow(distblk.b, 2) - Math.Pow(distblk.c, 2)) / ((-2) * distblk.b * distblk.c)) * (180 / Math.PI);//(a^2-b^2-c^2)/2bc
            }
        }
        private struct check_st : IComparable<check_st>
        {
            public star_st star;
            public double dist;
            public double angle;
            public int CompareTo(check_st other)
            {
                return this.dist.CompareTo(other.dist);
            }
        }
        private struct coord_block_st
        {
            public double x_start;
            public double x_end;
            public double y_start;
            public double y_end;
            public double z_start;
            public double z_end;
        }
        private struct dist_block_st
        {
            public double a; //DB star to dest
            public double b; //curr to db star
            public double c; //curr to dest
        }
        private struct div_st
        {
            public double x;
            public double y;
            public double z;
        }
        Random rnd = new Random();
        public star_st zen(star_st start, int distance = 18000)
        {
            fstrun();
            loaddb();

            star_st saga = new star_st();
            saga.name = "Sagittarius A*";
            saga.coord.x = 25.21875;
            saga.coord.y = -20.90625;
            saga.coord.z = 25899.96875;
            List<star_st> list = new List<star_st>();
            foreach (star_st x in database)
                if (x.distance(start) > distance && x.distance(start) < 20000)
                    list.Add(x);
            if (list.Count > 0)
            {
                int rand = rnd.Next(0, list.Count);
                star_st ret = list[rand];
                list.Remove(ret);
                return ret;
            }
            return saga;
        }
        public star_st findnext(star_st curr, star_st dest, int variation = 20, int min_dist = 0)
        {
            fstrun();
            div_st div = new div_st();
            div_st dir = new div_st();
            div.x = dest.coord.x - curr.coord.x;
            div.y = dest.coord.y - curr.coord.y;
            div.z = dest.coord.z - curr.coord.z;
            dir.x = div.x >= 0 ? 1 : -1;
            dir.y = div.y >= 0 ? 1 : -1;
            dir.z = div.z >= 0 ? 1 : -1;
            double dist = curr.distance(dest);
            double jumps = dist / 400;
            if (dist < min_dist)
                return dest;
            coord_block_st query = new coord_block_st();
            query.x_start = curr.coord.x + (div.x / jumps) + (dir.x * min_dist / 3) - 200;
            query.x_end = curr.coord.x + (div.x / jumps) + (dir.x * min_dist / 3) + 200;
            query.y_start = curr.coord.y + (div.y / jumps) - 200;
            query.y_end = curr.coord.y + (div.y / jumps) + 200;
            query.z_start = curr.coord.z + (div.z / jumps) + (dir.z * min_dist / 3) - 200;
            query.z_end = curr.coord.z + (div.z / jumps) + (dir.z * min_dist / 3) + 200;
            conn.Open();

            string sql = "SELECT name, x, y, z FROM systems where x BETWEEN " + query.x_start.ToString(CultureInfo.InvariantCulture) + " and " + query.x_end.ToString(CultureInfo.InvariantCulture)
                + " and y BETWEEN " + query.y_start.ToString(CultureInfo.InvariantCulture) + " and " + query.y_end.ToString(CultureInfo.InvariantCulture)
                + " and z BETWEEN " + query.z_start.ToString(CultureInfo.InvariantCulture) + " and " + query.z_end.ToString(CultureInfo.InvariantCulture)
                + " and action_todo = 1 and deleted_at is NULL;";

            NpgsqlCommand command = new NpgsqlCommand(sql, conn);
            NpgsqlDataReader read = command.ExecuteReader();
            List<check_st> collect = new List<check_st>();
            while (read.Read())
            {
                check_st ret = new check_st();
                ret.star.name = read["name"].ToString();
                ret.star.coord.x = Double.Parse(read["x"].ToString(), CultureInfo.InvariantCulture);
                ret.star.coord.y = Double.Parse(read["y"].ToString(), CultureInfo.InvariantCulture);
                ret.star.coord.z = Double.Parse(read["z"].ToString(), CultureInfo.InvariantCulture);
                ret.dist = ret.star.distance(curr);
                ret.angle = curr.angle(ret.star, dest);
                collect.Add(ret);
            }
            conn.Close();
            check_st temp = new check_st();
            temp.star = dest;
            temp.dist = dist;
            temp.angle = 0;
            collect.Add(temp);
            collect.Sort();
            for (int x = 0; x != collect.Count; x++)
                if (collect[x].angle < variation)// || (collect[x].dist == 0 && collect[x].star.name != curr.name))
                    if (collect[x].dist > min_dist)
                        //if (checkstar(collect[x].star))
                        return collect[x].star;
            return dest;//This will never be reached, but just in case
        }


        private DateTime lastchecked = DateTime.Now.AddHours(-1);
        private int numofchecks = 0;
        private bool checkstar(star_st check)
        {
            if (lastchecked < DateTime.Now.AddSeconds(1))
            {
                lastchecked = DateTime.Now;
                numofchecks = 0;
            }
            numofchecks++;
            if (numofchecks < 6)
            {
                string result = new System.Net.WebClient().DownloadString("https://www.edsm.net/api-v1/systems?systemName=" + check.name + "&showCoordinates=1");
                if (!(result == "[]" || !result.Contains("coords")))
                    return false;
            }
            return false;
        }
        private void loaddb()
        {
            if (!firstrun)
                fstrun();
            if (database.Count != 0)
                return;
            conn.Open();
            string sql = "SELECT name, x, y, z FROM systems where action_todo = 1 and deleted_at is NULL;";
            NpgsqlTransaction tran = conn.BeginTransaction();
            NpgsqlCommand command = new NpgsqlCommand(sql, conn);
            NpgsqlDataReader read = command.ExecuteReader();
            while (read.Read())
            {
                star_st ret = new star_st();
                ret.name = read["name"].ToString();
                ret.coord.x = Double.Parse(read["x"].ToString(), CultureInfo.InvariantCulture);
                ret.coord.y = Double.Parse(read["y"].ToString(), CultureInfo.InvariantCulture);
                ret.coord.z = Double.Parse(read["z"].ToString(), CultureInfo.InvariantCulture);
                database.Add(ret);
            }
            conn.Close();
        }
        public star_st searchbyname(string starname)
        {
            string result = new System.Net.WebClient().DownloadString("https://www.edsm.net/api-v1/systems?systemName=" + starname + "&showCoordinates=1");
            if (result == "[]")
                return new star_st();
            if (!result.Contains("coords"))
                return new star_st();
            star_st ret = new star_st();
            ret.name = starname;
            ret.coord.x = Double.Parse((result.Substring(result.IndexOf("\"x\":") + "\"x\":".Length, result.IndexOf(",\"y\":") - (result.IndexOf("\"x\":") + "\"x\":".Length))), CultureInfo.InvariantCulture);
            ret.coord.y = Double.Parse((result.Substring(result.IndexOf(",\"y\":") + ",\"y\":".Length, result.IndexOf(",\"z\":") - (result.IndexOf(",\"y\":") + ",\"y\":".Length))), CultureInfo.InvariantCulture);
            ret.coord.z = Double.Parse((result.Substring(result.IndexOf(",\"z\":") + ",\"z\":".Length, result.IndexOf("}") - (result.IndexOf(",\"z\":") + ",\"z\":".Length))), CultureInfo.InvariantCulture);
            numofchecks++;
            return ret;
        }
        public star_st currentlocation(string username)
        {
            string result = new System.Net.WebClient().DownloadString("https://www.edsm.net/api-logs-v1/get-position?commanderName=" + username);
            if (result.Length < 13 || result.Substring(0, 13) != "{\"msgnum\":100")
                return new star_st();
            numofchecks++;
            return searchbyname(result.Substring(result.IndexOf(",\"system\":\"") + ",\"system\":\"".Length, result.IndexOf("\",\"firstDiscover") - (result.IndexOf(",\"system\":\"") + ",\"system\":\"".Length)));
        }
        private void fstrun()
        {
            if (firstrun)
                return;
            conn = new NpgsqlConnection("SERVER=cyberlord.de; Port=5432; Database=edmc_rse_db; User ID=edmc_rse_user; Password=asdfplkjiouw3875948zksmdxnf;Timeout=12;Application Name=nextinroutev" + version_major + "." + version_minor + (beta == true ? "b|" : "|") + version_date + ";Keepalive=60;");
            versioncheck();
            firstrun = true;
        }
        public void versioncheck()
        {
            string temp = "";
            if (!beta)
            {
                temp = new System.Net.WebClient().DownloadString("https://raw.githubusercontent.com/RapidfireCRH/nextinroute/master/version");
                if (temp.Substring(0, temp.Length - 1) != (version_major + "." + version_minor + " " + version_date))
                {
                    Console.Write("Newer version available. Current Version:" + version_major + "." + version_minor + " " + version_date + ". Latest version: " + temp);
                    Thread.Sleep(6000);
                }
            }
            else
            {
                temp = new System.Net.WebClient().DownloadString("https://raw.githubusercontent.com/RapidfireCRH/nextinroute/Dev/version");
                if (temp.Substring(0, temp.Length - 1) != (version_major + "." + version_minor + "b " + version_date))
                {
                    Console.Write("Newer version available. Current Version:" + version_major + "." + version_minor + "b " + version_date + ". Latest version: " + temp);
                    Thread.Sleep(6000);
                }
            }
        }

    }
}