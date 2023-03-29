using System;
using System.Collections.Generic;

namespace Workshop.CSharp.Advanced
{
    public class Generics
    {
        public static List<Pair<T1, T2>> Zip<T1, T2>(List<T1> list1, List<T2> list2)
        {
            var result = new List<Pair<T1, T2>>();
            var e1 = list1.GetEnumerator();
            var e2 = list2.GetEnumerator();

            while (e1.MoveNext() && e2.MoveNext())
            {
                result.Add(new Pair<T1, T2>(e1.Current, e2.Current));
            }

            return result;
        }

        public static Pair<List<T1>, List<T2>> Unzip<T1, T2>(List<Pair<T1, T2>> list)
        {
            var list1 = new List<T1>();
            var list2 = new List<T2>();

            foreach (var pair in list)
            {
                list1.Add(pair.Value1);
                list2.Add(pair.Value2);
            }

            return new Pair<List<T1>, List<T2>>(list1, list2);
        }

        public static void RunZipUnzip()
        {
            var stringList = new List<string>() { "a", "b", "c", "d" };
            var intList = new List<int>() { 1, 2, 3 };

            var zipped = Zip(stringList, intList);
            foreach (var pair in zipped)
            {
                Console.WriteLine(pair);
            }


            var unzipped = Unzip(zipped);
            foreach (var v in unzipped.Value1)
            {
                Console.WriteLine(v);
            }
            foreach (var v in unzipped.Value2)
            {
                Console.WriteLine(v);
            }
        }
    }

    public class Pair<T1, T2>
    {
        public T1 Value1 { get; set; }
        public T2 Value2 { get; set; }

        public Pair(T1 value1, T2 value2)
        {
            Value1 = value1;
            Value2 = value2;
        }

        public override string ToString()
        {
            return string.Format("{0}-{1}", Value1, Value2);
        }
    }
}