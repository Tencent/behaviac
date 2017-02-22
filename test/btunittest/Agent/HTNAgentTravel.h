/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Tencent is pleased to support the open source community by making behaviac available.
//
// Copyright (C) 2015-2017 THL A29 Limited, a Tencent company. All rights reserved.
//
// Licensed under the BSD 3-Clause License (the "License"); you may not use this file except in compliance with
// the License. You may obtain a copy of the License at http://opensource.org/licenses/BSD-3-Clause
//
// Unless required by applicable law or agreed to in writing, software distributed under the License is
// distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////

#ifndef BTUNITEST_HTNAGENTTRAVEL_H_
#define BTUNITEST_HTNAGENTTRAVEL_H_

#include "behaviac/common/base.h"
#include "behaviac/agent/agent.h"
#include "behaviac/common/member.h"
#include "behaviac/agent/registermacros.h"

class HTNAgentTravel : public behaviac::Agent
{
public:
    HTNAgentTravel();
    virtual ~HTNAgentTravel();
    HTNAgentTravel& operator=(const HTNAgentTravel &){ return *this; }

	BEHAVIAC_DECLARE_AGENTTYPE(HTNAgentTravel, behaviac::Agent);

    class Location
    {
    public:
        int id;
        int city;

		Location() : id(0), city(0)
        {}

        Location(int id, int city)
        {
            this->id = id;
            this->city = city;
        }
    };

    const int city_sh;
    const int city_sz;

    const int airport_sh_hongqiao;
    const int airport_sh_pudong;

    const int airport_sz_baoan;

    const int sh_td;
    const int sz_td;
    const int sh_home;
    const int sz_hotel;

    behaviac::map<int, Location> _locations;
    int _start;
    int _finish;

    class Journey
    {
    public:
        behaviac::string name;
        int x;
        int y;
    };

    behaviac::vector<Journey> _path;

    behaviac::vector<Journey>& Path()
    {
        return this->_path;
    }

    void SetStartFinish(int s, int f)
    {
        if (s != f)
        {
            if (this->_locations.find(s) != this->_locations.end())
            {
                this->_start = s;
            }

            if (this->_locations.find(f) != this->_locations.end())
            {
                this->_finish = f;
            }
        }
    }

    void resetProperties()
    {
        _locations.insert(std::pair<int, Location>(airport_sh_hongqiao, Location(airport_sh_hongqiao, city_sh)));
        _locations.insert(std::pair<int, Location>(airport_sh_pudong, Location(airport_sh_pudong, city_sh)));

        _locations.insert(std::pair<int, Location>(airport_sz_baoan, Location(airport_sz_baoan, city_sz)));

        _locations.insert(std::pair<int, Location>(sh_td, Location(sh_td, city_sh)));
        _locations.insert(std::pair<int, Location>(sh_home, Location(sh_home, city_sh)));
        _locations.insert(std::pair<int, Location>(sz_td, Location(sz_td, city_sz)));
        _locations.insert(std::pair<int, Location>(sz_hotel, Location(sz_hotel, city_sz)));

        _path.clear();
    }

    void init()
    {
        //base.Init();
        resetProperties();
    }

    void finl()
    {
    }

    //[behaviac::MemberMetaInfo("MemberProperty", "MemberProperty")]
    //UInt32 MemberProperty = 0;

    void ride_taxi(int x, int y)
    {
        Journey j;
        j.name = "ride_taxi";
        j.x = x;
        j.y = y;
        _path.push_back(j);
    }

    void fly(int x, int y)
    {
        Journey j;
        j.name = "fly";
        j.x = x;
        j.y = y;
        _path.push_back(j);
    }

    bool exist_start(int& s)
    {
        s = this->_start;
        return true;
    }

    bool exist_finish(int& f)
    {
        f = this->_finish;
        return true;
    }

    bool short_distance(int x, int y)
    {
        Location lx = _locations[x];
        Location ly = _locations[y];

        if (lx.city == ly.city)
        {
            return true;
        }

        return false;
    }

    bool long_distance(int x, int y)
    {
        Location lx = _locations[x];
        Location ly = _locations[y];

        if (lx.city != ly.city)
        {
            return true;
        }

        return false;
    }

    bool exist_airport(int x, int& ax)
    {
        if (x == sh_td)
        {
            ax = airport_sh_hongqiao;
            //ax = airport_sh_pudong;
            return true;

        }
        else if (x == sz_td)
        {
            ax = airport_sz_baoan;
            return true;
        }

        return false;
    }

    bool exist_airports(int x, behaviac::vector<int>& axs)
    {
        //BEHAVIAC_ASSERT(axs.size() == 0);
        //axs = behaviac::vector<int>();
        axs.clear();

        if (x == sh_td)
        {
            axs.push_back(airport_sh_hongqiao);
            axs.push_back(airport_sh_pudong);
            return true;

        }
        else if (x == sz_td)
        {
            axs.push_back(airport_sz_baoan);
            return true;
        }

        return false;
    }
};

#endif//BTUNITEST_HTNAGENTTRAVEL_H_
