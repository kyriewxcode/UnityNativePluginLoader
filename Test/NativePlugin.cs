using System;
using GTC.BEngine;
using UnityEngine;

[PluginAttr("NativePlugin")]
public static class MyPlugin
{
    public delegate int AddDelegate(int a, int b);
    [PluginFunctionAttr("native_add")] public static AddDelegate native_add = null;

    public delegate void UnityPluginLoadDelegate(IntPtr unityInterfaces);
    [PluginFunctionAttr("UnityPluginLoad")]
    public static UnityPluginLoadDelegate UnityPluginLoad = null;

    public delegate void UnityPluginUnloadDelegate();
    [PluginFunctionAttr("UnityPluginUnload")]
    public static UnityPluginUnloadDelegate UnityPluginUnload = null;
}

public class NativePlugin : MonoBehaviour
{
    void OnEnable()
    {
        NativePluginLoader.LoadAll(typeof(MyPlugin), "Assets/Plugins");

        IntPtr unityInterface = NativePluginLoader.GetUnityInterface();
        MyPlugin.UnityPluginLoad(unityInterface);

        Debug.Log(MyPlugin.native_add(1, 2));
    }

    private void OnDisable()
    {
        MyPlugin.UnityPluginUnload();
        NativePluginLoader.UnloadAll();
    }
}