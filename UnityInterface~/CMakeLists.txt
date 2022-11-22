cmake_minimum_required(VERSION 3.2.2)
set(CMAKE_EXPORT_COMPILE_COMMANDS ON)

project(UnityInterface LANGUAGES CXX)
set(CXX_STANDARD 17)

file(GLOB_RECURSE PLUGIN_SRC "*.cpp")

add_library(${PROJECT_NAME} SHARED ${PLUGIN_SRC})

find_package(Vulkan REQUIRED)

target_link_libraries(${PROJECT_NAME} PUBLIC Vulkan::Vulkan)
target_include_directories(${PROJECT_NAME} PUBLIC ${CMAKE_CURRENT_SOURCE_DIR})
target_compile_features(${PROJECT_NAME} PUBLIC cxx_std_17)
set_target_properties(${PROJECT_NAME} PROPERTIES PREFIX "")

SET(LIBRARY_OUTPUT_PATH ${CMAKE_CURRENT_SOURCE_DIR}/../Plugins)