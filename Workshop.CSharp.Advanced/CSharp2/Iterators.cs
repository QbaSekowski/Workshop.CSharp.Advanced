using System;
using System.Collections.Generic;
using System.IO;

namespace Workshop.CSharp.Advanced
{
    public class Iterators
    {
        public static IEnumerable<TResult> Zip<T1, T2, TResult>(IEnumerable<T1> list1, IEnumerable<T2> list2, Func<T1, T2, TResult> resultSelector)
        {
            var e1 = list1.GetEnumerator();
            var e2 = list2.GetEnumerator();

            while (e1.MoveNext() && e2.MoveNext())
            {
                yield return resultSelector(e1.Current, e2.Current);
            }
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

        public static IEnumerable<FileInfo> GetFileSequence(string folderPath)
        {
            var files = Directory.GetFiles(folderPath);
            foreach (var file in files)
            {
                yield return new FileInfo(file);
            }

            var directories = Directory.GetDirectories(folderPath);
            foreach (var directory in directories)
            {
                foreach (var file in GetFileSequence(directory))
                {
                    yield return file;
                }
            }
        }

        public static void RunFileSequence()
        {
            var files = GetFileSequence(Consts.ProjectFolderPath);
            foreach (var file in files)
            {
                Console.WriteLine(file.Name);
            }
        }
    }
}
