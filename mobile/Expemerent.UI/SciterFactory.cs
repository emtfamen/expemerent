using System;
using System.Collections.Generic;
using Expemerent.UI.Behaviors;
using Expemerent.UI.Protocol;
using System.IO;
using System.Text;
using System.Diagnostics;

namespace Expemerent.UI
{
    /// <summary>
    /// Defines "factory" delegate
    /// </summary>
    internal delegate T CreateInstance<T>();

    /// <summary>
    /// Allows to extend library with predefied behaviors and protocol handlers
    /// </summary>
    public static class SciterFactory
    {
        /// <summary>
        /// Synchronization object
        /// </summary>
        private static readonly object _syncRoot = new object();

        /// <summary>
        /// Collection of builtin behaviors
        /// </summary>
        private readonly static Dictionary<string, CreateInstance<SciterBehavior>> _behaviors = new Dictionary<string, CreateInstance<SciterBehavior>>();

        /// <summary>
        /// Collection of registered protocol handlers
        /// </summary>
        private readonly static List<IProtocolHandler> _protocols = new List<IProtocolHandler>() { new ResProtocol() };

        /// <summary>
        /// Registers protocol handler
        /// </summary>
        public static void RegiterProtocol<TProtocol>(string prefix) where TProtocol : IProtocolHandler, new()
        {
            #region Arguments checking
            if (String.IsNullOrEmpty(prefix))
                throw new ArgumentNullException("prefix"); 
            #endregion

            lock (_syncRoot)
            {
                _protocols.Add(new TProtocol());
            }
        }

        /// <summary>
        /// Registers a static behavior
        /// </summary>
        public static void RegisterBehavior<TBehavior>(string behavior)
            where TBehavior : SciterBehavior, new()
        {
            #region Arguments checking
            if (String.IsNullOrEmpty(behavior))
                throw new ArgumentNullException("behavior"); 
            #endregion

            lock (_syncRoot)
            {
                SciterBehavior bhv = null;
                _behaviors.Add(behavior, () => (bhv = bhv ?? new TBehavior()));
            }
        }

        /// <summary>
        /// Resolves resource using registered handlers
        /// </summary>
        public static void RegisterBehavior<TBehavior>()
            where TBehavior : SciterBehavior, new()
        {
            var attr = (BehaviorAttribute)Attribute.GetCustomAttribute(typeof(TBehavior), typeof(BehaviorAttribute));
            RegisterBehavior<TBehavior>(attr != null ? attr.BehaviorName : typeof(TBehavior).Name);
        }

        /// <summary>
        /// Resolves resource using registered handlers
        /// </summary>
        /// <remarks>resource stream or null if not found</remarks>
        internal static Stream ResolveStream(string resource, ResourceType resourceType)
        {
            lock (_syncRoot)
            {
                foreach (var prot in _protocols)
                {
                    if (prot.Accept(resource, resourceType))
                        return prot.Resolve(resource, resourceType);
                }
            }

            return null;
        }

        /// <summary>
        /// Resolves resource using registered handlers
        /// </summary>
        internal static byte[] ResolveBinResource(string resource, ResourceType resourceType)
        {
            Debug.Assert(!String.IsNullOrEmpty("resource"), "Resource name cannot be null");

            using (var stm = ResolveStream(resource, resourceType))
            {
                if (stm == null)
                    return null;

                using (var reader = new BinaryReader(stm))            
                    return reader.ReadBytes((int)stm.Length);
            }            
        }

        /// <summary>
        /// Resolves resource using registered handlers
        /// </summary> 
        internal static string ResolveTextResource(string resource, ResourceType resourceType)
        {
            Debug.Assert(!String.IsNullOrEmpty("resource"), "Resource name cannot be null");

            using (var stm = ResolveStream(resource, resourceType))
            {
                if (stm == null)
                    return null;

                using (var reader = new StreamReader(stm, Encoding.UTF8, true))
                    return reader.ReadToEnd();
            }
        }

        /// <summary>
        /// Returns behavior instance 
        /// </summary>
        internal static SciterBehavior ResolveBehavior(string behavior)
        {
            Debug.Assert(!String.IsNullOrEmpty("behavior"), "Behavior name cannot be null");

            var factory =  default(CreateInstance<SciterBehavior>);
            if (_behaviors.TryGetValue(behavior, out factory))
                return factory();

            return null;
        }
    }
}
