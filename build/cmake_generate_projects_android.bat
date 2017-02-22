@echo off

echo please visit http://www.behaviac.com/docs/zh/articles/build/ for more information
echo ---------------------------------------------------------------------------------

where cmake
IF %ERRORLEVEL% NEQ 0 GOTO l_cmake_error

mkdir cmake_binary
cd cmake_binary

rem ----------------------------------------------------
mkdir android
cd android
cmake -G "Visual Studio 14 ARM" -DCMAKE_SYSTEM_NAME=VCMDDAndroid --build ../../..
cd ..

rem mkdir example_airbattledemo_android
rem cd example_airbattledemo_android

rem cmake -G "Visual Studio 14 ARM" -DCMAKE_SYSTEM_NAME=VCMDDAndroid --build ../../../example/airbattledemo
rem cd ..

rem ----------------------------------------------------
rem back cmake_binary
cd ..

goto l_end

:l_cmake_error
echo please make sure you have installed cmake 3.3 above (https://cmake.org/files/)
echo and please add cmake's Path to the environment 'PATH'

pause

:l_end
