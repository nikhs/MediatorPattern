using System;
using Xunit;

namespace MediatorPattern.Core.Tests
{
    public class MediatorClientTest
    {
        readonly IMediator mediator;
        static readonly Pong pong = new Pong();

        class Ping : IRequest<Pong> { }
        class Pong { }

        class PongHandler : IHandler<Ping, Pong>
        {
            public Pong Handle(Ping request)
            {
                return pong;
            }
        }


        public MediatorClientTest()
        {
            mediator = new NaiveMediator();
            mediator.Register(typeof(Ping), typeof(PongHandler));
        }

        [Fact]
        public void BaselineMediatorRoundtripTest()
        {
            var ping = new Ping();
            var actualPong = mediator.Send<Pong>(ping);
            Assert.Equal(MediatorClientTest.pong, actualPong);
        }

        private class GenericRequest : IRequest<object> { }

        [Fact]
        public void GivenHandlerIsIncorrectType_OnMediatorSend_ThrowsErr()
        {
            mediator.Register(typeof(GenericRequest), typeof(object));
            var request = new GenericRequest();
            Assert.Throws<ArgumentException>(() => mediator.Send<object>(request));
        }
    }
}
