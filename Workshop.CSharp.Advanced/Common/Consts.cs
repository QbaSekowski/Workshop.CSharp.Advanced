using System.IO;
using System.Reflection;

namespace Workshop.CSharp.Advanced
{
    public class Consts
    {
        public static string ProjectFolderPath { get; } =
            Path.Combine(new[] { new FileInfo(Assembly.GetEntryAssembly().Location).DirectoryName!, "..", "..", ".." });
    }
}
