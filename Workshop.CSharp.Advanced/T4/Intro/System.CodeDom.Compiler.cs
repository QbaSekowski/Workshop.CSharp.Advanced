using System.Collections.Generic;

namespace System.CodeDom.Compiler
{
    public class CompilerErrorCollection
    {
        public List<CompilerError> Errors = new List<CompilerError>();

        public void Add(CompilerError error)
        {
            Errors.Add(error);
        }
    }

    public class CompilerError : Tuple<object, int, int, object, string>
    {
        public bool IsWarning { get; set; }

        public CompilerError(object arg1, int arg2, int arg3, object arg4, string arg5)
            : base(arg1, arg2, arg3, arg4, arg5)
        {
        }
    }
}
