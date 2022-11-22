using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace GTC.BEngine
{
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

    public class NativePluginLoader : IDisposable
    {
        [DllImport("UnityInterface")] public static extern IntPtr GetUnityInterface();

        private delegate void UnityPluginLoadDelegate(IntPtr unityInterfaces);
        private delegate void UnityPluginUnloadDelegate();

        const string EXT = ".dll"; // TODO: Handle different platforms
        private IntPtr m_DllPtr = IntPtr.Zero;
        private string m_DLLName;

        public NativePluginLoader(Type type, string path)
        {
            object[] typeAttributes = type.GetCustomAttributes(typeof(PluginAttr), true);
            if (typeAttributes.Length > 0)
            {
                Debug.Assert(typeAttributes.Length == 1);

                var typeAttribute = typeAttributes[0] as PluginAttr;
                m_DLLName = typeAttribute.pluginName + EXT;
                string pluginPath = path + "/" + m_DLLName;

                // Load Plugins
                m_DllPtr = LoadLibrary(pluginPath);
                if (m_DllPtr == IntPtr.Zero) throw new Exception("Failed to load plugin [" + pluginPath + "]");

                // Load Function
                var fields = type.GetFields(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
                foreach (var field in fields)
                {
                    object[] fieldAttributes = field.GetCustomAttributes(typeof(PluginFunctionAttr), true);
                    if (fieldAttributes.Length > 0)
                    {
                        Debug.Assert(fieldAttributes.Length == 1);

                        if (fieldAttributes[0] is PluginFunctionAttr fieldAttribute)
                        {
                            string functionName = fieldAttribute.functionName;

                            var fnPtr = GetProcAddress(m_DllPtr, functionName);
                            if (fnPtr == IntPtr.Zero)
                            {
                                Debug.LogError($"Failed to find function [{functionName}] in plugin [{m_DLLName}]. Err: [{GetLastError()}]");
                                continue;
                            }
                            var fnDelegate = Marshal.GetDelegateForFunctionPointer(fnPtr, field.FieldType);
                            field.SetValue(null, fnDelegate);
                        }
                    }
                }

                // Invoke UnityPluginLoad
                GetDelegate<UnityPluginLoadDelegate>("UnityPluginLoad")?.Invoke(GetUnityInterface());
            }
        }

        public void Dispose()
        {
            if (m_DllPtr != IntPtr.Zero)
            {
                GetDelegate<UnityPluginUnloadDelegate>("UnityPluginUnload")?.Invoke();
                FreeLibrary(m_DllPtr);
                m_DllPtr = IntPtr.Zero;
            }
            else
            {
                Debug.LogError(GetLastError());
            }
        }

        private TDelegate GetDelegate<TDelegate>(string funcName)
        {
            return m_DllPtr != IntPtr.Zero ? Marshal.GetDelegateForFunctionPointer<TDelegate>(GetProcAddress(m_DllPtr, funcName)) : default(TDelegate);
        }

        [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Unicode)] private static extern IntPtr LoadLibrary(string fileName);

        [DllImport("kernel32", SetLastError = true)] private static extern bool FreeLibrary(IntPtr module);

        [DllImport("kernel32")] private static extern IntPtr GetProcAddress(IntPtr module, string procName);

        [DllImport("kernel32.dll")] private static extern uint GetLastError();
    }
}