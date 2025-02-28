using System;
using System.Collections.Generic;
using UnityEngine;
using WibeSoft.Core.Singleton;

namespace WibeSoft.Core.DI
{
    /// <summary>
    /// Dependency Injection Container class
    /// </summary>
    public class DIContainer : SingletonBehaviour<DIContainer>
    {
        private readonly Dictionary<Type, object> _services = new Dictionary<Type, object>();
        private readonly Dictionary<Type, List<Type>> _implementations = new Dictionary<Type, List<Type>>();

        /// <summary>
        /// Register service to container
        /// </summary>
        public void RegisterService<T>(T service) where T : class
        {
            var type = typeof(T);
            if (_services.ContainsKey(type))
            {
                Debug.LogWarning($"Service {type.Name} is already registered. It will be overwritten.");
            }
            _services[type] = service;

            // Register interface implementations
            foreach (var interfaceType in type.GetInterfaces())
            {
                if (!_implementations.ContainsKey(interfaceType))
                {
                    _implementations[interfaceType] = new List<Type>();
                }
                _implementations[interfaceType].Add(type);
            }

            Debug.Log($"Service {type.Name} registered successfully.");
        }

        /// <summary>
        /// Get service from container
        /// </summary>
        public T GetService<T>() where T : class
        {
            var type = typeof(T);
            if (!_services.ContainsKey(type))
            {
                Debug.LogError($"Service {type.Name} is not registered.");
                return null;
            }
            return _services[type] as T;
        }

        /// <summary>
        /// Get all implementations of an interface
        /// </summary>
        public IEnumerable<T> GetImplementations<T>() where T : class
        {
            var type = typeof(T);
            if (!_implementations.ContainsKey(type))
            {
                yield break;
            }

            foreach (var implementationType in _implementations[type])
            {
                if (_services.ContainsKey(implementationType))
                {
                    yield return _services[implementationType] as T;
                }
            }
        }

        /// <summary>
        /// Unregister service from container
        /// </summary>
        public void UnregisterService<T>() where T : class
        {
            var type = typeof(T);
            if (_services.ContainsKey(type))
            {
                _services.Remove(type);

                // Clean up interface registrations
                foreach (var interfaceType in type.GetInterfaces())
                {
                    if (_implementations.ContainsKey(interfaceType))
                    {
                        _implementations[interfaceType].Remove(type);
                    }
                }

                Debug.Log($"Service {type.Name} unregistered successfully.");
            }
        }

        /// <summary>
        /// Clear container
        /// </summary>
        public void Clear()
        {
            _services.Clear();
            _implementations.Clear();
            Debug.Log("DI Container cleared.");
        }
    }
} 