# Unity Native Plugin Loader

## 简介

在为Unity写Native插件时，调式的时候总是会遇到一个问题，重新编译插件时会遇到Plugin一直被Unity引用而无法删除。这个时候就只能关掉Unity编辑器—>替换DLL—>再打开Unity。这个操作对于开发过程中频繁的修改Plugin代码和测试的时候的效率影响是非常大的，所以这里写了一个解决方案，用以解决上述问题。



## 用法

可以从仓库的Test文件夹下的测试案例中学到用法。

首先是创建一个Plugin类，加上“PluginAttr” Attribute，Attribute里的pluginName需要和Plugin的名字相同

```c#
[PluginAttr("libNativePlugin")]
public static class MyPlugin
{
}
```

然后在类里加入Plugin里需要调用的函数相同返回值和形参的delegate，再写入Plugin中对应该delegate的Function，为Function加上 “PluginFunctionAttr” Attribute，Atrtribute里的functionName需要和Plugin里的FunctionName相同

```c#

[PluginAttr("libNativePlugin")]
public static class MyPlugin
{
    public delegate int Sum(int a, int b);
    [PluginFunctionAttr("native_add")] public static Sum native_add = null;
}
```

这样就定义好了一个Plugin里的几个Functiion类

然后在运行时，直接实例化一个`NativePluginLoader` 其中`typeof(MyPlugin)`为上述封装的Plugin类，`pluginPath`为Plugin的文件夹路径

随后调用函数时就可以：

```c#
loader = new NativePluginLoader(typeof(MyPlugin), "Assets/Plugins");
Debug.Log(MyPlugin.native_add(1, 2));
```

用完之后，直接对loader进行Dispose即可释放Plugin

```c#
loader?.Dispose();
loader = null;
```

释放之后就可以随意更换DLL了 :)



## 注意事项

​	在[Unity官方文档](https://docs.unity3d.com/Manual/NativePluginInterface.html)中，在插件加载和卸载时，Unity会自动调用`UnityPluginLoad(UnityInterface* interface)`和`UnityPluginUnload()`，Unity的图形接口都包含在其中。但是这是在用DllImport的前提下才会触发，所以如果用本仓库方法手动加载Plugin时，必须先从一个单独的Plugin中拿到`UnityInterface`的指针，然后手动调用UnityPluginLoad并传入前面得到的指针参数。

​	仓库中的 `Plugin/UnityInterface.dll` 中已经提供了相关接口，并且已经进行内置调用，你只需要按照上述用法并且在Plugin里定义好`UnityPluginLoad(UnityInterface* interface)`和`UnityPluginUnload()`两个方法即可。

​	Plugin源代码位于`NativePlugin~`和`UnityInterface~`文件夹下，所有示例都在`Test/NativePlugin.cs`中演示。



## Reference

[How to Reload Native Plugins in Unity](https://www.forrestthewoods.com/blog/how-to-reload-native-plugins-in-unity/)
