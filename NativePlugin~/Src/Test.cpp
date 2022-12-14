#include "IUnityInterface.h"

#ifdef __cplusplus
extern "C"
{
#endif
    int UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API native_add(int a, int b)
    {
        return a + b;
    }

    IUnityInterfaces* g_unityInterfaces = nullptr;

    void UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API UnityPluginLoad(IUnityInterfaces* unityInterfaces)
    {
        g_unityInterfaces = unityInterfaces;
    }

    void UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API UnityPluginUnload()
    {
        g_unityInterfaces = nullptr;
    }

#ifdef __cplusplus
}
#endif