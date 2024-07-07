using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Security.Authentication.ExtendedProtection;

namespace EventBus.UnitTest
{
    [TestClass]
    public class EventBusTests
    {

        private ServiceCollection _serviceDescriptors;

        public EventBusTests()
        {
            _serviceDescriptors = new ServiceCollection();
            _serviceDescriptors.AddLogging(configure => configure.AddConsole());
        }

        [TestMethod]
        public void subscribe_evenet_on_rabbitmq_test()
        {
            var sp = _serviceDescriptors.BuildServiceProvider();
        }
    }
}
