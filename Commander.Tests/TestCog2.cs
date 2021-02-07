using System;
using System.Text.RegularExpressions;
using Commander.Attributes;
using Commander.Convertors;

namespace Commander.Tests
{
    public class TestCog2 : Cog
    {
        public TestCog2(Program prog) : base(prog)
        {
        }

        [Command(Description = "gets the distance between two points")]
        [Example("@c 10,10 5,5")]
        [Example("@c \"10, 10\" \"5, 5\"")]
        public string Distance(Point a, Point b)
        {
            return string.Format("{0:F}", a.Distance(b));
        }

        public struct Point
        {
            public int X, Y;

            public Point(int x, int y)
            {
                X = x;
                Y = y;
            }

            public double Distance(Point p)
            {
                return Math.Sqrt(Math.Pow(p.X - X, 2) + Math.Pow(p.Y - Y, 2));
            }
        }

        public class PointConvertor : Convertor
        {
            private readonly Regex _regex;
            private readonly string _regexPattern = @"^[\ 0-9\,]+$";

            public PointConvertor()
            {
                _regex = new Regex(_regexPattern);
            }

            /// <summary>
            ///     Attempts to convert the given string value to a Point struct.
            /// </summary>
            /// <seealso cref="Convertor.TryConvert(string,out object)" />
            public override bool TryConvert(Type type, string val, out object res)
            {
                // format: "x,y"
                if (!_regex.IsMatch(val))
                {
                    res = null;
                    return false;
                }

                var parts = val.Trim().Split(',');
                var x = int.Parse(parts[0].Trim());
                var y = int.Parse(parts[1].Trim());

                res = new Point(x, y);
                return true;
            }
        }
    }
}