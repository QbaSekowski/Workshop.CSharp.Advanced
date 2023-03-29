using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Workshop.CSharp.Advanced
{
    public class Container
    {
        private readonly Dictionary<Type, object> _services = new Dictionary<Type, object>();

        public static Container Default = new Container();
        public Func<Type, object, object> Wrap = (interfaceType, implementation) => implementation;

        public void Register(Type serviceType, object serviceImplementation)
        {
            _services.Add(serviceType, Wrap(serviceType, serviceImplementation));
        }

        public object Resolve(Type type)
        {
            object[] injectedServices = null;
            var constructors = type.GetConstructors();

            // czy istnieje dokladnie jeden niedomyslny konstruktorego ktorego wszystkie wartosci prametrow sa rejestrowanymi serwisami
            if (constructors.Length == 1 &&
                (
                injectedServices =
                    (
                        from p in constructors[0].GetParameters()
                        join kv in _services on p.ParameterType equals kv.Key
                        select kv.Value
                    ).ToArray()
                ).Length > 0 && injectedServices.Length == constructors[0].GetParameters().Length)
            {
                return Activator.CreateInstance(type, injectedServices);
            }

            // wstrzykniecie wlasciwosci
            object result = Activator.CreateInstance(type);
            ResolveInstance(result);
            return result;
        }

        public T Resolve<T>()
        {
            return (T)Resolve(typeof(T));
        }

        public void ResolveInstance(object instance)
        {
            if (instance == null) throw new ArgumentNullException("instance");

            var q =
                from p in instance.GetType()
                    .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(p => p.GetGetMethod() != null && p.GetSetMethod() != null)
                join kv in _services on p.PropertyType equals kv.Key
                select new { property = p, service = kv.Value };

            foreach (var item in q)
            {
                item.property.SetValue(instance, item.service, null);
            }
        }
    }
}
