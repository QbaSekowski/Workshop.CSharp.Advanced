using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Workshop.CSharp.Advanced
{
    public class Reflection
    {
        public static void RunMapper()
        {
            var mapper = new ReflectionMapper();

            var to = new SimpleClass() { Property = "stara wartosc" };
            mapper.Map(new { Property = "nowa wartosc" }, to);

            Console.WriteLine("aktualna wartosc wlaciwosci to : " + to.Property);
        }

        public class SimpleClass
        {
            public string Property { get; set; }
        }
    }

    public class ReflectionMapper : IMapper
    {
        // kesz mapowanych wlasciwosci z typu 'from' do 'to' gdzie kluczem slownika jest format("{0}->{1}",from,to)
        private static readonly Dictionary<string, Tuple<PropertyInfo, PropertyInfo>[]> _mappers =
            new Dictionary<string, Tuple<PropertyInfo, PropertyInfo>[]>();

        public void Map(object from, object to)
        {
            if (@from == null) throw new ArgumentNullException("from");
            if (to == null) throw new ArgumentNullException("to");

            var fromType = from.GetType();
            var toType = to.GetType();

            var key = string.Format("{0}->{1}", fromType.FullName, toType.FullName);

            Tuple<PropertyInfo, PropertyInfo>[] mapper = null;
            if (!_mappers.TryGetValue(key, out mapper))
            {
                mapper =
                    (
                        from fp in fromType.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                            .Where(p => p.GetGetMethod() != null)
                        join tp in toType.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                            .Where(p => p.GetSetMethod() != null)
                            on new { fp.Name, fp.PropertyType } equals new { tp.Name, tp.PropertyType }
                        select Tuple.Create(fp, tp)
                    ).ToArray();

                _mappers.Add(key, mapper);
            }

            // przepisanie wlasciwosci
            foreach (var p in mapper)
            {
                p.Item2.SetValue(to, p.Item1.GetValue(from, null), null);
            }
        }
    }
}