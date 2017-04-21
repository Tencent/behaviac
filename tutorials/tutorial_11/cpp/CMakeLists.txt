message(STATUS "tutorials: ${CMAKE_CURRENT_SOURCE_DIR}")

file(GLOB_RECURSE BTUNITTEST_SRC  "${CMAKE_CURRENT_SOURCE_DIR}/*.cpp")
file(GLOB_RECURSE BTUNITTEST_INC  "${CMAKE_CURRENT_SOURCE_DIR}/*.h")

# add the binary tree to the search path for include files
# so that we will find TutorialConfig.h
include_directories("${PROJECT_SOURCE_DIR}/inc")
include_directories(${CMAKE_CURRENT_SOURCE_DIR})

# set(BTUNITTEST_SRC_INC ${BTUNITTEST_SRC} ${BTUNITTEST_INC})

if (BEHAVIAC_ANDROID_STUDIO)
	add_library(tutorial_11 SHARED ${BTUNITTEST_INC} ${BTUNITTEST_SRC})
else()
	# add the executable
	add_executable(tutorial_11 ${BTUNITTEST_INC} ${BTUNITTEST_SRC})
endif()

target_link_libraries (tutorial_11 libbehaviac ${CMAKE_THREAD_LIBS_INIT})

if (NOT BEHAVIAC_ANDROID_STUDIO)
set_target_properties(tutorial_11 PROPERTIES PREFIX "")
# set_target_properties(tutorial_11 PROPERTIES OUTPUT_NAME_DEBUG libbehaviac${${CMAKE_DEBUG_POSTFIX}}
# set_target_properties(tutorial_11 PROPERTIES OUTPUT_NAME_RELEASE libbehaviac${${CMAKE_RELEASE_POSTFIX}}
set_target_properties(tutorial_11 PROPERTIES DEBUG_POSTFIX ${CMAKE_DEBUG_POSTFIX})
set_target_properties(tutorial_11 PROPERTIES RELEASE_POSTFIX ${CMAKE_RELEASE_POSTFIX})
endif()
