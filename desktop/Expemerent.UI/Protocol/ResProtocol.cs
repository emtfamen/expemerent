using System.IO;
using System;

namespace Expemerent.UI.Protocol
{
    /// <summary>
    /// Handles loading data from the managed resources
    /// </summary>
    public class ResProtocol : IProtocolHandler
    {
        /// <summary>
        /// Prefix for the 
        /// </summary>
        public const string ProtocolPrefix = "res://";
        
        /// <summary>
        /// Path separator char
        /// </summary>
        public const char PathSeparator = '/';

        /// <summary>
        /// Used to check whether handler can deal with specified resource
        /// </summary>
        public bool Accept(String uri, ResourceType resourceType)
        {
            #region Parameters checking
            if (String.IsNullOrEmpty(uri))
                throw new ArgumentNullException("uri"); 
            #endregion

            return uri.StartsWith(ProtocolPrefix);
        }

        /// <summary>
        /// Loads specified resource
        /// </summary>
        public Stream Resolve(String uri, ResourceType resourceType)
        {
            #region Parameters checking
            if (String.IsNullOrEmpty(uri))
                throw new ArgumentNullException("uri");

            if (!Accept(uri, resourceType))
                throw new ArgumentException("uri", "Protocol not supported");
            #endregion

            var typeNameIndex = uri.IndexOf(PathSeparator, ProtocolPrefix.Length);
            if (typeNameIndex > 0)
            {
                var typeName = uri.Substring(ProtocolPrefix.Length, typeNameIndex - ProtocolPrefix.Length);
                var resourceName = uri.Substring(typeNameIndex).Replace(PathSeparator, '.');

                var type = Type.GetType(typeName);
                var fullName = type.Namespace + resourceName;
                return type.Assembly.GetManifestResourceStream(fullName);
            }
            else
                throw new ArgumentOutOfRangeException("uri", uri);
        }
    }
}
