﻿# Unity Native Plugin Loader

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

```C#

[PluginAttr("libNativePlugin")]
public static class MyPlugin
{
    public delegate int Sum(int a, int b);
    [PluginFunctionAttr("native_add")] public static Sum native_add = null;
}

```

这样就定义好了一个Plugin里的几个Functiion类

然后在运行时，需要调用` NativePluginLoader.LoadAll(typeof(MyPlugin)mpluginPath);` 其中`typeof(MyPlugin)`为上述封装的Plugin类，`pluginPath`为Plugin的文件夹路径

随后调用函数时就可以：

```C#
NativePluginLoader.LoadAll(typeof(MyPlugin), pluginPath);
Debug.Log(MyPlugin.native_add(1, 2));
```



需要更换Plugin时，需要调用卸载方法，最好是在用完之后直接调用卸载

```C#
NativePluginLoader.UnloadAll();
```



调用完卸载代码就可以随意更换DLL了 :)



## Reference

[How to Reload Native Plugins in Unity](https://www.forrestthewoods.com/blog/how-to-reload-native-plugins-in-unity/)
