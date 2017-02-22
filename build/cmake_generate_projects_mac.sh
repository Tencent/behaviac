
echo "please visit http://www.behaviac.com/docs/zh/articles/build/ for more information"
echo ---------------------------------------------------------------------------------

mkdir -p cmake_binary
cd cmake_binary

# --------------------------------------------------------------
mkdir -p linux
cd linux

mkdir -p debug
cd debug
cmake -G "Unix Makefiles" -DCMAKE_BUILD_TYPE=Debug -DBUILD_USE_64BITS=ON --build ../../../..
cd ..

mkdir -p release
cd release
cmake -G "Unix Makefiles" -DCMAKE_BUILD_TYPE=Release -DBUILD_USE_64BITS=ON --build ../../../..
cd ..

# linux
cd ..
# --------------------------------------------------------------
mkdir -p xcode
cd xcode
# cmake -G "Xcode" -DBUILD_USE_64BITS=ON --build ../../..
# cmake -G "Xcode" -DCMAKE_BUILD_TYPE=Release --build ../../..
# cmake -G "Xcode" --build -DCMAKE_BUILD_TYPE=Debug ../../..
cmake -G "Xcode" --build ../../..
cd ..
# --------------------------------------------------------------
# mkdir -p sublime
# cd sublime
# cmake -G "Sublime Text 2 - Unix Makefiles" --build ../../..
# cd ..

# back cmake_binary
cd ..
