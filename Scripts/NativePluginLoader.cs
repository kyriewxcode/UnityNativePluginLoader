using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEngine;

namespace GTC.BEngine
{
    static class SystemLibrary
    {
        [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern IntPtr LoadLibrary(string fileName);

        [DllImport("kernel32", SetLastError = true)]
        public static extern bool FreeLibrary(IntPtr module);

        [DllImport("kernel32")] public static extern IntPtr GetProcAddress(IntPtr module, string procName);

        [DllImport("kernel32.dll")] public static extern uint GetLastError();
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class PluginAttr : Attribute
    {
        public string pluginName { get; }

        public PluginAttr(string pluginName)
        {
            this.pluginName = pluginName;
        }
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class PluginFunctionAttr : Attribute
    {
        public string functionName { get; }

        public PluginFunctionAttr(string functionName)
        {
            this.functionName = functionName;
        }
    }

    [ExecuteAlways]
    public class NativePluginLoader
    {
        [DllImport("UnityInterface")] public static extern IntPtr GetUnityInterface();

        private static NativePluginLoader instance;

        const string EXT = ".dll"; // TODO: Handle different platforms
        private static Dictionary<string, IntPtr> m_LoadedPlugins = new();

        public static void LoadAll(Type type, string path)
        {
            var typeAttributes = type.GetCustomAttributes(typeof(PluginAttr), true);
            if (typeAttributes.Length > 0)
            {
                Debug.Assert(typeAttributes.Length == 1);

                var typeAttribute = typeAttributes[0] as PluginAttr;
                var pluginName = typeAttribute.pluginName + EXT;
                if (!m_LoadedPlugins.TryGetValue(pluginName, out IntPtr pluginHandle))
                {
                    var pluginPath = path + "/" + pluginName;
                    pluginHandle = SystemLibrary.LoadLibrary(pluginPath);
                    if (pluginHandle == IntPtr.Zero)
                        throw new Exception("Failed to load plugin [" + pluginPath + "]");

                    m_LoadedPlugins.Add(pluginName, pluginHandle);
                }

                var fields = type.GetFields(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
                foreach (var field in fields)
                {
                    var fieldAttributes = field.GetCustomAttributes(typeof(PluginFunctionAttr), true);
                    if (fieldAttributes.Length > 0)
                    {
                        Debug.Assert(fieldAttributes.Length == 1);

                        var fieldAttribute = fieldAttributes[0] as PluginFunctionAttr;
                        var functionName = fieldAttribute.functionName;

                        var fnPtr = SystemLibrary.GetProcAddress(pluginHandle, functionName);
                        if (fnPtr == IntPtr.Zero)
                        {
                            Debug.LogError($"Failed to find function [{functionName}] in plugin [{pluginName}]. Err: [{SystemLibrary.GetLastError()}]");
                            continue;
                        }
                        var fnDelegate = Marshal.GetDelegateForFunctionPointer(fnPtr, field.FieldType);
                        field.SetValue(null, fnDelegate);
                    }
                }
            }
        }

        public static void UnloadAll()
        {
            foreach (var plugin in m_LoadedPlugins)
            {
                SystemLibrary.FreeLibrary(plugin.Value);
            }
            m_LoadedPlugins.Clear();
        }
    }
}