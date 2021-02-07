using System.Net.NetworkInformation;
using Commander.Attributes;

namespace Commander.Tests
{
    [CommandGroup("test", Description = "yeah yeah")]
    public class TestCog : Cog
    {
        public TestCog(Program prog) : base(prog)
        {
        }

        [Command(Name = "pinger", Description = "pings a website")]
        [Example("@c https://www.google.com")]
        public string Ping(string url)
        {
            var ping = new Ping();
            var result = ping.Send(url);
            return result.RoundtripTime + " ms";
        }

        [Command(Parent = "pinger")]
        [Example("@c 'hello there'")]
        [Example("@c 'general kenobi'")]
        public string Echo(string message)
        {
            return message;
        }

        [Command]
        public string Say(string message = "Hello World")
        {
            return $"{Name} says: \"{message}\"";
        }

        [Command]
        public string Add(int a, int b)
        {
            return $"{(a + b).ToString()}";
        }
    }
}