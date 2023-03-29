using System;
using System.Collections.Generic;

namespace Workshop.CSharp.Advanced
{
    public class ServiceLocator
    {
        private static readonly Dictionary<Type, object> _services = new Dictionary<Type, object>();

        public static void Register(Type serviceType, object serviceImplementation)
        {
            _services.Add(serviceType, serviceImplementation);
        }
        public static object Get(Type serviceType)
        {
            return _services[serviceType];
        }
        public static T Get<T>()
        {
            return (T)Get(typeof(T));
        }
    }
}
