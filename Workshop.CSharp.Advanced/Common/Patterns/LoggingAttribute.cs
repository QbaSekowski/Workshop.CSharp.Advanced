using System;
using System.Linq;
using System.Reflection;

namespace Workshop.CSharp.Advanced
{
    public class LoggingAttribute : Attribute, IMethodAspect
    {
        public ILogger Logger { get; set; }

        public void BeforeMethod(object @object, MethodInfo method, object[] parameters)
        {
            var message = string.Format("{0}({1})", method.Name,
                string.Join(",", method.GetParameters().Zip(parameters, (p, v) => p.Name + ":" + v)));

            Logger.LogMessage(message);
        }

        public void AfterMethod(object @object, MethodInfo method, object result) { }
    }
}