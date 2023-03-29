using System;
using System.Reflection;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("Workshop.CSharp.Advanced.Tests")]

namespace Workshop.CSharp.Advanced
{
    class Program
    {
        static void Main(string[] args)
        {
            Runner.StartRepl(Assembly.GetExecutingAssembly());

            Generics.RunZipUnzip();
            Delegates.RunZip();
            Delegates.RunCompose();
            Iterators.RunZip();
            Iterators.RunFileSequence();

            Runner.ExecuteAll(Assembly.GetExecutingAssembly());
        }
    }
}