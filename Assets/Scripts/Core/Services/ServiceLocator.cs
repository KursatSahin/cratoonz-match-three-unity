using System;
using System.Collections.Generic;

namespace Core.Services
{
    public class ServiceLocator
    {
        private static ServiceLocator _instance;
        public static ServiceLocator Instance => _instance ?? (_instance = new ServiceLocator());

        private readonly Dictionary<Type, IService> _services;

        private ServiceLocator()
        {
            _services = new Dictionary<Type, IService>();
        }

        /// <summary>
        /// Add the service as a type of T service to the services dictionary.
        /// </summary>
        /// <param name="service">Instance object of service</param>
        /// <typeparam name="T">Generic type service</typeparam>
        public void RegisterService<T>(T service) where T : IService
        {
            _services[typeof(T)] = service;
        }
        
        /// <summary>
        /// Returns service of type T if registered.
        /// </summary>
        /// <typeparam name="T">Generic type service</typeparam>
        /// <returns>Instance object of service</returns>
        /// <exception cref="Exception">Throws if requested service was not registered</exception>
        public T Get<T>() where T : IService
        {
            var type = typeof(T);
            if (!_services.ContainsKey(type))
            {
                throw new Exception($"{type.Name} not registered.");
            }
            
            return (T) _services[type];
        }
    }
}