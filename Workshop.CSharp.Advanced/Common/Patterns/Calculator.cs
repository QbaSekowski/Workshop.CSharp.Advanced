using System.Linq;

namespace Workshop.CSharp.Advanced
{
    public class ComplexCalculator
    {
        public float? Average(float[] numbers)
        {
            if (numbers.Length == 0)
            {
                return null;
            }

            var simpleCalculator = new SimpleCalculator();

            var sum = numbers[0];
            foreach (var number in numbers.Skip(1))
            {
                sum = simpleCalculator.Add(sum, number);
            }

            return simpleCalculator.Div(sum, numbers.Length);
        }
    }

    public class SimpleCalculator
    {
        public float Add(float a, float b)
        {
            return a + b;
        }
        public float Div(float a, float b)
        {
            return a / b;
        }
    }
}