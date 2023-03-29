using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Workshop.CSharp.Advanced
{
    public class ExpressionTrees
    {
        public static void RunGetPropertyName()
        {
            Console.WriteLine(GetPropertyName(() => DateTime.Now));
            Console.WriteLine(GetPropertyName(() => "".Length));
        }

        public static string GetPropertyName<T>(Expression<Func<T>> expression)
        {
            return (expression.Body as MemberExpression).Member.Name;
        }


        public static Func<T, object> CreatePropertySelector<T>(string propertyName)
        {
            var paramter = Expression.Parameter(typeof(T), "item");

            var lambda = Expression.Lambda<Func<T, object>>(
                Expression.Convert(
                    Expression.Property(paramter, propertyName),
                    typeof(object))
                , new[] { paramter });

            return lambda.Compile();
        }

        public class Pair
        {
            public string StringProperty { get; set; }
            public int IntProperty { get; set; }
        }

        public static void RunCreatePropertySelector()
        {
            var items = new[]
                {
                    new Pair{StringProperty = "a", IntProperty = 5},
                    new Pair{StringProperty = "g", IntProperty = 0},
                    new Pair{StringProperty = "x", IntProperty = 14},
                    new Pair{StringProperty = "c", IntProperty = -4},
                };

            var lengthSelector = CreatePropertySelector<Pair>("IntProperty");

            foreach (var item in items.OrderBy(lengthSelector))
            {
                Console.WriteLine(new { item.StringProperty, item.IntProperty });
            }
        }


        public static void RunMapper()
        {
            var mapper = new ExpressionTreeMapper();

            var to = new SimpleClass() { Property = "stara wartosc" };
            mapper.Map(new { Property = "nowa wartosc" }, to);

            Console.WriteLine("aktualna wartosc wlaciwosci to : " + to.Property);
        }


        public class SimpleClass
        {
            public string Property { get; set; }
        }
    }

    public class ExpressionTreeMapper : IMapper
    {
        // kesz mapowanych wlasciwosci z typu 'from' do 'to' gdzie kluczem slownika jest format("{0}->{1}",from,to)
        private static readonly Dictionary<string, Action<object, object>> _mappers =
            new Dictionary<string, Action<object, object>>();

        public void Map(object from, object to)
        {
            if (@from == null) throw new ArgumentNullException("from");
            if (to == null) throw new ArgumentNullException("to");

            var fromType = from.GetType();
            var toType = to.GetType();

            var key = string.Format("{0}->{1}", fromType.FullName, toType.FullName);

            Action<object, object> mapper = null;
            if (!_mappers.TryGetValue(key, out mapper))
            {
                ParameterExpression fromParamExpr = Expression.Parameter(typeof(object), "fromParam");
                ParameterExpression toParamExpr = Expression.Parameter(typeof(object), "toParam");

                ParameterExpression fromVarExpr = Expression.Variable(fromType, "fromVar");
                ParameterExpression toVarExpr = Expression.Variable(toType, "toVar");

                // znalezienie pasujacych do siebie par wlasciwosci
                var properties =
                    (
                        from fp in fromType.GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(p => p.GetGetMethod() != null)
                        join tp in toType.GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(p => p.GetSetMethod() != null)
                            on new { fp.Name, fp.PropertyType } equals new { tp.Name, tp.PropertyType }
                        select Tuple.Create(fp, tp)
                    ).ToArray();

                var mapperExpression = Expression.Lambda<Action<object, object>>(
                    Expression.Block(
                        new[] { fromVarExpr, toVarExpr }, // zmienne
                        new Expression[]
                        {
                            Expression.Assign(fromVarExpr, Expression.Convert(fromParamExpr, fromType)),
                            Expression.Assign(toVarExpr, Expression.Convert(toParamExpr, toType))
                        }
                        .Concat(
                            properties.Select(map => Expression.Assign(
                                Expression.Property(toVarExpr, map.Item2),
                                Expression.Property(fromVarExpr, map.Item1)))
                        )
                    ),
                        fromParamExpr, toParamExpr // parametry
                    );

                mapper = mapperExpression.Compile();
                _mappers.Add(key, mapper);
            }

            mapper(from, to);
        }
    }
}
