
echo please visit http://www.behaviac.com/docs/zh/articles/build/ for more information
echo ---------------------------------------------------------------------------------

mkdir -p cmake_binary
cd cmake_binary

# --------------------------------------------------------------
mkdir -p linux
cd linux

# by default, to use 64 bits on a 64 bit arch, you can modify the following at your will
USE_64BITS='ON'
# uname -m | grep 64
if [ `getconf LONG_BIT` = "64" ]
then
    echo "64-bit"
    USE_64BITS='ON'
else
    echo "32-bit"
    USE_64BITS='OFF'
fi

# echo $USE_64BITS

echo ---------------------------------------------------------------------------------
mkdir -p debug
cd debug
cmake -G "Unix Makefiles" -DCMAKE_BUILD_TYPE=Debug -DBUILD_USE_64BITS=$USE_64BITS --build ../../../..
cd ..

echo ---------------------------------------------------------------------------------
mkdir -p release
cd release
# cmake -G "Unix Makefiles" -DCMAKE_BUILD_TYPE=Release -DBEHAVIAC_VERSION_MODE=ForeUseRelease -DBUILD_USE_64BITS=ON --build ../../../..
cmake -G "Unix Makefiles" -DCMAKE_BUILD_TYPE=Release -DBUILD_USE_64BITS=$USE_64BITS --build ../../../..
cd ..

# linux
cd ..

# --------------------------------------------------------------
# mkdir -p sublime_release
# cd sublime_release
# cmake -G "Sublime Text 2 - Unix Makefiles" -DCMAKE_BUILD_TYPE=Release --build ../../..
# cd ..

# --------------------------------------------------------------
# mkdir -p sublime_debug
# cd sublime_debug
# cmake -G "Sublime Text 2 - Unix Makefiles" -DCMAKE_BUILD_TYPE=Debug --build ../../..
# cd ..

# back cmake_binary
cd ..
