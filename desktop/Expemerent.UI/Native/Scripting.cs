using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Reflection;

namespace Expemerent.UI.Native
{
    #region Native data types for scripting support
    /// <summary>
    /// Native representation of the scripting method
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct SciterNativeMethodDef
    {
        /// <summary>
        /// Scripting delegate
        /// </summary>
        internal delegate void SciterNativeMethod_t(IntPtr hvm, ref JsonValue self, IntPtr argv, int argc, /*out*/ ref JsonValue retval);

        /// <summary>
        /// Precalculated size of structure
        /// </summary>
        public static readonly int SizeOf = Marshal.SizeOf(typeof(SciterNativeMethodDef));

        [MarshalAs(UnmanagedType.LPStr)]
        public string name;
        [MarshalAs(UnmanagedType.FunctionPtr)]
        public SciterNativeMethod_t method;
    }

    /// <summary>
    /// Native representation of the scripting property
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct SciterNativePropertyDef
    {
        /// <summary>
        /// Scripting delegate
        /// </summary>
        internal delegate void SciterNativeProperty_t(IntPtr hvm, ref JsonValue p_data_slot, bool set, ref JsonValue val);

        /// <summary>
        /// Precalculated size of structure
        /// </summary>
        public static readonly int SizeOf = Marshal.SizeOf(typeof(SciterNativePropertyDef));

        [MarshalAs(UnmanagedType.LPStr)]
        public string name;
        [MarshalAs(UnmanagedType.FunctionPtr)]
        public SciterNativeProperty_t property;
    }

    /// <summary>
    /// Native representation of the scripting class
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal class SciterNativeClassDef
    {
        public static readonly int SizeOf = Marshal.SizeOf(typeof(SciterNativeClassDef));

        public IntPtr name;
        public IntPtr methods;
        public IntPtr properties;
        public IntPtr dtor;
    }

    /// <summary>
    /// Scripting class definition 
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct SciterClassDef
    {
        /// <summary>
        /// Scripting delegate
        /// </summary>
        internal delegate void SciterNativeDtor_t(IntPtr hvm, IntPtr p_data_slot);

        public string name;
        public SciterNativeMethodDef[] methods;
        public SciterNativePropertyDef[] properties;
        public SciterNativeDtor_t dtor;

        #region Custom marshalling
        /// <summary>
        /// Prepares <see cref="SciterClassDef"/> for marshalling
        /// </summary>
        public SciterNativeClassDef ToNative()
        {
            var sciterNative = new SciterNativeClassDef();
            sciterNative.name = Marshal.StringToCoTaskMemAnsi(name);
            sciterNative.dtor = Marshal.GetFunctionPointerForDelegate(dtor);

            sciterNative.methods = Marshal.AllocCoTaskMem(SciterNativeMethodDef.SizeOf * methods.Length + 1);
            for (var i = 0; i < methods.Length; ++i)
                Marshal.StructureToPtr(methods[i], new IntPtr(sciterNative.methods.ToInt64() + i * SciterNativeMethodDef.SizeOf), false);

            Marshal.StructureToPtr(new SciterNativeMethodDef(), new IntPtr(sciterNative.methods.ToInt64() + methods.Length * SciterNativeMethodDef.SizeOf), false);

            sciterNative.properties = Marshal.AllocCoTaskMem(SciterNativePropertyDef.SizeOf * properties.Length + 1);
            for (var i = 0; i < properties.Length; ++i)
                Marshal.StructureToPtr(properties[i], new IntPtr(sciterNative.properties.ToInt64() + i * properties.Length), false);

            Marshal.StructureToPtr(new SciterNativePropertyDef(), new IntPtr(sciterNative.properties.ToInt64() + properties.Length * SciterNativePropertyDef.SizeOf), false);

            return sciterNative;
        }

        /// <summary>
        /// Releases resources from <see cref="SciterNativeClassDef"/>
        /// </summary>
        public static void FreeNative(SciterNativeClassDef nativeClass)
        {
            Marshal.FreeCoTaskMem(nativeClass.name);

            var loc = nativeClass.methods;
            while (Marshal.ReadIntPtr(loc) != IntPtr.Zero)
            {
                Marshal.StructureToPtr(new SciterNativeMethodDef(), loc, true);
                loc = new IntPtr(loc.ToInt64() + SciterNativeMethodDef.SizeOf);
            }

            loc = nativeClass.properties;
            while (Marshal.ReadIntPtr(loc) != IntPtr.Zero)
            {
                Marshal.StructureToPtr(new SciterNativePropertyDef(), loc, true);
                loc = new IntPtr(loc.ToInt64() + SciterNativePropertyDef.SizeOf);
            }

            Marshal.FreeCoTaskMem(nativeClass.methods);
            Marshal.FreeCoTaskMem(nativeClass.properties);
        }
        #endregion
    } 
    #endregion

    /// <summary>
    /// Interface to the scripting VM
    /// </summary>
    internal class Scripting
    {
        #region ScriptingClasses: Collection of classes and instances associated with the VM
        /// <summary>
        /// Collection of classes and instances associated with the VM
        /// </summary>
        private struct ScriptingClasses
        {
            /// <summary>
            /// Collection of registered classes
            /// </summary>
            public List<SciterClassDef> ClassDefs;

            /// <summary>
            /// DataSlot to Object mapping
            /// </summary>
            public Dictionary<IntPtr, object> Instances;

            /// <summary>
            /// Locates class definition by its name
            /// </summary>
            public bool IsExists(SciterClassDef classDef)
            {
                return ClassDefs.FindIndex(c => c.name == classDef.name) >= 0;
            }
        } 
        #endregion

        #region Private data
        /// <summary>
        /// Synchronizes access from the different threads
        /// </summary>
        private static readonly object _syncRoot = new object();

        /// <summary>
        /// HVM to registration mapping
        /// </summary>
        private static readonly Dictionary<IntPtr, ScriptingClasses> _registrations = new Dictionary<IntPtr, ScriptingClasses>(); 
        #endregion

        #region Public interface
        /// <summary>
        /// Registers scripting class
        /// </summary>
        public static bool RegisterClass<TType>(SciterView view)
        {
            Debug.Assert(view != null && view.HandleInternal != IntPtr.Zero, "View cannot be null");

            var result = false;
            var hvm = SciterHostApi.SciterGetVM(view.HandleInternal);
            if (hvm == IntPtr.Zero)
            {
                var scripting = GetScriptingClasses(view, hvm);

                var sciterClassDef = GetClassDef(typeof(TType));
                if (!scripting.IsExists(sciterClassDef))
                {
                    var nativeClass = sciterClassDef.ToNative();
                    try
                    {
                        result = SciterHostApi.SciterNativeDefineClass(hvm, nativeClass);
                        scripting.ClassDefs.Add(sciterClassDef);
                    }
                    finally { SciterClassDef.FreeNative(nativeClass); }
                }
                else
                    throw new ArgumentException("Class already registered", "TType");
            }

            return result;
        } 
        #endregion

        #region Scripting: Extracting reflection information
        /// <summary>
        /// Returns class definition by specified type
        /// </summary>
        private static SciterClassDef GetClassDef(Type type)
        {
            var scriptingClass = GetScriptingClass(type);
            var classDef = new SciterClassDef()
            {
                name = scriptingClass.Name,
                methods = new List<SciterNativeMethodDef>(GetMethods(type, scriptingClass.IsStatic)).ToArray(),
                properties = new List<SciterNativePropertyDef>(GetProperties(type)).ToArray(),
                dtor = ScriptingDtor
            };

            return classDef;
        }

        /// <summary>
        /// Returns collection of scripting properties
        /// </summary>
        private static IEnumerable<SciterNativePropertyDef> GetProperties(Type type)
        {
            foreach (var item in type.GetProperties())
            {
                var propertyInfo = item;
                var propertyAttr = (ScriptingPropertyAttribute)Attribute.GetCustomAttribute(propertyInfo, typeof(ScriptingPropertyAttribute));
                if (propertyAttr != null)
                    yield return DefineScriptingProperty(propertyInfo, propertyAttr); ;
            }
        }

        /// <summary>
        /// Returns collection of scripting methods
        /// </summary>
        private static IEnumerable<SciterNativeMethodDef> GetMethods(Type type, bool createStatic)
        {
            if (!createStatic)
                yield return DefineCtorMethod(type);

            foreach (var item in type.GetMethods())
            {
                var methodAttr = (ScriptingMethodAttribute)Attribute.GetCustomAttribute(item, typeof(ScriptingMethodAttribute));
                if (methodAttr != null)
                    yield return DefineScriptingMethod(item, methodAttr); ;
            }
        }

        /// <summary>
        /// Returns scripting class attribute
        /// </summary>
        private static ScriptingClassAttribute GetScriptingClass(Type type)
        {
            var scriptingClass = (ScriptingClassAttribute)Attribute.GetCustomAttribute(type, typeof(ScriptingClassAttribute));
            scriptingClass.Name = String.IsNullOrEmpty(scriptingClass.Name) ? type.Name : scriptingClass.Name;

            return scriptingClass;
        }
        #endregion

        #region Scripting: Defining scripting methods and properties
        /// <summary>
        /// Creates a scripting property definitions
        /// </summary>        
        private static SciterNativePropertyDef DefineScriptingProperty(PropertyInfo propertyInfo, ScriptingPropertyAttribute propertyAttr)
        {
            return new SciterNativePropertyDef()
            {
                name = propertyAttr.Name ?? propertyInfo.Name,

                // Property callback implementation
                property = (IntPtr hvm, ref JsonValue p_data_slot, bool set, ref JsonValue retval) =>
                {
                    try
                    {
                        var instance = default(object);
                        if (!propertyInfo.GetGetMethod().IsStatic)
                            instance = InstanceProtector.GetInstance(p_data_slot.GetNativeObject());

                        retval = new JsonValue();
                        if (set)
                            propertyInfo.SetValue(instance, retval.GetValue(), null);
                        else
                            retval = new JsonValue(propertyInfo.GetValue(instance, null));
                    }
                    catch (Exception ex)
                    {
                        SciterHostApi.SciterNativeThrow(hvm, ex.Message);
                    }
                }
            };
        }

        /// <summary>
        /// Creates a scripting ctor definition from the reflection info
        /// </summary>
        private static SciterNativeMethodDef DefineCtorMethod(Type type)
        {
            return new SciterNativeMethodDef()
            {
                name = "this",

                // Construction callback implementation
                method = (IntPtr hvm, ref JsonValue p_data_slot, IntPtr argv, int argc, ref JsonValue retval) =>
                {
                    try
                    {
                        var result = Activator.CreateInstance(type, JsonPtrToArray(argv, argc));
                        var data_slot_value = InstanceProtector.Protect(result);

                        p_data_slot.SetNativeObject(data_slot_value);
                        _registrations[hvm].Instances.Add(data_slot_value, result);
                    }
                    catch (Exception ex)
                    {
                        SciterHostApi.SciterNativeThrow(hvm, ex.Message);
                    }
                }
            };
        }

        /// <summary>
        /// Creates a scripting method definition from the reflection info
        /// </summary>
        private static SciterNativeMethodDef DefineScriptingMethod(MethodInfo methodInfo, ScriptingMethodAttribute methodAttr)
        {
            return new SciterNativeMethodDef()
            {
                name = methodAttr.Name ?? methodInfo.Name,

                // Method callback implementation
                method = (IntPtr hvm, ref JsonValue p_data_slot, IntPtr argv, int argc, ref JsonValue retval) =>
                {
                    try
                    {
                        var instance = default(object);
                        if (!methodInfo.IsStatic)
                            instance = InstanceProtector.GetInstance(p_data_slot.GetNativeObject());

                        var result = methodInfo.Invoke(instance, JsonPtrToArray(argv, argc));
                        retval = result == null ? new JsonValue() : new JsonValue(result);
                    }
                    catch (Exception ex)
                    {
                        SciterHostApi.SciterNativeThrow(hvm, ex.Message);
                    }
                }
            };
        } 
        #endregion

        #region Private implementation
        /// <summary>
        /// Returns scripting information assosiated with VM
        /// </summary>
        private static ScriptingClasses GetScriptingClasses(SciterView view, IntPtr hvm)
        {
            var scripting = default(ScriptingClasses);

            lock (_syncRoot)
            {
                if (!_registrations.TryGetValue(hvm, out scripting))
                {
                    scripting.ClassDefs = new List<SciterClassDef>();
                    scripting.Instances = new Dictionary<IntPtr, object>();

                    _registrations.Add(hvm, scripting);
                    view.Hook.HandleDestroyed += (s, e) =>
                    {
                        lock (_syncRoot)
                        {
                            Debug.Assert(_registrations[hvm].Instances.Count == 0, "all scripting instances should be freed at this point");
                            _registrations.Remove(hvm);
                        }
                    };
                }
            }

            return scripting;
        }

        /// <summary>
        /// Converts unmanaged pointer to json objects to object[]
        /// </summary>
        private unsafe static object[] JsonPtrToArray(IntPtr argv, int argc)
        {
            JsonValue* pJsonValue = (JsonValue*)argv;
            object[] res = new object[argc];

            for (var i = 0; i < argc; ++i)
                res[i] = (pJsonValue + i)->GetValue();

            return res;
        }

        /// <summary>
        /// Releases resources associated with class instance
        /// </summary>
        private static void ScriptingDtor(IntPtr hvm, IntPtr p_data_slot)
        {
            var p_data_slot_value = Marshal.ReadIntPtr(p_data_slot);
            _registrations[hvm].Instances.Remove(p_data_slot_value);

            var instance = InstanceProtector.GetInstance(p_data_slot_value) as IDisposable;
            if (instance != null)
                instance.Dispose();
        } 
        #endregion
    }
}