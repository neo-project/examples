using Neo.SmartContract.Framework;

namespace Neo.SmartContract
{
    public class StructExample : FunctionCode
    {
        public static Point Add(Point a, Point b)
        {
            return new Point
            {
                X = a.X + b.X,
                Y = a.Y + b.Y
            };
        }

        public static void Main()
        {
            Point[] array = new[]
            {
                new Point
                {
                    X = 1,
                    Y = 2
                },
                new Point
                {
                    X = 2,
                    Y = 1
                }
            };
            Add(array[0], array[1]);
        }
    }
}
