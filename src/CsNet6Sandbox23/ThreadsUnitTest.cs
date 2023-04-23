using FluentAssertions;

namespace CsNet6Sandbox23
{
    public class ThreadsUnitTest
    {
        [Fact]
        public void TEST__ctor__ThreadStart__isStartedShouldBeTrue()
        {
            bool isStarted = false;


            // act
            var t1 = new Thread(() => isStarted = true);

            t1.Start();
            t1.Join();

            isStarted.Should().BeTrue();
        }

        [Fact]
        public void TEST__ctor__ThreadStart__isStartedShouldBeTrue__usingThreadStart()
        {
            bool isStarted = false;


            // act
            var t1 = new Thread(new ThreadStart(() => isStarted = true));


            t1.Start();
            t1.Join();
            isStarted.Should().BeTrue();
        }

        [Fact]
        public void TEST__ctor__ParameterizedThreadStart__isStartedShouldBeTrue()
        {
            bool isStarted = false;


            // act
            var t1 = new Thread((object o) => isStarted = true);


            t1.Start();
            t1.Join();

            isStarted.Should().BeTrue();
        }

        [Fact]
        public void TEST__ctor__ParameterizedThreadStart__isStartedShouldBeTrue__usingThreadStart()
        {
            bool isStarted = false;


            // act
            var t1 = new Thread(new ParameterizedThreadStart((object o) => isStarted = true));


            t1.Start();
            t1.Join();

            isStarted.Should().BeTrue();
        }
    }
}