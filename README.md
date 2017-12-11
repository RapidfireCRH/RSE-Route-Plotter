Nextinroute

Nextinroute is a app plugin written in c# it comes prepackaged with a command line front end

Here are the 3 methods contained in the plugin. They all return a predefined structure, star_st:

public struct star_st{

    public string name;
    
    public coord_st coord;
    
}

public struct coord_st{

    public double x;
    
    public double y;
    
    public double z;
    
}

Here are the 3 methods:

1. star_st findnext(star_st curr, star_st dest, int variation=20)

    finds the next star between curr and dest with less then variation angle.
    
2. star_st searchbyname(string starname)

    finds a star through EDSM and returns its star_st, or a blank star_st if it cannot be found
    
3. star_st currentlocation(string username)

    finds a user in EDSM and extracts their current location. This requires that the profile be public
    
    
The front end uses the following format:

nextinroute.exe [-u username|-s starname] [-u username|-s starname]
