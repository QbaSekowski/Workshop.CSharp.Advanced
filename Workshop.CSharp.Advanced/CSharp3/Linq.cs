using System;
using System.Linq;

namespace Workshop.CSharp.Advanced
{
    public class Linq
    {
        public static void RunWhereSelect()
        {

            var q =
                from p in DataProvider.Products
                where p.UnitPrice > 50
                select new { p.ProductName, p.UnitPrice };

            q.Print();

        }

        public static void RunLetOrderBy()
        {
            var q = from p in DataProvider.Products
                    let total = p.UnitPrice * p.UnitsInStock
                    orderby total descending
                    select new { p.ProductName, Total = total };

            q.Print();
        }

        public static void RunJoin()
        {
            var q = from p in DataProvider.Products
                    join c in DataProvider.Categories on p.CategoryID equals c.CategoryID
                    orderby c.CategoryName, p.ProductName
                    select new { c.CategoryName, p.ProductName, };

            q.Print();
        }

        public static void RunTakeSkip()
        {
            var q =
                (
                    from p in DataProvider.Products
                    orderby p.UnitPrice descending
                    select p
                ).Skip(5).Take(5);

            q.Print();
        }

        private static string ToPascalCase(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return str;
            }

            return str.Aggregate(
                new { StringBuilder = new System.Text.StringBuilder(), IsUpperCase = true },
                (acc, c) => new
                {
                    StringBuilder = c == '_' ? acc.StringBuilder : acc.StringBuilder.Append(acc.IsUpperCase ? char.ToUpper(c) : char.ToLower(c)),
                    IsUpperCase = c == '_'
                })
                .StringBuilder
                .ToString();
        }

        public static void RunToPascalCase()
        {
            var texts = new[] { "", "MICHAEL_JORDAN", "__MICHAEL___JORDAN__" };
            foreach (var text in texts)
            {
                Console.WriteLine("{0} -> {1}", text, ToPascalCase(text));
            }
        }

        public static void RunLinqOperators()
        {
        }
    }
}