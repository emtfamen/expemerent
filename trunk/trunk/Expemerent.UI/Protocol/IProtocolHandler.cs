using System;
using System.IO;

namespace Expemerent.UI.Protocol
{
    /// <summary>
    /// Base interface for handling resource loading 
    /// </summary>
    public interface IProtocolHandler
    {
        /// <summary>
        /// Used to check whether handler can deal with specified resource
        /// </summary>
        bool Accept(String uri, ResourceType resourceType);

        /// <summary>
        /// Loads specified resource
        /// </summary>
        Stream Resolve(String uri, ResourceType resourceType);
    }
}
