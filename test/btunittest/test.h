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

#ifndef TEST_H
#define TEST_H

#include "behaviac/common/base.h"
#include "behaviac/common/container/string.h"

#include <iostream>
#include <sstream>
#include <vector>
#include <functional>


//#define ENABLE_MEMORY_LEAKTEST	1

#if ENABLE_MEMORY_LEAKTEST 
#define ENABLE_MEMORYDUMP	1
#endif


namespace test
{
    typedef void(*Test)();

    class TestFailedException
    {
    public:
        TestFailedException(const std::string& message)
            : m_message(message)
        {
        }

        std::string m_message;
    };

    class TestSuite
    {
    private:
        typedef std::pair<std::string, Test> RegisteredTest;
        typedef std::vector<RegisteredTest> RegisteredTests;

    public:
        TestSuite()
            : m_verbose(false)
        {
        }

        void registerTest(const std::string& testName, Test test)
        {
            m_tests.push_back(RegisteredTest(testName, test));
        }

        void setVerbose(bool b)
        {
            m_verbose = b;
        }

        bool runAllTests()
        {
            bool allPassed = true;

            for (RegisteredTests::iterator test = m_tests.begin(); test != m_tests.end(); ++test)
            {
                if (m_verbose)
                {
                    std::cout << test->first << " " << std::flush;
                }

#if BEHAVIAC_CCDEFINE_MSVC

                try
                {
#endif//#if BEHAVIAC_CCDEFINE_MSVC
                    test->second();

                    if (m_verbose)
                    {
                        std::cout << "PASS" << std::endl << std::flush;

                    }
                    else
                    {
                        std::cout << "." << std::flush;
                    }

#if BEHAVIAC_CCDEFINE_MSVC

                }
                catch (TestFailedException e)
                {
                    allPassed = false;

                    if (!m_verbose)
                    {
                        std::cout << test->first << " ";
                    }

                    std::cout << "FAIL: " << e.m_message << std::endl << std::flush;
                }

#endif//#if BEHAVIAC_CCDEFINE_MSVC
            }

            if (!m_verbose)
            {
                std::cout << std::endl << std::flush;
            }

            return allPassed;
        }

        static TestSuite& getInstance()
        {
            static TestSuite instance;
            return instance;
        }

    private:
        bool m_verbose;
        RegisteredTests m_tests;
    };

    class AutoTestRegister
    {
    public:
        AutoTestRegister(const std::string& name, Test test)
        {
            TestSuite::getInstance().registerTest(name, test);
        }
    };
}

#define TEST(SUITENAME, TESTNAME)                                           \
    void SUITENAME##_##TESTNAME();                                          \
    static test::AutoTestRegister autoTestRegister_##SUITENAME##_##TESTNAME(#SUITENAME "_" #TESTNAME, SUITENAME##_##TESTNAME); \
    void SUITENAME##_##TESTNAME()

#define SUITE(Name)                                                         \
    namespace Suite##Name {                                                 \
        namespace UnitTestSuite {                                           \
            inline char const* GetSuiteName () {                            \
                return #Name ;                                              \
            }                                                               \
        }                                                                   \
    }                                                                       \
    namespace Suite##Name

#define TEST_FIXTURE(SUITENAME, TESTNAME) \
    class SUITENAME##_##TESTNAME##Helper : public SUITENAME \
    { \
    public: \
        void test(); \
    }; \
    void SUITENAME##_##TESTNAME()                         \
    {\
        SUITENAME##_##TESTNAME##Helper testinstance; \
        testinstance.test(); \
    }\
    static test::AutoTestRegister autoTestRegister_##SUITENAME##_##TESTNAME(#SUITENAME "_" #TESTNAME, SUITENAME##_##TESTNAME); \
    void SUITENAME##_##TESTNAME##Helper::test()

#define TEST_COLLECTION( CollectionType, Type, TESTNAME )												\
    void _##TESTNAME();																					\
    static test::AutoTestRegister autoTestRegister_##TESTNAME(#TESTNAME, _##TESTNAME);					\
    struct TestFunctor_##TESTNAME																		\
    {                                                                                                   \
        TestFunctor_##TESTNAME()																		\
        {}                                                                                              \
        \
        template< typename Type >                                                                       \
        void operator()( Type );                                                                        \
    private:                                                                                            \
        \
        TestFunctor_##TESTNAME & operator=( const TestFunctor_##TESTNAME & );							\
    };                                                                                                  \
    void _##TESTNAME()																					\
    {																									\
        TestFunctor_##TESTNAME functor;																	\
        behaviac::Meta::ForEach< CollectionType >( functor );												\
    }																									\
    template< typename Type >                                                                           \
    void TestFunctor_##TESTNAME::operator()( Type )

#if BEHAVIAC_CCDEFINE_MSVC
#define CHECK(X)                                              \
    if (!(X))                                                 \
    {                                                         \
        std::ostringstream os;                                \
        os << #X << " ";                                      \
        os << __FILE__ << ":" << __LINE__;                    \
        throw test::TestFailedException(os.str());            \
    }

#define CHECK_EQUAL(E, A)                                     \
    if (!((E) == (A)))                                        \
    {                                                         \
        std::ostringstream os;                                \
        os << "(" << #E << " == " << #A << ") ";              \
        os << "E:" << (E) << " actual:" << (A) << " "; \
        os << __FILE__ << ":" << __LINE__;                    \
        throw test::TestFailedException(os.str());            \
    }

#define CHECK_FLOAT_EQUAL(E, A)                               \
    if (!behaviac::IsEqualWithEpsilon((E), (A)))			  \
    {                                                         \
        std::ostringstream os;                                \
        os << "(" << #E << " == " << #A << ") ";              \
        os << "E:" << (E) << " actual:" << (A) << " ";		  \
        os << __FILE__ << ":" << __LINE__;                    \
        throw test::TestFailedException(os.str());            \
    }



#else
#define CHECK(X)                                              \
    if (!(X))                                                 \
    {                                                         \
        std::ostringstream os;                                \
        os << #X << " ";                                      \
        os << __FILE__ << ":" << __LINE__;                    \
        BEHAVIAC_ASSERT(false);                               \
    }

#define CHECK_EQUAL(E, A)                                     \
    if (!((E) == (A)))                                        \
    {                                                         \
        std::ostringstream os;                                \
        os << "(" << #E << " == " << #A << ") ";              \
        os << __FILE__ << ":" << __LINE__;                    \
        BEHAVIAC_ASSERT(false);                               \
    }

#define CHECK_FLOAT_EQUAL(E, A)                               \
    if (!behaviac::IsEqualWithEpsilon((E), (A)))			  \
    {                                                     \
        std::ostringstream os;                                \
        os << "(" << #E << " == " << #A << ") ";              \
        os << "E:" << (E) << " actual:" << (A) << " "; \
        os << __FILE__ << ":" << __LINE__;                    \
        BEHAVIAC_ASSERT(false);                               \
    }

#endif//#if BEHAVIAC_CCDEFINE_MSVC

#endif // TEST_H
