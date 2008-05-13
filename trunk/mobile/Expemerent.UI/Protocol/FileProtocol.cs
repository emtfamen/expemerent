using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Expemerent.UI.Protocol
{
    public class FileProtocol : IProtocolHandler
    {
        /// <summary>
        /// Prefix for the 
        /// </summary>
        public const string ProtocolPrefix = "file://";

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

            var resourceName = uri.Substring(ProtocolPrefix.Length);
            var fileInfo = new FileInfo(resourceName);
            if (fileInfo.Exists)
                return fileInfo.OpenRead();

            return null;
        }

    }
}
