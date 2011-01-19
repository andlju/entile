using System;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Entile.Test.Tests
{
    [TestClass]
    public class ChannelManagerTest : SilverlightTest
    {
        [TestMethod]
        public void Constructor_UriListNull_DoesNotFail()
        {
            try
            {
                var chFactory = new TestHttpNotificationChannelFactory();
                var target = new ChannelManager("TestChannel", null, true, true, chFactory);
            }
            catch (Exception)
            {
                Assert.Fail("Should not throw exception");
            }
        }

        [TestMethod]
        public void NotificationChannelWithNoUri_OpenChannel_Calls_Open()
        {
            var chFactory = new TestHttpNotificationChannelFactory();
            var target = new ChannelManager("TestChannel", null, true, true, chFactory);

            target.OpenChannelAsync();

            Assert.IsTrue(chFactory.Mock.OpenCalled);
        }

        [TestMethod]
        public void NotificationChannelWithUri_OpenChannel_DoesNotCall_Open()
        {
            var chFactory = new TestHttpNotificationChannelFactory();
            chFactory.Mock.ChannelUri = new Uri("http://test.com");

            var target = new ChannelManager("TestChannel", null, true, true, chFactory);

            target.OpenChannelAsync();

            Assert.IsFalse(chFactory.Mock.OpenCalled);
        }

        [TestMethod]
        public void NotTileBound_ChannelUriEventTriggered_Calls_BindTile()
        {
            var chFactory = new TestHttpNotificationChannelFactory();
            chFactory.Mock.IsShellTileBound = false;

            var target = new ChannelManager("TestChannel", null, true, true, chFactory);
            target.OpenChannelAsync();

            chFactory.Mock.InvokeChannelUriUpdated(null);

            Assert.IsTrue(chFactory.Mock.BindToShellTileCalled);
        }

        [TestMethod]
        public void TileBound_ChannelUriEventTriggered_DoesNotCall_BindTile()
        {
            var chFactory = new TestHttpNotificationChannelFactory();
            chFactory.Mock.IsShellTileBound = true;

            var target = new ChannelManager("TestChannel", null, true, true, chFactory);
            target.OpenChannelAsync();

            chFactory.Mock.InvokeChannelUriUpdated(null);

            Assert.IsFalse(chFactory.Mock.BindToShellTileCalled);
        }

        [TestMethod]
        public void NotToastBound_ChannelUriEventTriggered_Calls_BindToast()
        {
            var chFactory = new TestHttpNotificationChannelFactory();
            chFactory.Mock.IsShellToastBound = false;

            var target = new ChannelManager("TestChannel", null, true, true, chFactory);
            target.OpenChannelAsync();

            chFactory.Mock.InvokeChannelUriUpdated(null);

            Assert.IsTrue(chFactory.Mock.BindToShellToastCalled);
        }

        [TestMethod]
        public void ToastBound_ChannelUriEventTriggered_DoesNotCall_BindToast()
        {
            var chFactory = new TestHttpNotificationChannelFactory();
            chFactory.Mock.IsShellToastBound = true;

            var target = new ChannelManager("TestChannel",  null, true, true, chFactory);
            target.OpenChannelAsync();

            chFactory.Mock.InvokeChannelUriUpdated(null);

            Assert.IsFalse(chFactory.Mock.BindToShellToastCalled);
        }

        [TestMethod]
        [Asynchronous]
        public void ChannelUriUpdated_Invokes_OpenChannelCompleted_Event()
        {
            var testUri = new Uri("http://test.com");

            var chFactory = new TestHttpNotificationChannelFactory();

            var target = new ChannelManager("TestChannel", null, true, true, chFactory);

            target.OpenChannelCompleted += (sender, args) =>
                                                   {
                                                       Assert.IsTrue(args.NotificationUri == testUri);
                                                       EnqueueTestComplete();
                                                   };
            target.OpenChannelAsync();

            chFactory.Mock.ChannelUri = testUri;
            chFactory.Mock.InvokeChannelUriUpdated(null);
        }
    }
}