#
# If the user specifies -DCMAKE_BUILD_TYPE on the command line, take their definition
# and dump it in the cache along with proper documentation, 
# otherwise set CMAKE_BUILD_TYPE to Release prior to calling PROJECT()
#

	
if( DEFINED CMAKE_BUILD_TYPE )
	set( CMAKE_BUILD_TYPE ${CMAKE_BUILD_TYPE} CACHE STRING "Set to either \"Release\" or \"Debug\"" )
else()
	set( CMAKE_BUILD_TYPE Release CACHE STRING "Set to either \"Release\" or \"Debug\"" )
endif()

# message( STATUS "CMAKE_BUILD_TYPE = ${CMAKE_BUILD_TYPE}" )

mark_as_advanced(CMAKE_CONFIGURATION_TYPES)
project (behaviac)

message( STATUS "MSVC=${MSVC}" )

# if (MSVC)
	# cmake_minimum_required (VERSION 3.3)
# else()
	# cmake_minimum_required (VERSION 2.8)
# endif()

cmake_minimum_required (VERSION 2.8)

# SET(CMAKE_USE_RELATIVE_PATHS ON)

# set(CMAKE_CONFIGURATION_TYPES "Debug;Release;Profile" CACHE STRING "" FORCE) 
set(CMAKE_CONFIGURATION_TYPES "Debug;Release" CACHE STRING "" FORCE) 

set (BEHAVIAC_PACKAGE_VERSION 3.6.39)

#option( BUILD_SHARED_LIBS "set to OFF to build static libraries" ON )
SET(BUILD_SHARED_LIBS ON CACHE BOOL "set to OFF to build static libraries")

# configure a header file to pass some of the CMake settings
# to the source code
#CONFIGURE_FILE ("${PROJECT_SOURCE_DIR}/Config.h.in" "${PROJECT_BINARY_DIR}/Config.h")

file(GLOB_RECURSE BEHAVIAC_INC  "${PROJECT_SOURCE_DIR}/inc/*.*")
file(GLOB_RECURSE BEHAVIAC_SRC  "${PROJECT_SOURCE_DIR}/src/*.*")

# exclude_dir = "/exclude_dir/"
macro (ExcludeDir src_files exclude_dir)
	foreach (TMP_PATH ${src_files})
		string (FIND ${TMP_PATH} ${exclude_dir} EXCLUDE_DIR_FOUND)
		if (NOT ${EXCLUDE_DIR_FOUND} EQUAL -1)
			list (REMOVE_ITEM ${src_files} ${TMP_PATH})
		endif ()
	endforeach(TMP_PATH)
endmacro()



# usage: GroupSources("${PROJECT_SOURCE_DIR}/inc/", "inc")
macro(GroupSources dir prefix)
    # file(GLOB_RECURSE elements RELATIVE ${dir} *.h *.hpp *.c *.cpp *.cc)
    file(GLOB_RECURSE elements RELATIVE ${dir} *.*)

    foreach(element ${elements})
      # Extract filename and directory
      get_filename_component(element_name ${element} NAME)
      get_filename_component(element_dir ${element} DIRECTORY)

      if (NOT ${element_dir} STREQUAL "")
        # If the file is in a subdirectory use it as source group.
          # Use the full hierarchical structure to build source_groups.
          string(REPLACE "/" "\\" group_name ${element_dir})
          source_group("${prefix}\\${group_name}" FILES ${dir}/${element})
      else()
        # If the file is in the root directory, place it in the root source_group.
        source_group("${prefix}\\" FILES ${dir}/${element})
      endif()
    endforeach()
endmacro()

if (MSVC)
	GroupSources("${PROJECT_SOURCE_DIR}/inc/" "inc")
	GroupSources("${PROJECT_SOURCE_DIR}/src/" "src")
endif()


# source_group(inc FILES ${BEHAVIAC_INC})
# source_group(src FILES ${BEHAVIAC_SRC})

# add the binary tree to the search path for include files
# so that we will find TutorialConfig.h
include_directories ("${PROJECT_SOURCE_DIR}/inc")

if( DEFINED BEHAVIAC_VERSION_MODE )
	set(BEHAVIAC_VERSION_MODE ${BEHAVIAC_VERSION_MODE} CACHE STRING "version mode, force to use dev/release version or default(dev version on Debug config and release version on Release config)")
else()
	set(BEHAVIAC_VERSION_MODE "Default" CACHE STRING "version mode, force to use dev/release version or default(dev version on Debug config and release version on Release config)")
endif()

SET_PROPERTY(CACHE BEHAVIAC_VERSION_MODE PROPERTY STRINGS Default ForceUseDev ForeUseRelease) 
if(NOT(DEFINED BEHAVIAC_VERSION_MODE_LAST))
	 set(BEHAVIAC_VERSION_MODE_LAST "NotAnOption" CACHE STRING "last version mode")
	 MARK_AS_ADVANCED (FORCE BEHAVIAC_VERSION_MODE_LAST)
endif()


# set(BEHAVIAC_VERSION_MODE "ForeUseRelease" CACHE STRING "version mode)" FORCE)

if(NOT (${BEHAVIAC_VERSION_MODE} MATCHES ${BEHAVIAC_VERSION_MODE_LAST}))
     set(BEHAVIAC_VERSION_MODE_LAST ${BEHAVIAC_VERSION_MODE} CACHE STRING "Updating version mode option" FORCE)
endif()

set (BEHAVIAC_RELEASE_CUSTOMIZED 0)
set (BEHAVIAC_RELEASE_ON 0)

if(${BEHAVIAC_VERSION_MODE} MATCHES "ForceUseDev")
	set (BEHAVIAC_RELEASE_CUSTOMIZED 1)
	set (BEHAVIAC_RELEASE_ON 0)
endif()
if(${BEHAVIAC_VERSION_MODE} MATCHES "ForeUseRelease")
	set (BEHAVIAC_RELEASE_CUSTOMIZED 1)
	set (BEHAVIAC_RELEASE_ON 1)
endif()

# set (LINKER_SCRIPT "")

if (CMAKE_GENERATOR MATCHES "Xcode")
	message( STATUS "XCODE = ${XCODE}" )
endif()
 
# if(MSVC OR (CMAKE_GENERATOR MATCHES "Xcode"))
if(MSVC OR XCODE)
	set_property(DIRECTORY APPEND PROPERTY COMPILE_DEFINITIONS
		$<$<CONFIG:Debug>:_DEBUG>
		$<$<CONFIG:Release>:NDEBUG>
	)
	
	if (XCODE)
		set(XCODE_ATTRIBUTE_GCC_PREPROCESSOR_DEFINITIONS[variant=Debug] "_DEBUG")
		set(XCODE_ATTRIBUTE_GCC_PREPROCESSOR_DEFINITIONS[variant=Release] "NDEBUG")
	endif()
else()
	if( CMAKE_BUILD_TYPE MATCHES Debug )
		#add_definitions( "-DDEBUG" )
		add_definitions( "-D_DEBUG" )
	else()
		add_definitions( "-DNDEBUG" )
	endif()
endif()


if(BEHAVIAC_RELEASE_ON)

	if (MSVC)
		set(CMAKE_CXX_FLAGS_RELEASE "${CMAKE_CXX_FLAGS_RELEASE} /Gy /GL")
		set(CMAKE_EXE_LINKER_FLAGS_RELEASE "${CMAKE_EXE_LINKER_FLAGS} /OPT:ICF /OPT:REF /LTCG")
		set(CMAKE_MODULE_LINKER_FLAGS_RELEASE "${CMAKE_MODULE_LINKER_FLAGS} /OPT:ICF /OPT:REF /LTCG")
	else()
		include( CheckCXXCompilerFlag )
		check_cxx_compiler_flag( -flto GXX_HAS_LTO_FLAG )

		message( STATUS "GXX_HAS_LTO_FLAG = ${GXX_HAS_LTO_FLAG}" )

		if( CMAKE_BUILD_TYPE MATCHES Release AND GXX_HAS_LTO_FLAG )
			find_program(CMAKE_GCC_AR NAMES ${_CMAKE_TOOLCHAIN_PREFIX}gcc-ar${_CMAKE_TOOLCHAIN_SUFFIX} HINTS ${_CMAKE_TOOLCHAIN_LOCATION})
			find_program(CMAKE_GCC_NM NAMES ${_CMAKE_TOOLCHAIN_PREFIX}gcc-nm HINTS ${_CMAKE_TOOLCHAIN_LOCATION})
			find_program(CMAKE_GCC_RANLIB NAMES ${_CMAKE_TOOLCHAIN_PREFIX}gcc-ranlib HINTS ${_CMAKE_TOOLCHAIN_LOCATION})

			if( CMAKE_GCC_AR AND CMAKE_GCC_NM AND CMAKE_GCC_RANLIB )
				message( STATUS "SET LTO Flags" )

				# set( CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} -flto -fno-fat-lto-objects -O3 -ffunction-sections -fdata-sections -fvisibility=hidden -fvisibility-inlines-hidden" )
				set( CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} -flto -O3" )
				
				check_cxx_compiler_flag( -fno-fat-lto-objects GXX_HAS_NO_FAT_LTO_FLAG )
				
				message( STATUS "GXX_HAS_NO_FAT_LTO_FLAG = ${GXX_HAS_NO_FAT_LTO_FLAG}" )
				
				if (GXX_HAS_NO_FAT_LTO_FLAG)
					set( CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} -fno-fat-lto-objects" )
				endif()
				
				set( CMAKE_AR "${CMAKE_GCC_AR}" )
				set( CMAKE_NM "${CMAKE_GCC_NM}" )
				set( CMAKE_RANLIB "${CMAKE_GCC_RANLIB}" )
				
				# set(CMAKE_MODULE_LINKER_FLAGS "${CMAKE_MODULE_LINKER_FLAGS} -Wl,--gc-sections")
				# set(CMAKE_EXE_LINKER_FLAGS "${CMAKE_EXE_LINKER_FLAGS} -Wl,--gc-sections")
			else()
				message( WARNING "GCC indicates LTO support, but binutils wrappers could not be found. Disabling LTO." )
			endif()
		endif()
		
		# -fipa-icf
		# -finline-limit=64 
		# -fno-exceptions -fno-rtti
		# -fwhole-program
		# set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} -ffunction-sections -fdata-sections -fvisibility=hidden -O3 -flto")
		# set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} -Os -flto -fno-fat-lto-objects")
		# set(CMAKE_MODULE_LINKER_FLAGS "${CMAKE_MODULE_LINKER_FLAGS} -Wl,--gc-sections -flto")
		# set(CMAKE_EXE_LINKER_FLAGS "${CMAKE_EXE_LINKER_FLAGS} -Wl,--gc-sections -flto")
		# set(CMAKE_STATIC_LINKER_FLAGS_RELEASE "${CMAKE_STATIC_LINKER_FLAGS_RELEASE} -flto")
		
		# find_package(Gold)
		 
		# if (GOLD_FOUND)
			# SET(LINKER_SCRIPT -fuse-ld=gold -Wl,--icf=safe)
		# endif()	
	endif()
endif()


SET(BUILD_USE_64BITS OFF CACHE BOOL "set to ON to use 64 bits")	

if (MSVC)
	if (CMAKE_GENERATOR MATCHES "64")
		SET(BUILD_USE_64BITS ON CACHE BOOL "CMAKE_GENERATOR is for 64 bit, set to ON accordingly" FORCE)	
	endif()
else()
	# SET(BUILD_USE_64BITS ON CACHE BOOL "CMAKE_GENERATOR is for 64 bit, set to ON accordingly" FORCE)	
endif()

if (BUILD_USE_64BITS)
	if (MSVC)
		if (NOT CMAKE_GENERATOR MATCHES "64")
			#
			message( SEND_ERROR "You need to specify '${CMAKE_GENERATOR} Win64' to generate project file for 64 bit" )
		endif()
	else()
		set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} -m64")
		# set (CMAKE_OSX_ARCHITECTURES x86_64)
	endif()
endif()

message( STATUS "CMAKE_SYSTEM_NAME = ${CMAKE_SYSTEM_NAME}" )
message( STATUS "BUILD_USE_64BITS = ${BUILD_USE_64BITS}" )

message( STATUS "CMAKE_GENERATOR = ${CMAKE_GENERATOR}" )

if (MSVC)
	# message( STATUS "MSVC" )
	
	set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} /bigobj")
endif()

if (VC_MDD_ANDROID)
	message( STATUS "VC_MDD_ANDROID" )
	cmake_minimum_required (VERSION 3.4)
endif()

set (BEHAVIAC_ANDROID_ON 0)

# demo of the variables being set when VCMDDAndroid is targeted
if(CMAKE_GENERATOR MATCHES "Visual Studio 14" AND CMAKE_SYSTEM_NAME MATCHES "VCMDDAndroid")
	# message(STATUS "VCMDDAndroid")
	set (BEHAVIAC_ANDROID_ON 1)
endif()

set (BEHAVIAC_ANDROID_STUDIO 0)

if(CMAKE_GENERATOR MATCHES "Ninja" AND CMAKE_SYSTEM_NAME MATCHES "Android")
	set (BEHAVIAC_ANDROID_STUDIO 1)
endif()

set (COMPILE_POSTFIX "")


if(MSVC OR BEHAVIAC_ANDROID_ON)
	message(STATUS "set msvc flags")
	# Force to always compile with W4
	# if(CMAKE_CXX_FLAGS MATCHES "/W[0-4]")
		# STRING(REGEX REPLACE "/W[0-4]" "/W4" CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS}")
	# else()
		# set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} /W4")
	# endif()
    set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} /W4")
	
	#warning treated as error
	set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} /WX")
	#multiple processor compilation
	set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} /MP /EHs")
	set (COMPILE_POSTFIX "msvc")
	
	# hide CMAKE_BUILD_TYPE on msvc
	mark_as_advanced(CMAKE_BUILD_TYPE)

	if (BEHAVIAC_ANDROID_ON)
		message(STATUS "set msvc flags for android")
		set (BUILD_SHARED_LIBS OFF CACHE BOOL "set to OFF to build static libraries")
		mark_as_advanced(BUILD_SHARED_LIBS)
		set (COMPILE_POSTFIX "msvc_android")
	endif()
else()
	# Update if necessary
	set (BUILD_SHARED_LIBS OFF CACHE BOOL "set to OFF to build static libraries" FORCE)
	mark_as_advanced(BUILD_SHARED_LIBS)
	mark_as_advanced(CMAKE_GNUtoMS)
	
 	if (CMAKE_COMPILER_IS_GNUCC OR CMAKE_COMPILER_IS_GNUCXX)
		message(STATUS "set gcc flags")
		find_package(Threads REQUIRED)

		set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} -Wall")
		set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} -Werror -Woverloaded-virtual -Wnon-virtual-dtor -Wfloat-equal -Wno-unused-local-typedefs -finput-charset=UTF-8 ")
        # set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} -Wa,-mbig-obj")
    endif()
	
	# if (BEHAVIAC_ANDROID_STUDIO)
	#    set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} -mxgot")
	# endif()
	
	# if(CMAKE_GENERATOR STREQUAL Xcode)
	# 	set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} -Wunused-private-field")
	# endif()

	set (COMPILE_POSTFIX "gcc")
endif()

message( STATUS "BUILD_SHARED_LIBS = ${BUILD_SHARED_LIBS}" )
message( STATUS "BUILD_ENABLE_LUA = ${BUILD_ENABLE_LUA}" )
message( STATUS "BEHAVIAC_VERSION_MODE = ${BEHAVIAC_VERSION_MODE}" )
message( STATUS "CMAKE_INSTALL_PREFIX = ${CMAKE_INSTALL_PREFIX}" )
# message( STATUS "BEHAVIAC_VERSION = ${BEHAVIAC_VERSION}" )
if(MSVC OR BEHAVIAC_ANDROID_ON)
	message( STATUS "Config = Debug|Release" )
else()
	message( STATUS "CMAKE_BUILD_TYPE = ${CMAKE_BUILD_TYPE}" )
endif()
# MESSAGE( STATUS "CMAKE_MODULE_PATH = ${CMAKE_MODULE_PATH}" )

configure_file (
  "${PROJECT_SOURCE_DIR}/build/_config.h.in"
  "${PROJECT_SOURCE_DIR}/inc/behaviac/common/_config.h"
  NEWLINE_STYLE WIN32
  )

if (NOT BEHAVIAC_ANDROID_STUDIO)
    set(LIB_OUTPUT_PATH ${PROJECT_SOURCE_DIR}/lib)
endif()
set(BINARY_PATH ${PROJECT_SOURCE_DIR}/bin)

set( CMAKE_RUNTIME_OUTPUT_DIRECTORY ${BINARY_PATH} )
if (NOT BEHAVIAC_ANDROID_STUDIO)
    set( CMAKE_LIBRARY_OUTPUT_DIRECTORY ${LIB_OUTPUT_PATH} )
    set( CMAKE_ARCHIVE_OUTPUT_DIRECTORY ${LIB_OUTPUT_PATH} )
endif()

set( CMAKE_DEBUG_POSTFIX "_${COMPILE_POSTFIX}_debug" )
set( CMAKE_RELEASE_POSTFIX "_${COMPILE_POSTFIX}_release" )

foreach( OUTPUTCONFIG ${CMAKE_CONFIGURATION_TYPES} )
    string( TOUPPER ${OUTPUTCONFIG} OUTPUTCONFIG )
    set( CMAKE_RUNTIME_OUTPUT_DIRECTORY_${OUTPUTCONFIG} ${BINARY_PATH} )

if (NOT BEHAVIAC_ANDROID_STUDIO)
    set( CMAKE_LIBRARY_OUTPUT_DIRECTORY_${OUTPUTCONFIG} ${LIB_OUTPUT_PATH} )
    set( CMAKE_ARCHIVE_OUTPUT_DIRECTORY_${OUTPUTCONFIG} ${LIB_OUTPUT_PATH} )
endif()
endforeach( OUTPUTCONFIG CMAKE_CONFIGURATION_TYPES )

set( SHARED_LIB "" )

if (BEHAVIAC_ANDROID_STUDIO)
    set( SHARED_LIB "SHARED" )
endif()

add_library(libbehaviac ${SHARED_LIB} ${BEHAVIAC_INC} ${BEHAVIAC_SRC} ${LINKER_SCRIPT})

set_target_properties(libbehaviac PROPERTIES PREFIX "")
# set_target_properties(libbehaviac PROPERTIES OUTPUT_NAME_DEBUG libbehaviac${${CMAKE_DEBUG_POSTFIX}}
# set_target_properties(libbehaviac PROPERTIES OUTPUT_NAME_RELEASE libbehaviac${${CMAKE_RELEASE_POSTFIX}}

set_target_properties(libbehaviac PROPERTIES DEBUG_POSTFIX ${CMAKE_DEBUG_POSTFIX})
set_target_properties(libbehaviac PROPERTIES RELEASE_POSTFIX ${CMAKE_RELEASE_POSTFIX})

# foreach( OUTPUTCONFIG ${CMAKE_CONFIGURATION_TYPES} )
#     string( TOUPPER ${OUTPUTCONFIG} OUTPUTCONFIG )
# 	set_target_properties(libbehaviac PROPERTIES
# 		${OUTPUTCONFIG}_OUTPUT_NAME libBehaviac_${OUTPUTCONFIG}
# 		)
# 	#     ARCHIVE_OUTPUT_DIRECTORY_${OUTPUTCONFIG}   ${LIB_OUTPUT_PATH}
# 	#     LIBRARY_OUTPUT_DIRECTORY_${OUTPUTCONFIG}   ${LIB_OUTPUT_PATH}
# 	#     RUNTIME_OUTPUT_DIRECTORY_${OUTPUTCONFIG}   ${BINARY_PATH})
# endforeach( OUTPUTCONFIG CMAKE_CONFIGURATION_TYPES )

if (BEHAVIAC_ANDROID_STUDIO)
	target_link_libraries(libbehaviac log android)
endif()

# Cmake comes with a builtin add_definitions command to add definitions to
# everything in the current directory, but we need to add definitions to
# targets. 
function (add_target_definitions target)
  get_target_property(defs ${target} COMPILE_DEFINITIONS)
  if (defs MATCHES "NOTFOUND")
    set(defs "")
  endif ()
  foreach (def ${defs} ${ARGN})
    list(APPEND deflist ${def})
  endforeach ()
  set_target_properties(${target} PROPERTIES COMPILE_DEFINITIONS "${deflist}")
endfunction ()

# if (XCODE)
	# set_target_properties(libbehaviac PROPERTIES  XCODE_ATTRIBUTE_GCC_PREPROCESSOR_DEFINITIONS[variant=Debug] "_DEBUG")
	# set_target_properties(libbehaviac PROPERTIES  XCODE_ATTRIBUTE_GCC_PREPROCESSOR_DEFINITIONS[variant=Release] "NDEBUG")
# endif()

if( BUILD_SHARED_LIBS )
	# SET_TARGET_PROPERTIES(libbehaviac PROPERTIES
	# 	COMPILE_DEFINITIONS "BEHAVIACDLL_EXPORTS")
	message( STATUS "build shared libs" )
	add_target_definitions(libbehaviac "BEHAVIACDLL_EXPORTS" "BEHAVIAC_DLL")
else()
	message( STATUS "build static libs" )
endif()

if (NOT BEHAVIAC_ANDROID_STUDIO)
	add_subdirectory ("${PROJECT_SOURCE_DIR}/test/btunittest")
	add_subdirectory ("${PROJECT_SOURCE_DIR}/test/btremotetest")
	add_subdirectory ("${PROJECT_SOURCE_DIR}/test/demo_running")
	add_subdirectory ("${PROJECT_SOURCE_DIR}/test/usertest")

	if (MSVC)
		add_subdirectory ("${PROJECT_SOURCE_DIR}/test/btperformance")
	    if (EXISTS "${PROJECT_SOURCE_DIR}/example/spaceship")
	        add_subdirectory ("${PROJECT_SOURCE_DIR}/example/spaceship")
	    endif()
	endif()

	#add_subdirectory ("${PROJECT_SOURCE_DIR}/example/airbattledemo")

	add_subdirectory ("${PROJECT_SOURCE_DIR}/tutorials/tutorial_1/cpp")
	add_subdirectory ("${PROJECT_SOURCE_DIR}/tutorials/tutorial_1_1/cpp")
	add_subdirectory ("${PROJECT_SOURCE_DIR}/tutorials/tutorial_1_2/cpp")
	add_subdirectory ("${PROJECT_SOURCE_DIR}/tutorials/tutorial_2/cpp")
	add_subdirectory ("${PROJECT_SOURCE_DIR}/tutorials/tutorial_3/cpp")
	add_subdirectory ("${PROJECT_SOURCE_DIR}/tutorials/tutorial_4/cpp")
	add_subdirectory ("${PROJECT_SOURCE_DIR}/tutorials/tutorial_5/cpp")
	add_subdirectory ("${PROJECT_SOURCE_DIR}/tutorials/tutorial_6/cpp")
	add_subdirectory ("${PROJECT_SOURCE_DIR}/tutorials/tutorial_7/cpp")
	add_subdirectory ("${PROJECT_SOURCE_DIR}/tutorials/tutorial_8/cpp")
	add_subdirectory ("${PROJECT_SOURCE_DIR}/tutorials/tutorial_9/cpp")
	add_subdirectory ("${PROJECT_SOURCE_DIR}/tutorials/tutorial_10/cpp")
	add_subdirectory ("${PROJECT_SOURCE_DIR}/tutorials/tutorial_11/cpp")
	add_subdirectory ("${PROJECT_SOURCE_DIR}/tutorials/tutorial_12/cpp")
	add_subdirectory ("${PROJECT_SOURCE_DIR}/tutorials/tutorial_13/cpp")
	add_subdirectory ("${PROJECT_SOURCE_DIR}/tutorials/tutorial_14/cpp")
else()
	add_subdirectory ("${PROJECT_SOURCE_DIR}/tutorials/tutorial_11/cpp")
endif()
