cmake_minimum_required(VERSION 3.2.2)
set(CMAKE_EXPORT_COMPILE_COMMANDS ON)

project(NativePlugin LANGUAGES CXX)
set(CXX_STANDARD 17)

file(GLOB_RECURSE PLUGIN_SRC "*.cpp")

add_library(${PROJECT_NAME} SHARED ${PLUGIN_SRC})

target_include_directories(${PROJECT_NAME} PUBLIC UnityInterface Include)
target_compile_features(${PROJECT_NAME} PUBLIC cxx_std_17)

SET(LIBRARY_OUTPUT_PATH ${CMAKE_CURRENT_SOURCE_DIR}/../Test/Plugins)