using System;
using Xunit;

namespace Commander.Tests
{
    public class ArgPolicyTest
    {
        [Theory]
        [InlineData("one")]
        [InlineData("one", "two")]
        public void ArgPolicyExactTest(params string[] args)
        {
            ArgPolicy policy = new ArgPolicy(1);
            if (args.Length > 1)
            {
                Assert.False(policy.isValid(args.Length));
            }
            else
            {
                Assert.True(policy.isValid(args.Length));
            }
        }
        
        [Theory]
        [InlineData("one", "two", "three")]
        [InlineData("one")]
        public void ArgPolicyMinTest(params string[] args)
        {
            ArgPolicy policy = new ArgPolicy(2, Policy.Min);

            if (args.Length < 2)
            {
                Assert.False(policy.isValid(args.Length));
            }
            else
            {
                Assert.True(policy.isValid(args.Length));   
            }
        }
        
        [Theory]
        [InlineData("one", "two")]
        [InlineData("one", "two", "three")]
        public void ArgPolicyMaxTest(params string[] args)
        {
            ArgPolicy policy = new ArgPolicy(2, Policy.Max);

            if (args.Length > 2)
            {
                Assert.False(policy.isValid(args.Length));
            }
            else
            {
                Assert.True(policy.isValid(args.Length));   
            }
        }
        
        [Theory]
        [InlineData("one", "two")]
        [InlineData()]
        [InlineData("one", "two", "three", "four")]
        public void ArgPolicyBetweenTest(params string[] args)
        {
            ArgPolicy policy = new ArgPolicy(1, 3, Policy.Between);
            if (args.Length == 2)
            {
                Assert.True(policy.isValid(args.Length));   
            }
            else
            {
                Assert.False(policy.isValid(args.Length));
            }
        }
        
        [Theory]
        [InlineData()]
        [InlineData("one")]
        public void ArgPolicyNoneTest(params string[] args)
        {
            ArgPolicy policy = new ArgPolicy();
            if (args.Length > 0)
            {
                Assert.False(policy.isValid(args.Length));
            }
            else
            {
                Assert.True(policy.isValid(args.Length));   
            }
        }

        [Theory]
        [InlineData("one")]
        [InlineData("one", "two")]
        [InlineData()]
        public void TwoArgConstructorBetweenPolicyTest(params string[] args)
        {
            ArgPolicy policy = new ArgPolicy(1, Policy.Between);
            if (args.Length == 1)
            {
                Assert.True(policy.isValid(args.Length));
            }
            else
            {
                Assert.False(policy.isValid(args.Length));   
            }
        }
    }
}
