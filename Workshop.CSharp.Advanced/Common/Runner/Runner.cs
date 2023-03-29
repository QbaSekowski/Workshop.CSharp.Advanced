using System;
using System.Diagnostics;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;

namespace Workshop.CSharp.Advanced
{
    public class Runner
    {
        public static void StartRepl(Assembly assembly)
        {
            var methods = FindMethods(assembly).ToArray();

            while (true)
            {
                bool debugMode = false;
                int index = 0;

                Console.WriteLine();
                foreach (var methodInfo in methods)
                {
                    Console.WriteLine("{0,-3} - {1}.{2}", index++, methodInfo.DeclaringType.Name, methodInfo.Name);
                }
                Console.WriteLine("Podaj numer metody (oraz ! jesli chcesz debugowac), 'cls' aby wyczyscic, Enter aby zakonczyc : ");

                var line = Console.ReadLine();

                if (line == "")
                {
                    return;
                }

                if (line == "cls")
                {
                    Console.Clear();
                }
                else
                {
                    if (line.Contains("!"))
                    {
                        line = line.Replace("!", "");
                        debugMode = true;
                    }

                    int number;
                    if (int.TryParse(line, out number) && number >= 0 && number < methods.Length)
                    {
                        ExecuteMethod(methods[number], debugMode);
                    }
                }
            }
        }

        public static void ExecuteAll(Assembly assembly)
        {
            var methods = FindMethods(assembly);
            foreach (var method in methods)
            {
                ExecuteMethod(method, false);
            }
        }

        private static IEnumerable<MethodInfo> FindMethods(Assembly assembly)
        {
            return assembly.GetTypes().SelectMany(t => t.GetMethods()).Where(m => m.IsStatic && m.Name.StartsWith("Run"));
        }

        private static void ExecuteMethod(MethodInfo method, bool debugMode)
        {
            Console.WriteLine(new String('-', 100));
            Console.WriteLine("{0}.{1}", method.DeclaringType.Name, method.Name);

            try
            {
                if (debugMode)
                {
                    Debugger.Break();
                }
                method.Invoke(null, new object[0]);
                Console.WriteLine();
            }
            catch (TargetInvocationException exception)
            {
                Console.WriteLine("Exception: " + exception.InnerException.Message);
            }
        }
    }
}