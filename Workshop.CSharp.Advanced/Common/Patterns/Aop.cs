using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

// https://devblogs.microsoft.com/dotnet/migrating-realproxy-usage-to-dispatchproxy/
// https://stackoverflow.com/questions/38467753/realproxy-in-dotnet-core

namespace Workshop.CSharp.Advanced
{
    public interface IMethodAspect
    {
        void BeforeMethod(object @object, MethodInfo method, object[] parameters);
        void AfterMethod(object @object, MethodInfo method, object result);
    }

    public static class Aop
    {
        /// <summary>
        /// Ustawic jesli aspekty wykorzystuja Dependency Injection
        /// </summary>
        public static Action<object> ResolveAspect = o => { };

        /// <summary>
        /// Jesli obiekt implementujacy interfejs wykorzystuje aspekty (nad metoda lub klasa),
        /// metoda Wrap zwraca dynamiczna implementacje przekazanego interfejsu 
        /// ktora wywoluje kod aspektow przed/po wywolaniem oryginalnej implementacji.
        /// </summary>
        public static object Wrap(Type @interface, object implementation)
        {
            if (@interface == null) throw new ArgumentNullException("interface");
            if (implementation == null) throw new ArgumentNullException("implementation");

            var implemenationType = implementation.GetType();
            var typeAspects = Attribute.GetCustomAttributes(implemenationType).Where(IsAspect).ToArray();
            var methodAspects =
                (
                    //from m in implemenationType.GetMethods()
                    from m in GetMethods(@interface, implemenationType)
                    let attrs = Attribute.GetCustomAttributes(m.Item2).Where(IsAspect).ToArray()
                    where attrs.Length > 0
                    select Tuple.Create(m.Item1, attrs)
                ).ToArray();

            if (typeAspects.Length > 0 || methodAspects.Length > 0)
            {
                var allAspects = methodAspects
                    .Concat
                    (
                        typeAspects.Length == 0
                            ? Enumerable.Empty<Tuple<MethodInfo, Attribute[]>>()
                            : from m in GetMethods(@interface, implemenationType)
                              select Tuple.Create(m.Item1, typeAspects)
                    )
                    .GroupBy(t => t.Item1, t => t.Item2)
                    .ToDictionary(g => g.Key, g => g.SelectMany(gg => gg).Select(a => a.GetType()).ToArray());

                return Interceptor.CreateInterceptor(@interface, implementation, allAspects, ResolveAspect);
            }

            return implementation;
        }

        public static TInterface Wrap<TInterface>(TInterface implementation)
        {
            return (TInterface)Wrap(typeof(TInterface), implementation);
        }


        private static Tuple<MethodInfo, MethodInfo>[] GetMethods(Type interfaceType, Type implementationType)
        {
            var map = implementationType.GetInterfaceMap(interfaceType);
            return map.InterfaceMethods.Zip(map.TargetMethods, Tuple.Create).ToArray();
        }

        private static bool IsAspect(Attribute attribute)
        {
            return attribute is IMethodAspect;
        }

        public class Interceptor : DispatchProxy
        {
            private object _instance;
            private Action<object> _resolve;
            private Dictionary<MethodInfo, Type[]> _aspects;

            public static object CreateInterceptor(Type interfaceType, object instance, Dictionary<MethodInfo, Type[]> aspects, Action<object> resolve)
            {
                var method = typeof(Interceptor).GetMethods().Single(m => m.IsGenericMethod && m.Name == nameof(CreateInterceptor));
                return method.MakeGenericMethod(interfaceType).Invoke(null, new object[] { instance, aspects, resolve });
            }

            public static T CreateInterceptor<T>(object instance, Dictionary<MethodInfo, Type[]> aspects, Action<object> resolve)
                where T : class
            {
                // generowany jest nowy typ .Net (przykladowa nazwa to 'generatedProxy_1') ktory 
                // - dziedziczy po naszej klasie 'Interceptor' dziedziczacej po 'DispatchProxy'
                // - implementuje przekazany interfejs 'T'
                var interceptor = Create<T, Interceptor>() as Interceptor;

                interceptor._instance = instance;
                interceptor._resolve = resolve;
                interceptor._aspects = aspects;

                return interceptor as T;
            }

            protected override object Invoke(MethodInfo targetMethod, object[] args)
            {
                if (_aspects.TryGetValue(targetMethod, out var aspectTypes))
                {
                    IMethodAspect[] aspectInstances = aspectTypes.Select(Activator.CreateInstance).Cast<IMethodAspect>().ToArray();
                    foreach (var aspectInstance in aspectInstances)
                    {
                        _resolve(aspectInstance);
                        aspectInstance.BeforeMethod(_instance, targetMethod, args);
                    }
                    var result = targetMethod.Invoke(_instance, args);
                    foreach (var aspectInstance in aspectInstances)
                    {
                        aspectInstance.AfterMethod(_instance, targetMethod, result);
                    }
                    return result;
                }
                else
                {
                    return targetMethod.Invoke(_instance, args);
                }
            }
        }
    }
}
