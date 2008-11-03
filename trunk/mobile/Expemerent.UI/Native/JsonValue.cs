using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Collections;
using System.Security;

namespace Expemerent.UI.Native
{
    internal enum VALUE_TYPE
    {
        T_UNDEFINED = 0,
        T_NULL,
        T_BOOL,
        T_INT,
        T_FLOAT,
        T_STRING,
        T_DATE,     // INT64 - contains a 64-bit value representing the number of 100-nanosecond intervals since January 1, 1601 (UTC), a.k.a. FILETIME on Windows
        T_CURRENCY, // INT64 - 14.4 fixed number. E.g. dollars = int64 / 10000; 
        T_LENGTH,   // length units, value is int or float, units are VALUE_UNIT_TYPE
        T_ARRAY,
        T_MAP,
        T_FUNCTION,
        T_BYTES,    // sequence of bytes - e.g. image data
        T_OBJECT,   // scripting object proxy (TISCRIPT/SCITER)
    }

    internal enum VALUE_UNIT_TYPE
    {
        UT_EM = 1, //height of the element's font. 
        UT_EX = 2, //height of letter 'x' 
        UT_PR = 3, //%
        UT_SP = 4, //%% "springs", a.k.a. flex units
        reserved1 = 5,
        reserved2 = 6,
        UT_PX = 7, //pixels
        UT_IN = 8, //inches (1 inch = 2.54 centimeters). 
        UT_CM = 9, //centimeters. 
        UT_MM = 10, //millimeters. 
        UT_PT = 11, //points (1 point = 1/72 inches). 
        UT_PC = 12, //picas (1 pica = 12 points). 
        reserved3 = 13,
        UT_COLOR = 14, // color in int
        UT_URL = 15,  // url in string
        UT_SYMBOL = 0xFFFF, // for T_STRINGs designates symbol string ( so called NAME_TOKEN - CSS or JS identifier )
    }

    internal enum VALUE_UNIT_TYPE_DATE
    {
        DT_HAS_DATE = 0x01, // date contains date portion
        DT_HAS_TIME = 0x02, // date contains time portion HH:MM
        DT_HAS_SECONDS = 0x04, // date contains time and seconds HH:MM:SS
        DT_UTC = 0x10, // T_DATE is known to be UTC. Otherwise it is local date/time
    }

    /// <summary>
    /// Sciter or TIScript specific
    /// </summary>
    internal enum VALUE_UNIT_TYPE_OBJECT
    {
        UT_OBJECT_ARRAY = 0,   // type T_OBJECT of type Array
        UT_OBJECT_OBJECT = 1,   // type T_OBJECT of type Object
        UT_OBJECT_CLASS = 2,   // type T_OBJECT of type Type (class or namespace)
        UT_OBJECT_NATIVE = 3,   // type T_OBJECT of native Type with data slot (LPVOID)
        UT_OBJECT_FUNCTION = 4, // type T_OBJECT of type Function
        UT_OBJECT_ERROR = 5,    // type T_OBJECT of type Error
    }

    internal enum VALUE_STRING_CVT_TYPE
    {
        CVT_SIMPLE,       ///< simple conversion of terminal values 
        CVT_JSON_LITERAL, ///< json literal parsing/emission 
        CVT_JSON_MAP,     ///< json parsing/emission, it parses as if token '{' already recognized 
    }

    internal struct JsonValue
    {
        #region Native methods
        /// <summary>
        /// Native methods declarations
        /// </summary>
        private static class NativeMethods
        {
            /// <summary>
            /// ValueInit - initialize VALUE storage
            /// This call has to be made before passing ref JsonValue to any other functions
            /// </summary>
            [DllImport(SciterHostApi.SciterDll)]
            public static extern uint ValueInit(ref JsonValue pval);

            /// <summary>
            /// ValueClear - clears the VALUE and deallocates all assosiated structures that are not used anywhere else.
            /// </summary>
            [DllImport(SciterHostApi.SciterDll)]
            public static extern uint ValueClear(ref JsonValue pval);

            /// <summary>
            /// ValueCompare - compares two values, returns HV_OK_TRUE if val1 == val2.
            /// </summary>
            [DllImport(SciterHostApi.SciterDll)]
            public static extern uint ValueCompare(ref JsonValue pval1, ref JsonValue pval2);

            /// <summary>
            /// ValueCopy - copies src VALUE to dst VALUE. dst VALUE must be in ValueInit state.
            /// </summary>
            [DllImport(SciterHostApi.SciterDll)]
            public static extern uint ValueCopy(ref JsonValue pdst, ref JsonValue psrc);

            /// <summary>
            /// ValueType - returns VALUE_TYPE and VALUE_UNIT_TYPE flags of the VALUE
            /// </summary>
            [DllImport(SciterHostApi.SciterDll)]
            public static extern uint ValueType(ref JsonValue pval, out VALUE_TYPE pType, out int pUnits);

            /// <summary>
            /// ValueStringData - returns string data for T_STRING type
            /// For T_FUNCTION returns name of the fuction. 
            /// </summary>
            [DllImport(SciterHostApi.SciterDll)]
            public static extern uint ValueStringData(ref JsonValue pval, out IntPtr pChars, out int pNumChars);

            /// <summary>
            /// ValueStringDataSet - sets VALUE to T_STRING type and copies chars/numChars to
            /// internal refcounted buffer assosiated with the value. 
            /// </summary>
            [DllImport(SciterHostApi.SciterDll)]
            public static extern uint ValueStringDataSet(ref JsonValue pval, [MarshalAs(UnmanagedType.LPWStr)]String chars, int numChars, VALUE_UNIT_TYPE units);

            /// <summary>
            /// ValueIntData - retreive integer data of T_INT, T_LENGTH and T_BOOL types
            /// </summary>
            [DllImport(SciterHostApi.SciterDll)]
            public static extern uint ValueIntData(ref JsonValue pval, out int pData);

            /// <summary>
            /// ValueIntDataSet - sets VALUE integer data of T_INT and T_BOOL types 
            /// Optionally sets units field too.
            /// </summary>
            [DllImport(SciterHostApi.SciterDll)]
            public static extern uint ValueIntDataSet(ref JsonValue pval, int data, VALUE_TYPE type, int units);

            /// <summary>
            /// ValueInt64Data - retreive 64bit integer data of T_CURRENCY and T_DATE values.
            /// </summary>
            [DllImport(SciterHostApi.SciterDll)]
            public static extern uint ValueInt64Data(ref JsonValue pval, out long pData);

            /// <summary>
            /// ValueInt64DataSet - sets 64bit integer data of T_CURRENCY and T_DATE values.
            /// </summary>
            [DllImport(SciterHostApi.SciterDll)]
            public static extern uint ValueInt64DataSet(ref JsonValue pval, long data, VALUE_TYPE type, int units);

            /// <summary>
            /// ValueFloatData - retreive FLOAT_VALUE (double) data of T_FLOAT and T_LENGTH values.
            /// </summary>
            [DllImport(SciterHostApi.SciterDll)]
            public static extern uint ValueFloatData(ref JsonValue pval, out double pData);

            /// <summary>
            /// ValueFloatDataSet - sets FLOAT_VALUE data of T_FLOAT and T_LENGTH values.
            /// </summary>
            [DllImport(SciterHostApi.SciterDll)]
            public static extern uint ValueFloatDataSet(ref JsonValue pval, double data, VALUE_TYPE type, int units);

            /// <summary>
            /// ValueBinaryData - retreive integer data of T_BYTES type
            /// </summary>
            [DllImport(SciterHostApi.SciterDll)]
            public static extern uint ValueBinaryData(ref JsonValue pval, out IntPtr pBytes, out int pnBytes);

            /// <summary>
            /// ValueBinaryDataSet - sets VALUE to sequence of bytes of type T_BYTES 
            /// 'type' here must be set to T_BYTES. Optionally sets units field too.
            /// The function creates local copy of bytes in its own storage.
            /// </summary>
            [DllImport(SciterHostApi.SciterDll)]
            public static extern uint ValueBinaryDataSet(ref JsonValue pval, byte[] pBytes, int nBytes, VALUE_TYPE type, int units);

            /// <summary>
            /// ValueBinaryDataSet - sets VALUE to sequence of bytes of type T_BYTES 
            /// 'type' here must be set to T_BYTES. Optionally sets units field too.
            /// The function creates local copy of bytes in its own storage.
            /// </summary>
            [DllImport(SciterHostApi.SciterDll)]
            public static extern uint ValueBinaryDataSet(ref JsonValue pval, IntPtr pBytes, int nBytes, VALUE_TYPE type, int units);

            /// <summary>
            /// ValueElementsCount - retreive number of sub-elements for:
            /// - T_ARRAY - number of elements in the array; 
            /// - T_MAP - number of key/value pairs in the map;
            /// - T_FUNCTION - number of arguments in the function;
            /// </summary>
            [DllImport(SciterHostApi.SciterDll)]
            public static extern uint ValueElementsCount(ref JsonValue pval, out int pn);

            /// <summary>
            /// ValueNthElementValue - retreive value of sub-element at index n for:
            /// - T_ARRAY - nth element of the array; 
            /// - T_MAP - value of nth key/value pair in the map;
            /// - T_FUNCTION - value of nth argument of the function;
            /// </summary>
            [DllImport(SciterHostApi.SciterDll)]
            public static extern uint ValueNthElementValue(ref JsonValue pval, int n, ref JsonValue pretval);

            /// <summary>
            /// ValueNthElementValueSet - sets value of sub-element at index n for:
            /// - T_ARRAY - nth element of the array; 
            /// - T_MAP - value of nth key/value pair in the map;
            /// - T_FUNCTION - value of nth argument of the function;
            /// If the VALUE is not of one of types above then it makes it of type T_ARRAY with 
            /// single element - 'val_to_set'.
            /// </summary>
            [DllImport(SciterHostApi.SciterDll)]
            public static extern uint ValueNthElementValueSet(ref JsonValue pval, int n, ref JsonValue pval_to_set);

            /// <summary>Callback function used with #ValueEnumElements().
            /// return TRUE to continue enumeration
            /// </summary>
            public delegate uint KeyValueCallback(IntPtr param, ref JsonValue pkey, ref JsonValue pval);

            /// <summary>
            /// ValueEnumElements - enumeartes key/value pairs of T_MAP, T_FUNCTION and T_OBJECT values
            /// - T_MAP - key of nth key/value pair in the map;
            /// - T_FUNCTION - name of nth argument of the function (if any);
            /// </summary>
            [DllImport(SciterHostApi.SciterDll)]
            public static extern uint ValueNthElementKey(ref JsonValue pval, int n, ref JsonValue pretval);

            [DllImport(SciterHostApi.SciterDll)]
            public static extern uint ValueEnumElements(ref JsonValue pval, KeyValueCallback penum, IntPtr param);

            /// <summary>
            /// ValueSetValueToKey - sets value of sub-element by key:
            /// - T_MAP - value of key/value pair with the key;
            /// - T_FUNCTION - value of argument with the name key;
            /// - T_OBJECT (tiscript) - value of property of the object
            /// If the VALUE is not of one of types above then it makes it of type T_MAP with 
            /// single pair - 'key'/'val_to_set'.
            ///
            /// key usually is a value of type T_STRING
            ///
            /// </summary>
            [DllImport(SciterHostApi.SciterDll)]
            public static extern uint ValueSetValueToKey(ref JsonValue pval, ref JsonValue pkey, ref JsonValue pval_to_set);

            /// <summary>
            /// ValueGetValueOfKey - retrieves value of sub-element by key:
            /// - T_MAP - value of key/value pair with the key;
            /// - T_FUNCTION - value of argument with the name key;
            /// - T_OBJECT (tiscript) - value of property of the object
            /// Otherwise *pretval will have T_UNDEFINED value.
            /// </summary>
            [DllImport(SciterHostApi.SciterDll)]
            public static extern uint ValueGetValueOfKey(ref JsonValue pval, ref JsonValue pkey, ref JsonValue pretval);

            /// <summary>
            /// ValueToString - converts value to T_STRING inplace:
            /// - CVT_SIMPLE - parse/emit terminal values (T_INT, T_FLOAT, T_LENGTH, T_STRING)
            /// - CVT_JSON_LITERAL - parse/emit value using JSON literal rules: {}, [], "string", true, false, null 
            /// - CVT_JSON_MAP - parse/emit MAP value without enclosing '{' and '}' brackets.
            /// </summary>
            [DllImport(SciterHostApi.SciterDll)]
            public static extern uint ValueToString(ref JsonValue pval, /*VALUE_STRING_CVT_TYPE*/ VALUE_STRING_CVT_TYPE how);

            /// <summary>
            /// ValueFromString - parses string into value:
            /// - CVT_SIMPLE - parse/emit terminal values (T_INT, T_FLOAT, T_LENGTH, T_STRING), "guess" non-strict parsing
            /// - CVT_JSON_LITERAL - parse/emit value using JSON literal rules: {}, [], "string", true, false, null 
            /// - CVT_JSON_MAP - parse/emit MAP value without enclosing '{' and '}' brackets.
            /// Returns:
            ///   Number of non-parsed characters in case of errors. Thus if string was parsed in full it returns 0 (success)  
            /// </summary>
            [DllImport(SciterHostApi.SciterDll)]
            public static extern uint ValueFromString(ref JsonValue pval, [MarshalAs(UnmanagedType.LPWStr)] string str, int strLength, /*VALUE_STRING_CVT_TYPE*/ VALUE_STRING_CVT_TYPE how);

            /// <summary>
            /// ValueInvoke - function invocation (Sciter/TIScript).
            /// - ref JsonValue pval is a value of type T_OBJECT/UT_OBJECT_FUNCTION
            /// - ref JsonValue pthis - object that will be known as 'this' inside that function.
            /// - UINT argc, ref JsonValue argv - vector of arguments to pass to the function. 
            /// - ref JsonValue pretval - parse/emit MAP value without enclosing '{' and '}' brackets.
            /// - LPCWSTR url - url or name of the script - used for error reporting in the script.
            /// Returns:
            ///   HV_OK, HV_BAD_PARAMETER or HV_INCOMPATIBLE_TYPE
            /// </summary>
            [DllImport(SciterHostApi.SciterDll)]
            public static extern uint ValueInvoke(ref JsonValue pval, ref JsonValue pthis, uint argc, ref JsonValue argv, ref JsonValue pretval, [MarshalAs(UnmanagedType.LPWStr)] string url);
        }
        #endregion

        #region Internal data
        /// <summary>
        /// Size of <see cref="JsonValue"/> structure
        /// </summary>
        internal const int SizeOf = 16;

        /// <summary>
        /// Type
        /// </summary>
        internal uint t;

        /// <summary>
        /// Unit
        /// </summary>
        internal uint u;

        /// <summary>
        /// Data
        /// </summary>
        internal ulong d; 
        #endregion

        public JsonValue(object data) : this()
        {
            NativeMethods.ValueClear(ref this);
            SetValue(data);
        }

        /// <summary>
        /// Gets LPVOID of the native object
        /// </summary>
        public IntPtr GetNativeObject()
        {
            var type = default(VALUE_TYPE);
            var unit = default(int);

            NativeMethods.ValueType(ref this, out type, out unit);
            if (type == VALUE_TYPE.T_OBJECT && unit == (int)VALUE_UNIT_TYPE_OBJECT.UT_OBJECT_NATIVE)
            {
                var pobj = default(IntPtr);
                var dummy = default(int);
                NativeMethods.ValueBinaryData(ref this, out pobj, out dummy);

                return pobj;
            }

            return IntPtr.Zero;
        }

        /// <summary>
        /// Sets LPVOID for the native object
        /// </summary>
        public void SetNativeObject(IntPtr ptr)
        {
            NativeMethods.ValueBinaryDataSet(ref this, ptr, 1, VALUE_TYPE.T_OBJECT, 0);
        }

        /// <summary>
        /// Returns a value from the <see cref="JsonValue"/> object
        /// </summary>
        public object GetValue()
        {
            var type = default(VALUE_TYPE);
            var unit = default(int);

            NativeMethods.ValueType(ref this, out type, out unit);
            switch (type)
            {
                case VALUE_TYPE.T_UNDEFINED:                    
                case VALUE_TYPE.T_NULL:
                    return null;
                case VALUE_TYPE.T_FLOAT:
                case VALUE_TYPE.T_LENGTH:
                    var fval = default(double);
                    NativeMethods.ValueFloatData(ref this, out fval);
                    return fval;
                case VALUE_TYPE.T_INT:
                case VALUE_TYPE.T_BOOL:
                    var ival = default(int);
                    NativeMethods.ValueIntData(ref this, out ival);
                    return type == VALUE_TYPE.T_BOOL ? (object) (ival != 0) : (object)ival;
                case VALUE_TYPE.T_STRING:
                    var sval = default(IntPtr);
                    var nchars = default(int);
                    NativeMethods.ValueStringData(ref this, out sval, out nchars);
                    return Marshal.PtrToStringUni(sval, nchars);
                case VALUE_TYPE.T_CURRENCY:
                    var ldata = default(long);
                    NativeMethods.ValueInt64Data(ref this, out ldata);

                    return new Decimal(ldata);
                case VALUE_TYPE.T_DATE:
                    var ddata = default(long);
                    NativeMethods.ValueInt64Data(ref this, out ldata);

                    if ((unit & (int)VALUE_UNIT_TYPE_DATE.DT_UTC) == 0)
                        return DateTime.FromFileTime(ddata);
                    else
                        return DateTime.FromFileTimeUtc(ddata);
                case VALUE_TYPE.T_ARRAY:
                case VALUE_TYPE.T_MAP:
                case VALUE_TYPE.T_OBJECT:
                    switch ((VALUE_UNIT_TYPE_OBJECT)unit)
                    {
                        case VALUE_UNIT_TYPE_OBJECT.UT_OBJECT_ARRAY:
                            return GetArrayValue();
                        case VALUE_UNIT_TYPE_OBJECT.UT_OBJECT_OBJECT:
                            return GetDictValue();
                    }
                    break;
                case VALUE_TYPE.T_FUNCTION:
                    break;
                case VALUE_TYPE.T_BYTES:
                    var pbytes = default(IntPtr);
                    var cbytes = default(int);
                    NativeMethods.ValueBinaryData(ref this, out pbytes, out cbytes);
                    var bytes = new byte[cbytes];
                    Marshal.Copy(pbytes, bytes, 0, cbytes);
                    return bytes;
                default:
                    break;
            }

            return null;
        }

        /// <summary>
        ///  clears the VALUE and deallocates all assosiated structures that are not used anywhere else.
        /// </summary>
        public void Clear()
        {
            NativeMethods.ValueClear(ref this);
        }

        /// <summary>
        /// Sets a new value to json object
        /// </summary>
        public void SetValue(object data)
        {
            if (data != null)
            {
                if (data is string)
                {
                    var str = (string)data;
                    NativeMethods.ValueStringDataSet(ref this, str, str.Length, 0);
                }
                else if (data is int)
                {
                    var ival = (int)data;
                    NativeMethods.ValueIntDataSet(ref this, ival, VALUE_TYPE.T_INT, 0);
                }
                else if (data is double)
                {
                    var dval = (double)data;
                    NativeMethods.ValueFloatDataSet(ref this, dval, VALUE_TYPE.T_FLOAT, 0);
                }
                else if (data is bool)
                {
                    var bval = (bool)data;
                    NativeMethods.ValueIntDataSet(ref this, bval ? 1 : 0, VALUE_TYPE.T_BOOL, 0);
                }
                else if (data is byte[])
                {
                    var bytes = (byte[])data;
                    NativeMethods.ValueBinaryDataSet(ref this, bytes, bytes.Length, VALUE_TYPE.T_BYTES, 0);
                }
                else if (data is decimal)
                {
                    var ddata = (decimal)data;
                    NativeMethods.ValueInt64DataSet(ref this, Decimal.ToInt64(ddata), VALUE_TYPE.T_CURRENCY, 0);
                }
                else if (data is DateTime)
                {
                    var ddata = (DateTime)data;
                    if (ddata.Kind == DateTimeKind.Utc)
                        NativeMethods.ValueInt64DataSet(ref this, ddata.ToFileTimeUtc(), VALUE_TYPE.T_DATE, 7 + (int)VALUE_UNIT_TYPE_DATE.DT_UTC);
                    else
                        NativeMethods.ValueInt64DataSet(ref this, ddata.ToFileTime(), VALUE_TYPE.T_DATE, 7);
                }
                else if (data is IDictionary)
                    SetDictValue((IDictionary)data);
                else if (data is IList)
                    SetArrayValue((IList)data);
                else SetDictValue(MarshalUtility.ObjectToDict(data));
            }
            else
                Clear();
        }

        /// <summary>
        /// Populates json value with passed dict
        /// </summary>
        private void SetDictValue(IDictionary dict)
        {
            Clear();
            foreach (var item in dict.Keys)
            {
                var key = default(JsonValue);
                var val = default(JsonValue);

                try
                {
                    key.SetValue(item);
                    val.SetValue(dict[item]);
                    NativeMethods.ValueSetValueToKey(ref this, ref key, ref val);
                }
                finally
                {
                    key.Clear();
                    val.Clear();
                }
            }
        }

        /// <summary>
        /// Extracts dictionary from current <see cref="JsonValue"/> object
        /// </summary>
        private IDictionary GetDictValue()
        {
            var dict = new Dictionary<object, object>();
            var handler = (NativeMethods.KeyValueCallback)delegate(IntPtr param, ref JsonValue pkey, ref JsonValue pval)
            {
                dict[pkey.GetValue()] = pval.GetValue();
                return 1;
            };
            NativeMethods.ValueEnumElements(ref this, handler, IntPtr.Zero);
            return dict;
        }

        /// <summary>
        /// Extracts array from current <see cref="JsonValue"/> object
        /// </summary>
        private IList GetArrayValue()
        {
            var cnt = default(int);
            NativeMethods.ValueElementsCount(ref this, out cnt);
            
            var items = new object[cnt];
            for (int i = 0; i < cnt; i++)
            {
                var res = default(JsonValue);
                try
                {
                    NativeMethods.ValueNthElementValue(ref this, i, ref res);
                    items[i] = res.GetValue();
                }
                finally { res.Clear(); }
            }

            return items;
        }

        /// <summary>
        /// Populates json value with values from passed array
        /// </summary>
        private void SetArrayValue(IList data)
        {
            Clear();
            for (int i = 0; i < data.Count; i++)
            {
                var item = default(JsonValue);
                try
                {
                    item.SetValue(data[i]);
                    NativeMethods.ValueNthElementValueSet(ref this, i, ref item);
                }
                finally { item.Clear(); }
            }
        }

        /// <summary>
        /// Converts array of <see cref="Object"/> to the array of <see cref="JsonValue"/>
        /// </summary>
        public static JsonValue[] CreateJsonArray(object[] args)
        {
            if (args != null)
            {
                var jsons = new JsonValue[args.Length];
                for (int i = 0; i < args.Length; i++)
                {
                    jsons[i] = new JsonValue(args[i]);
                }

                return jsons;
            }

            return null;
        }

        /// <summary>
        /// Releases allocated array
        /// </summary>
        public static void FreeJsonArray(JsonValue[] arr)
        {
            foreach (var val in arr)
                val.Clear();
        }
    }
}