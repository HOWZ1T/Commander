using Xunit;

namespace Commander.Tests
{
    public class TestProgram2 : Program
    {
        public TestProgram2() : base("TestProgram2", true, false)
        {
            AddConvertor(typeof(TestCog2.Point), new TestCog2.PointConvertor());
            Register(new TestCog2(this));
            Register(new TestCog(this));
        }

        [Theory]
        [InlineData("10,10 5,1")]
        [InlineData("\" 10, 10\" \"5 , 1\"")]
        public void TestDistance(string args)
        {
            var res = Run($"Distance {args}");
            Assert.Equal("10.296", res);
        }

        [Theory]
        [InlineData("1", "0", "1")]
        [InlineData("2", "1", "1")]
        [InlineData("3", "1", "2")]
        public void TestCogCommandAdd(string expected, string a, string b)
        {
            var res = Run(new[] {"test", "Add", a, b});
            Assert.Equal(expected, res);
        }

        [Theory]
        [InlineData("test", true)]
        [InlineData("TestCog2", true)]
        [InlineData("commander", false)]
        public void TestHasCog(string name, bool expected)
        {
            Assert.Equal(expected, HasCog(name));
        }

        [Theory]
        [InlineData("test", false)]
        [InlineData("TestCog2", false)]
        [InlineData("commander", true)]
        public void TestGetCog(string name, bool expectNull)
        {
            var cog = GetCog(name);
            if (expectNull)
            {
                Assert.Null(cog);
            }
            else
            {
                Assert.NotNull(cog);
                Assert.Equal(name, cog.Name);
            }
        }
    }
}