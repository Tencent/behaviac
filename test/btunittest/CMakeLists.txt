message(STATUS "test: ${CMAKE_CURRENT_SOURCE_DIR}")

file(GLOB_RECURSE BTUNITTEST_SRC  "${CMAKE_CURRENT_SOURCE_DIR}/*.cpp")
file(GLOB_RECURSE BTUNITTEST_INC  "${CMAKE_CURRENT_SOURCE_DIR}/*.h")

# add the binary tree to the search path for include files
# so that we will find TutorialConfig.h
include_directories("${PROJECT_SOURCE_DIR}/inc")
include_directories(${CMAKE_CURRENT_SOURCE_DIR})

# set(BTUNITTEST_SRC_INC ${BTUNITTEST_SRC} ${BTUNITTEST_INC})

# add the executable
add_executable(btunittest ${BTUNITTEST_INC} ${BTUNITTEST_SRC})

# target_link_libraries (btunittest libbehaviac ${CMAKE_THREAD_LIBS_INIT} ${LINKER_SCRIPT})
target_link_libraries (btunittest libbehaviac ${CMAKE_THREAD_LIBS_INIT})

# if (BEHAVIAC_RELEASE_ON)
	# set_target_properties(btunittest PROPERTIES INTERPROCEDURAL_OPTIMIZATION True)
# endif()

set_target_properties(btunittest PROPERTIES PREFIX "")
# set_target_properties(btunittest PROPERTIES OUTPUT_NAME_DEBUG libbehaviac${${CMAKE_DEBUG_POSTFIX}}
# set_target_properties(btunittest PROPERTIES OUTPUT_NAME_RELEASE libbehaviac${${CMAKE_RELEASE_POSTFIX}}
set_target_properties(btunittest PROPERTIES DEBUG_POSTFIX ${CMAKE_DEBUG_POSTFIX})
set_target_properties(btunittest PROPERTIES RELEASE_POSTFIX ${CMAKE_RELEASE_POSTFIX})

if(MSVC)
   SET_TARGET_PROPERTIES(btunittest PROPERTIES LINK_FLAGS "/MAP")
elseif(XCODE)
	#
elseif(CMAKE_COMPILER_IS_GNUCC OR CMAKE_COMPILER_IS_GNUCXX)
   set (CMAKE_EXE_LINKER_FLAGS "${CMAKE_EXE_LINKER_FLAGS} -Wl,-Map=btunittest_gcc_${CMAKE_BUILD_TYPE}.map")
endif()