using System;
using GTC.BEngine;
using UnityEngine;

[PluginAttr("NativePlugin")]
public static class MyPlugin
{
    public delegate int AddDelegate(int a, int b);
    [PluginFunctionAttr("native_add")] public static AddDelegate native_add = null;
}

public class NativePlugin : MonoBehaviour
{
    private NativePluginLoader loader;

    private void Start()
    {
        loader = new NativePluginLoader(typeof(MyPlugin), "Assets/Plugins");

        Debug.Log(MyPlugin.native_add(1, 2));
    }

    private void OnDestroy()
    {
        loader?.Dispose();
        loader = null;
    }
}