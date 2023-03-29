using System;
using System.Collections.Generic;

namespace Workshop.CSharp.Advanced
{
    public class Delegates
    {

        public static List<TResult> Zip<T1, T2, TResult>(List<T1> list1, List<T2> list2, Func<T1, T2, TResult> resultSelector)
        {
            var result = new List<TResult>();
            var e1 = list1.GetEnumerator();
            var e2 = list2.GetEnumerator();

            while (e1.MoveNext() && e2.MoveNext())
            {
                result.Add(resultSelector(e1.Current, e2.Current));
            }

            return result;
        }

        public static void RunZip()
        {
            var from1To5 = new List<int> { 1, 2, 3, 4, 5 };
            var from6To10 = new List<int> { 6, 7, 8, 9, 10 };

            foreach (var item in Zip(from1To5, from6To10, (i, j) => i + j))
            {
                Console.WriteLine(item);
            }
        }

        public static Func<T1, T3> Compose<T1, T2, T3>(Func<T1, T2> f1, Func<T2, T3> f2)
        {
            return delegate (T1 a)
            {
                var f1Result = f1(a);
                var f2Result = f2(f1Result);
                return f2Result;
            };
        }

        public static void RunCompose()
        {
            // Compose
            Func<int, int> add1 = a => a + 1;
            Func<int, string> toString = b => b.ToString();
            Func<int, string> composition = Compose(add1, toString);

            Console.WriteLine(composition(10));
            Console.WriteLine(Compose(Compose(add1, add1), toString)(10));
        }
    }
}