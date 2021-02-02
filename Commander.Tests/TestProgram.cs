using System;
using System.Text.RegularExpressions;
using Xunit;

namespace Commander.Tests
{
    public class TestProgram : Commander.Program
    {
        public TestProgram() : base("TestProgram")
        {
            Register(new TestCog(this));
        }
        
        [Fact]
        public void TestProgramCreationAndCogCreationAndRegister()
        {
            Program prog = this;
            Assert.NotNull(prog);
            Assert.Equal("TestProgram", prog.Name);
            Assert.False(prog.IsCaseSensitive);
            Assert.True(prog.UseProgramNamePrefix);

            Cog c = GetCog("test");
            Assert.Equal("yeah yeah", c.Description);
            Assert.Equal("test", c.Name);
            Assert.Equal(this.IsCaseSensitive, c.IsCaseSensitive);
            Assert.True(c.IsGroup);
            Assert.Equal(new string[]{"test"}, c.Commands.Keys);
            Assert.NotNull(c.GetCommand("Say"));
            Assert.NotNull(c.GetCommand("pinger"));
            Assert.NotNull(c.GetCommand("Echo"));
            Assert.Single(AllCogs());
        }

        [Theory]
        [InlineData(0, null)]
        [InlineData(1, "hello test world!")]
        public void TestCogCommandSay(int i, string val)
        {
            if (i == 0)
            {
                var res = this.Run(new[] {"testprogram", "test", "say"});  
                Utils.Debug(res);
                Assert.Equal("test says: \"Hello World\"", res);
            }
            else
            {
                var res = this.Run(new[] {"testprogram", "test", "say", val});
                Assert.Equal($"test says: \"{val}\"", res);
            }
        }

        [Theory]
        [InlineData("1", "0", "1")]
        [InlineData("2", "1", "1")]
        [InlineData("3", "1", "2")]
        public void TestCogCommandAdd(string expected, string a, string b)
        {
            var res = this.Run(new[] {"testprogram", "test", "add", a, b});
            Assert.Equal(expected, res);
        }

        [Fact]
        public void TestRunWithStr()
        {
            var res = Run("testprogram test add 10 100");
            Assert.Equal("110", res);
        }

        [Theory]
        [InlineData("hello world")]
        [InlineData("Hello World!")]
        [InlineData("\"Hello World!\"")]
        [InlineData("....AQUA!")]
        public void TestEcho(string data)
        {
            var runStr = "testprogram test echo ";
            runStr = (data.Contains(' ')) ? $"{runStr}'{data}'" : $"{runStr}{data}";
            var res = Run(runStr);
            Assert.Equal(data, res);
        }

        [Theory]
        [InlineData("www.google.com")]
        [InlineData("www.github.com")]
        public void TestPing(string url)
        {
            var res = Run($"testprogram test pinger {url}");
            Utils.Debug(res);
            Assert.Matches(new Regex(@"^[0-9]+\ ms$"), res);
        }
    }
}
