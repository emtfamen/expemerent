using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Collections;

namespace Expemerent.UI.Native
{
    /// <summary>
    /// Delegate to free allocated memory
    /// </summary>
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate void WipeCallback(IntPtr loc);

    [StructLayout(LayoutKind.Explicit, Size = SizeOf)]
    internal struct JsonValue
    {
        #region Private data
        /// <summary>
        /// SizeOf <see cref="JsonValue"/> structure
        /// </summary>
        public const int SizeOf = SizeOfInt + DataSlot.SizeOf; 

        /// <summary>
        /// Size of IntPtr
        /// </summary>
        private const int SizeOfIntPtr = 4;

        /// <summary>
        /// Size of IntPtr
        /// </summary>
        private const int SizeOfInt = 4;

        /// <summary>
        /// Cached delegate value to protect from GC
        /// </summary>
        private readonly static WipeCallback _wipe = Wipe;

        /// <summary>
        /// Unmanaged pointer to wipe func
        /// </summary>
        private readonly static IntPtr _wipePtr = Marshal.GetFunctionPointerForDelegate(_wipe);
        #endregion

        #region VALUETYPE enum
        /// <summary>
        /// Type of the value.
        /// </summary>
        internal enum VALUETYPE
        {
            V_UNDEFINED = 0,  ///< empty
            V_BOOL = 1,       ///< bool
            V_INT = 2,        ///< int
            V_REAL = 3,       ///< double
            V_STRING = 4,     ///< string of wchar_t
            V_ARRAY = 5,      ///< array of value elements
            V_MAP = 6,        ///< map of name/value pairs - simple map
            V_BYTES = 7       ///< vector of bytes, a.k.a. blob
        };
        #endregion

        #region Fields
        /// <summary>
        /// Type of containded value
        /// </summary>
        [FieldOffset(0)]
        private VALUETYPE v_type;

        /// <summary>
        /// Data block
        /// </summary>
        [FieldOffset(4)]
        private DataSlot data;
        #endregion

        #region Memory management

        /// <summary>
        /// Releases memory
        /// </summary>
        private static void Wipe(IntPtr addr)
        {
            Marshal.FreeCoTaskMem(addr);
        }

        /// <summary>
        /// Allocates memory
        /// </summary>
        private unsafe static IntPtr Allocate(int cb)
        {            
            var aligned = cb / SizeOfInt;
            aligned += 1;

            // allocating int aligned buffer
            var buffer = (int*)Marshal.AllocCoTaskMem(aligned * SizeOfInt);
            for (int i = 0; i < aligned; ++i)
                buffer[i] = 0;

            return new IntPtr(buffer);
        }

        /// <summary>
        /// Releases memory using specified wiper func
        /// </summary>
        private static void FreeMem(IntPtr wipe_func, IntPtr loc)
        {
            if (wipe_func != _wipePtr)
            {
                var wipe = MarshalUtility.GetWipeDelegate(wipe_func);
                wipe(loc);
            }
            else
                Wipe(loc);
        }
        #endregion

        #region Construction
        /// <summary>
        /// Creates a new instance of the <see cref="JsonValue"/> object
        /// </summary>
        public JsonValue(object val) : this()
        {
            var type = GetObjectType(val);
            
            if (type != VALUETYPE.V_UNDEFINED)
                SetValue(val);
        } 
        #endregion

        #region DataSlot structure
        [StructLayout(LayoutKind.Explicit, Size = 8)]
        internal struct DataSlot
        {
            /// <summary>
            /// SizeOf data_slot structure
            /// </summary>
            public const int SizeOf = 8;

            /// <summary>
            /// Bool value place holder
            /// </summary>
            [FieldOffset(0)]
            public bool b_val;

            /// <summary>
            /// Int value place holder
            /// </summary>
            [FieldOffset(0)]
            public int i_val;

            /// <summary>
            /// Double value placeholder
            /// </summary>
            [FieldOffset(0)]
            public double r_val;

            /// <summary>
            /// Long value place holder
            /// </summary>
            [FieldOffset(0)]
            public ulong l_val;

            /// <summary>
            /// String value place holder
            /// </summary>
            [FieldOffset(0)]
            public IntPtr s_val;

            /// <summary>
            /// Array place holder
            /// </summary>
            [FieldOffset(0)]
            public IntPtr a_val;

            /// <summary>
            /// Simple key/value pairs place holder
            /// </summary>
            [FieldOffset(0)]
            public IntPtr m_val;

            /// <summary>
            /// Simple blob place holder
            /// </summary>
            [FieldOffset(0)]
            public IntPtr pb_val;
        } 
        #endregion

        #region Public interface
        /// <summary>
        /// Converts array of objects to json array
        /// </summary>
        public static JsonValue[] CreateJsonArray(object[] args)
        {
            var jsonParams = new JsonValue[args.Length];

            for (var i = 0; i < args.Length; i++)
                jsonParams[i].SetValue(args[i]);

            return jsonParams;
        }

        /// <summary>
        /// Converts array of JsonObjects to array of Objects
        /// </summary>
        public static object[] CreateArray(JsonValue[] jsonParams)
        {
            var objs = new object[jsonParams.Length];

            for (var i = 0; i < jsonParams.Length; i++)
                objs[i] = jsonParams[i].GetValue();

            return objs;
        }

        /// <summary>
        /// Releases array of json objects
        /// </summary>
        public static void FreeJsonArray(JsonValue[] data)
        {
            for (var i = 0; i < data.Length; i++)
                data[i].Clear();
        }

        /// <summary>
        /// Clears all existing data
        /// </summary>
        public void Clear()
        {
            switch (v_type)
            {
                case VALUETYPE.V_UNDEFINED:
                case VALUETYPE.V_BOOL:
                case VALUETYPE.V_INT:
                case VALUETYPE.V_REAL:
                    break;

                case VALUETYPE.V_STRING:
                    StringData.Release(data.s_val);
                    break;
                case VALUETYPE.V_ARRAY:
                    ArrayData.Release(data.a_val);
                    break;
                case VALUETYPE.V_MAP:
                    NamedValue.Release(data.m_val);
                    break;
                case VALUETYPE.V_BYTES:
                    BytesData.Release(data.pb_val);
                    break;
                default:
                    Debug.Fail("Invalid enum value");
                    break;
            }

            v_type = VALUETYPE.V_UNDEFINED;
            data = new DataSlot();
        }

        /// <summary>
        /// Sets a new value to the <see cref="JsonValue"/> object
        /// </summary>
        public void SetValue(object source)
        {
            Clear();

            var type = GetObjectType(source);
            switch (type)
            {
                case VALUETYPE.V_UNDEFINED:
                    break;
                case VALUETYPE.V_BOOL:
                    data.b_val = (bool)source;
                    break;
                case VALUETYPE.V_INT:
                    data.i_val = (int)source;
                    break;
                case VALUETYPE.V_REAL:
                    data.r_val = (double)source;
                    break;
                case VALUETYPE.V_STRING:
                    data.s_val = StringData.ToPtr((string)source);
                    break;
                case VALUETYPE.V_ARRAY:
                    data.a_val = ArrayData.ToPtr((object[])source);
                    break;
                case VALUETYPE.V_MAP:
                    data.m_val = NamedValue.ToPtr((IDictionary)source);
                    break;
                case VALUETYPE.V_BYTES:
                    data.pb_val = BytesData.ToPtr((byte[])source);
                    break;
                default:
                    Debug.Fail(String.Format("Invalid enum value: {0}", v_type));
                    type = VALUETYPE.V_UNDEFINED;
                    break;
            }

            v_type = type;
        }

        /// <summary>
        /// Extracts value from the <see cref="JsonValue"/> instance
        /// </summary>
        public object GetValue()
        {
            switch (v_type)
            {
                case VALUETYPE.V_UNDEFINED:
                    return null;
                case VALUETYPE.V_BOOL:
                    return data.b_val;
                case VALUETYPE.V_INT:
                    return data.i_val;
                case VALUETYPE.V_REAL:
                    return data.r_val;
                case VALUETYPE.V_STRING:
                    return StringData.FromPtr(data.s_val);
                case VALUETYPE.V_ARRAY:
                    return ArrayData.FromPtr(data.a_val);
                case VALUETYPE.V_MAP:
                    return NamedValue.FromPtr(data.m_val);
                case VALUETYPE.V_BYTES:
                    return BytesData.FromPtr(data.pb_val);
                default:
                    Debug.Fail(String.Format("Invalid enum value: {0}", v_type));
                    break;
            }

            return null;
        } 
        #endregion

        #region Private implementation
        /// <summary>
        /// Gets a VALUETYPE from passed object
        /// </summary>
        private static VALUETYPE GetObjectType(object p)
        {
            var type = p == null ? null : p.GetType();
                
            if (type == null)
                return VALUETYPE.V_UNDEFINED;
            if (type == typeof(string))
                return VALUETYPE.V_STRING;
            if (type == typeof(double))
                return VALUETYPE.V_REAL;
            if (type == typeof(int))
                return VALUETYPE.V_INT;
            if (type == typeof(bool))
                return VALUETYPE.V_BOOL;
            if (type == typeof(byte[]))
                return VALUETYPE.V_BYTES;
            if (type == typeof(object[]))
                return VALUETYPE.V_ARRAY;

            return typeof(IDictionary).IsAssignableFrom(type) ? VALUETYPE.V_MAP : VALUETYPE.V_UNDEFINED;
        } 
        #endregion

        #region ArrayData implementation
        [StructLayout(LayoutKind.Sequential)]
        private unsafe struct ArrayData
        {
            private static readonly int SizeOf = Marshal.SizeOf(typeof(ArrayData));

            private int c_refs;
            private int allocated;
            private int length;
            private IntPtr wipe;

            /// <summary>
            /// Releases unmanaged array data
            /// </summary>
            /// <param name="loc"></param>
            public static void Release(IntPtr loc)
            {
                var data = (ArrayData*)loc.ToPointer();
                if (--data->c_refs > 0)
                    return;

                for (var i = 0; i < data->length; ++i)
                {
                    var val_loc = new IntPtr(loc.ToInt64() + SizeOf + i * JsonValue.SizeOf);
                    var json_value = (JsonValue)Marshal.PtrToStructure(val_loc, typeof(JsonValue));

                    json_value.Clear();
                }

                FreeMem(data->wipe, loc);
            }

            /// <summary>
            /// Marshals array from unamanaged data
            /// </summary>
            public static object[] FromPtr(IntPtr loc)
            {
                var data = (ArrayData*)loc.ToPointer();
                var res = new object[data->length];

                for (var i = 0; i < data->length; i++)
                {
                    var val_loc = new IntPtr(loc.ToInt64() + SizeOf + i * JsonValue.SizeOf);
                    var json_value = (JsonValue)Marshal.PtrToStructure(val_loc, typeof(JsonValue));

                    res[i] = json_value.GetValue();
                }

                return res;
            }

            /// <summary>
            /// Marshals array to unamanaged data
            /// </summary>
            public static IntPtr ToPtr(object[] source)
            {
                var loc = Allocate(SizeOf + source.Length * JsonValue.SizeOf);
                var data = (ArrayData*)loc.ToPointer();

                data->c_refs = 1;
                data->allocated = source.Length;
                data->length = source.Length;
                data->wipe = _wipePtr;

                for (var i = 0; i < data->length; i++)
                {
                    var val_loc = new IntPtr(loc.ToInt64() + SizeOf + i * JsonValue.SizeOf);
                    var json_value = new JsonValue(source[i]);

                    Marshal.StructureToPtr(json_value, val_loc, false);
                }

                return loc;
            }
        } 
        #endregion

        #region NamedValue implementation
        /// <summary>
        /// Map marshaller
        /// </summary>
        [StructLayout(LayoutKind.Explicit)]
        private unsafe struct NamedValue
        {
            public static readonly int SizeOf = Marshal.SizeOf(typeof(NamedValue));

            [FieldOffset(0 * JsonValue.SizeOf)]
            private JsonValue key;

            [FieldOffset(1 * JsonValue.SizeOf)]
            private  JsonValue value;
 
            [FieldOffset(2 * JsonValue.SizeOf)]
            private  IntPtr next;

            [FieldOffset(SizeOfIntPtr+ 2 * JsonValue.SizeOf)]
            private  IntPtr wipe;

            [FieldOffset(2 * SizeOfIntPtr + 2 * JsonValue.SizeOf)]
            private  int c_refs;

            /// <summary>
            /// Marshals <see cref="NamedValue"/> to the unmanaged buffer
            /// </summary>
            public static IntPtr ToPtr(IDictionary dict)
            {
                var head = IntPtr.Zero;
                NamedValue* current = null;
                var it = dict.GetEnumerator();
                
                while (it.MoveNext())
                {
                    var val = AllocateValue(it.Key, it.Value);
                    head = head == IntPtr.Zero ? new IntPtr(val) : head;

                    if (current != null)
                        current->next = new IntPtr(val);

                    current = val;
                }

                return head;
            }

            /// <summary>
            /// Unmarshals <see cref="NamedValue"/> from the unmanaged buffer
            /// </summary>
            public static IDictionary FromPtr(IntPtr loc)
            {
                var dict = new Dictionary<object, object>();
                var current = (NamedValue*)loc;
                while (current != null)
                {
                    var key = current->key.GetValue();
                    var value = current->value.GetValue();

                    if (key != null)
                        dict[key] = value;

                    current = (NamedValue*)current->next;
                }

                return dict;
            }

            /// <summary>
            /// Releases <see cref="NamedValue"/> unmanaged buffer
            /// </summary>
            public static void Release(IntPtr loc)
            {
                var current = (NamedValue*)loc;
                while (current != null)
                    current = ReleaseValue(current);
            }

            #region Private implementation
            /// <summary>
            /// Releases single value
            /// </summary>
            private static NamedValue* ReleaseValue(NamedValue* data)
            {
                var next = (NamedValue*)data->next;
                
                data->key.Clear();
                data->value.Clear();

                FreeMem(data->wipe, new IntPtr(data));

                return next;
            }

            /// <summary>
            /// Allocates single value
            /// </summary>
            private static NamedValue* AllocateValue(object key, object value)
            {
                var data = (NamedValue*)Allocate(SizeOf);
                data->c_refs = 1;
                data->key.SetValue(key);
                data->value.SetValue(value);
                data->wipe = _wipePtr;

                return data;
            } 
            #endregion
        } 
        #endregion

        #region BytesData implementation
        /// <summary>
        /// Byte buffer marshaller
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        private unsafe struct BytesData
        {
            private static readonly int SizeOf = Marshal.SizeOf(typeof(BytesData));

            private int c_refs;
            private IntPtr data;
            private int length;    // number of elements used in the buffer 
            private IntPtr wipe;
            
            /// <summary>
            /// Marshals byte[] array to unmanaged memory
            /// </summary>
            public static IntPtr ToPtr(byte[] source)
            {
                var data = (BytesData*)Allocate(SizeOf + source.Length);
                data->c_refs = 1;
                data->data = new IntPtr(data + SizeOf);
                data->length = source.Length;
                data->wipe = _wipePtr;

                Marshal.Copy(source, 0, data->data, source.Length);

                return new IntPtr(data);
            }

            /// <summary>
            /// Marshals byte[] array from unmanaged memory
            /// </summary>
            public static byte[] FromPtr(IntPtr loc)
            {
                var data = (BytesData*)loc;
                var res = new byte[data->length];

                Marshal.Copy(data->data, res, 0, res.Length);

                return res;
            }

            /// <summary>
            /// Releases byte[] array
            /// </summary>
            public static void Release(IntPtr loc)
            {
                var data = (BytesData*)loc;
                if (--data->c_refs <= 0)
                    FreeMem(data->wipe, loc);
            }
        } 
        #endregion

        #region StringData class
        /// <summary>
        /// String marhaller
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]        
        private unsafe struct StringData
        {
            private const int SizeOfChar = 2;
            private static readonly int SizeOf = Marshal.SizeOf(typeof(StringData));

            private int c_refs;
            private int length;
            private IntPtr wipe;

            /// <summary>
            /// Marshals string to the memory block
            /// </summary>
            public static IntPtr ToPtr(String str)
            {
                var strData = Encoding.Unicode.GetBytes(str);
                var loc = Allocate(SizeOf + strData.Length + SizeOfChar);
                var data = (StringData*)loc;
                {
                    data->c_refs = 1;
                    data->length = str.Length;
                    data->wipe = _wipePtr;
                    
                    Marshal.Copy(strData, 0, new IntPtr(loc.ToInt64() + SizeOf), strData.Length);
                }

                return loc;
            }

            /// <summary>
            /// Unmarshals string from memory block
            /// </summary>
            public static String FromPtr(IntPtr loc)
            {
                return Marshal.PtrToStringUni(new IntPtr(loc.ToInt64() + SizeOf));
            }

            /// <summary>
            /// Releases string buffer
            /// </summary>
            public static void Release(IntPtr loc)
            {
                var data = (StringData*)loc;
                if (--data->c_refs <= 0)
                    FreeMem(data->wipe, loc);
            }
        }
        #endregion
    }
}