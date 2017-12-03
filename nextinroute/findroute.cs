using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
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
        SQLiteConnection m_dbConnection;
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
        public star_st findnext(star_st curr, star_st dest)
        {
            fstrun();
            double dist = Math.Sqrt(Math.Pow(curr.coord.x - dest.coord.x, 2) + Math.Pow(curr.coord.y - dest.coord.y, 2) + Math.Pow(curr.coord.z - dest.coord.z, 2));
            double dev = dist / 3;
            coord_block_st query = new coord_block_st();
            query.x_start = curr.coord.x > dest.coord.x ? dest.coord.x - dev : curr.coord.x - dev;
            query.x_end = curr.coord.x < dest.coord.x ? dest.coord.x + dev : curr.coord.x + dev;
            query.y_start = curr.coord.y > dest.coord.y ? dest.coord.y - dev : curr.coord.y - dev;
            query.y_end = curr.coord.y < dest.coord.y ? dest.coord.y + dev : curr.coord.y + dev;
            query.z_start = curr.coord.z > dest.coord.z ? dest.coord.z - dev : curr.coord.z - dev;
            query.z_end = curr.coord.z < dest.coord.z ? dest.coord.z + dev : curr.coord.z + dev;
            m_dbConnection.Open();
            string sql = "SELECT name, x, y, z FROM systems where x BETWEEN " + query.x_start + " and " + query.x_end + " and y BETWEEN " + query.y_start + " and " + query.y_end + " and z BETWEEN " + query.z_start + " and " + query.z_end;
            SQLiteCommand create = new SQLiteCommand(sql, m_dbConnection);
            SQLiteDataReader read = create.ExecuteReader();
            List<check_st> collect = new List<check_st>();
            while (read.Read())
            {
                check_st ret = new check_st();
                ret.star.name = read["name"].ToString();
                ret.star.coord.x = Double.Parse(read["x"].ToString());
                ret.star.coord.y = Double.Parse(read["y"].ToString());
                ret.star.coord.z = Double.Parse(read["z"].ToString());
                ret.dist = Math.Sqrt(Math.Pow(curr.coord.x - ret.star.coord.x, 2) + Math.Pow(curr.coord.y - ret.star.coord.y, 2) + Math.Pow(curr.coord.z - ret.star.coord.z, 2));

                dist_block_st distblk = new dist_block_st();
                distblk.c = dist;
                distblk.b = ret.dist;
                distblk.a = Math.Sqrt(Math.Pow(ret.star.coord.x - dest.coord.x, 2) + Math.Pow(ret.star.coord.y - dest.coord.y, 2) + Math.Pow(ret.star.coord.z - dest.coord.z, 2));

                ret.angle = Math.Acos((Math.Pow(distblk.a, 2) - Math.Pow(distblk.b, 2) - Math.Pow(distblk.c, 2)) / ((-2) * distblk.b * distblk.c)) * (180 / Math.PI);
                collect.Add(ret);
            }
            m_dbConnection.Close();
            check_st temp = new check_st();
            temp.star = dest;
            temp.dist = dist;
            temp.angle = 0;
            collect.Add(temp);
            collect.Sort();
            StringBuilder bldr = new StringBuilder();
            foreach (check_st x in collect)
            {
                bldr.AppendLine(x.star.name + ", " + x.star.coord.x + ", " + x.star.coord.y + ", " + x.star.coord.z + ", " + x.dist + ", " + x.angle);
            }
            File.WriteAllText("temp.txt", bldr.ToString());
            for (int x = 0; x != collect.Count; x++)
                if (collect[x].angle <20)
                    return collect[x].star;
            return temp.star;//This will never be reached, but just in case
        }
        public void fstrun()
        {
            if (firstrun)
                return;
            if (!File.Exists("systemsWithoutCoordinates.sqlite"))
                throw new FileNotFoundException("DB not in local directory. Please download new database/ move database to current directory. Current Directory: " + Directory.GetCurrentDirectory());
            m_dbConnection = new SQLiteConnection("Data Source=systemsWithoutCoordinates.sqlite; Version=3;");
            firstrun = true;
        }
    }
}
