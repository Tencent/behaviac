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

#ifndef _BEHAVIAC_GENERATOR_H_
#define _BEHAVIAC_GENERATOR_H_

/*
void generator_test()
{
$generator(descent)
{
// place for all variables used in the generator
int i; // our counter

// place the constructor of our generator, e.g.
// descent(int minv, int maxv) {...}

// from $emit to $stop is a body of our generator:

$emit(int) // will emit int values. Start of body of the generator.
for (i = 10; i > 0; --i)
{
CHECK_EQUAL(1, 1);
$yield(i); // a.k.a. yield in Python,
}
// returns next number in [1..10], reversed.
$stop; // stop, end of sequence. End of body of the generator.
};

descent gen;
for(int n; gen(n);) // "get next" generator invocation
{
}
}
*/

struct _generator {
    int _line;
    _generator() : _line(0) {}
};

#define $generator(NAME) struct NAME : public _generator

#define $emit(T) bool operator()(T& _rv) { \
    switch(_line) { case 0:;

#define $stop  } _line = 0; return false; }

#define $yield(V)     \
    do {\
        _line=__LINE__;\
    _rv = (V); return true; case __LINE__:;\
    } while (0)

#endif//_BEHAVIAC_GENERATOR_H_
