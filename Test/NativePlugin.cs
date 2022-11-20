using GTC.BEngine;
using UnityEngine;

[PluginAttr("libNativePlugin")]
public static class MyPlugin
{
    public delegate int Sum(int a, int b);

    [PluginFunctionAttr("native_add")] public static Sum native_add = null;
}

[ExecuteAlways]
public class NativePlugin : MonoBehaviour
{
    void OnEnable()
    {
        NativePluginLoader.LoadAll(typeof(MyPlugin), "Assets/UnityNativePluginLoader/Test/Plugins/");
        Debug.Log(MyPlugin.native_add(1, 2));
    }

    private void OnDisable()
    {
        NativePluginLoader.UnloadAll();
    }
}