using System;
using System.Linq;

namespace Workshop.CSharp.Advanced
{
    public class Patterns
    {
        public static void RunCalculation()
        {
            // implementacja bez kontenera IoC
            {
                var complexCalculator = new ComplexCalculator();
                Console.WriteLine(complexCalculator.Average(new float[] { }));
                Console.WriteLine(complexCalculator.Average(new float[] { 10 }));
                Console.WriteLine(complexCalculator.Average(new float[] { 10, 20, 30 }));
            }

            // implementacja wykorzystujaca kontener IoC
            {
                // konfiguracja AOP
                Container.Default.Wrap = Aop.Wrap;
                Aop.ResolveAspect = Container.Default.ResolveInstance;

                // rejestacja serwisow
                Container.Default.Register(typeof(ILogger), new ConsoleLogger());
                Container.Default.Register(typeof(ISimpleCalculator), new SimpleCalculator2());

                var complexCalculator = Container.Default.Resolve<ComplexCalculator2>();
                Console.WriteLine(complexCalculator.Average(new float[] { }));
                Console.WriteLine(complexCalculator.Average(new float[] { 10 }));
                Console.WriteLine(complexCalculator.Average(new float[] { 10, 20, 30 }));
            }
        }


        public interface ISimpleCalculator
        {
            float Add(float a, float b);
            float Div(float a, float b);
        }

        public class SimpleCalculator2 : ISimpleCalculator
        {
            [Logging]
            public float Add(float a, float b)
            {
                return a + b;
            }
            [Logging]
            public float Div(float a, float b)
            {
                return a / b;
            }
        }

        public class ComplexCalculator2
        {
            public ISimpleCalculator SimpleCalculator { get; set; }

            public float? Average(float[] numbers)
            {
                if (numbers.Length == 0)
                {
                    return null;
                }

                var sum = numbers[0];
                foreach (var number in numbers.Skip(1))
                {
                    sum = SimpleCalculator.Add(sum, number);
                }

                return SimpleCalculator.Div(sum, numbers.Length);
            }
        }
    }
}