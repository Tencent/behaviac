using System;
using System.Collections;
using System.Collections.Generic;

[behaviac.TypeMetaInfo()]
public class HTNAgentTravel : behaviac.Agent
{
    public const int airport_sh_hongqiao = 0;

    public const int airport_sh_pudong = 1;

    public const int airport_sz_baoan = 2;

    public const int sh_home = 5;

    public const int sh_td = 3;

    public const int sz_hotel = 6;

    public const int sz_td = 4;

    private const int city_sh = 1;

    private const int city_sz = 2;

    private int _finish = sz_td;

    private Dictionary<int, Location> _locations = new Dictionary<int, Location>();

    private List<Journey> _path = new List<Journey>();

    private int _start = sh_td;

    public List<Journey> Path {
        get
        {
            return this._path;
        }
    }

    [behaviac.MethodMetaInfo()]
    public bool exist_airport(int x, ref int ax) {
        if (x == sh_td) {
            ax = airport_sh_hongqiao;
            //ax = airport_sh_pudong;
            return true;

        } else if (x == sz_td) {
            ax = airport_sz_baoan;
            return true;
        }

        return false;
    }

    [behaviac.MethodMetaInfo()]
    public bool exist_airports(int x, ref List<int> axs) {
        //axs = new List<int>();
        behaviac.Debug.Check(axs != null);
        axs.Clear();

        if (x == sh_td) {
            axs.Add(airport_sh_hongqiao);
            axs.Add(airport_sh_pudong);
            return true;

        } else if (x == sz_td) {
            axs.Add(airport_sz_baoan);
            return true;
        }

        return false;
    }

    [behaviac.MethodMetaInfo()]
    public bool exist_finish(ref int f) {
        f = this._finish;
        return true;
    }

    [behaviac.MethodMetaInfo()]
    public bool exist_start(ref int s) {
        s = this._start;
        return true;
    }

    public void finl() {
    }

    [behaviac.MethodMetaInfo()]
    public void fly(int x, int y) {
        Journey j = new Journey();
        j.name = "fly";
        j.x = x;
        j.y = y;
        _path.Add(j);
    }

    public void init() {
        base.Init();

        resetProperties();
    }

    [behaviac.MethodMetaInfo()]
    public bool long_distance(int x, int y) {
        Location lx = _locations[x];
        Location ly = _locations[y];

        if (lx.city != ly.city) {
            return true;
        }

        return false;
    }

    public void resetProperties() {
        _locations[airport_sh_hongqiao] = new Location(airport_sh_hongqiao, city_sh);
        _locations[airport_sh_pudong] = new Location(airport_sh_pudong, city_sh);

        _locations[airport_sz_baoan] = new Location(airport_sz_baoan, city_sz);

        _locations[sh_td] = new Location(sh_td, city_sh);
        _locations[sh_home] = new Location(sh_home, city_sh);
        _locations[sz_td] = new Location(sz_td, city_sz);
        _locations[sz_hotel] = new Location(sz_hotel, city_sz);

        _path.Clear();
    }

    [behaviac.MethodMetaInfo()]
    public void ride_taxi(int x, int y) {
        Journey j = new Journey();
        j.name = "ride_taxi";
        j.x = x;
        j.y = y;
        _path.Add(j);
    }

    public void SetStartFinish(int s, int f) {
        if (s != f) {
            if (this._locations.ContainsKey(s)) {
                this._start = s;
            }

            if (this._locations.ContainsKey(f)) {
                this._finish = f;
            }
        }
    }

    //[behaviac.MemberMetaInfo("MemberProperty", "MemberProperty")]
    //public uint MemberProperty = 0;
    [behaviac.MethodMetaInfo()]
    public bool short_distance(int x, int y) {
        Location lx = _locations[x];
        Location ly = _locations[y];

        if (lx.city == ly.city) {
            return true;
        }

        return false;
    }

    public class Journey
    {
        public string name;
        public int x;
        public int y;
    }

    private class Location
    {
        public int city;
        public int id;

        public Location(int id, int city) {
            this.id = id;
            this.city = city;
        }
    }
}
