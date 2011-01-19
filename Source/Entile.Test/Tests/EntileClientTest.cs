using System;
using System.Collections.Generic;
using Microsoft.Phone.Notification;
using Microsoft.Phone.Shell;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Entile.Test.Tests
{

    #region Mocks

    class MockChannelManager : IChannelManager
    {
        public bool OpenChannelAsyncCalled;
        public bool CloseChannelAsyncCalled;

        public void OpenChannelAsync()
        {
            OpenChannelAsyncCalled = true;
        }

        public void CloseChannel()
        {
            CloseChannelAsyncCalled = true;
        }

        public event EventHandler<OpenChannelCompletedEventArgs> OpenChannelCompleted;

        public void InvokeOpenChannelCompleted(OpenChannelCompletedEventArgs e)
        {
            EventHandler<OpenChannelCompletedEventArgs> handler = OpenChannelCompleted;
            if (handler != null) handler(this, e);
        }

        public event EventHandler<HttpNotificationEventArgs> HttpNotificationReceived;

        public void InvokeHttpNotificationReceived(HttpNotificationEventArgs e)
        {
            EventHandler<HttpNotificationEventArgs> handler = HttpNotificationReceived;
            if (handler != null) handler(this, e);
        }

        public event EventHandler<NotificationEventArgs> ShellToastNotificationReceived;

        public void InvokeShellToastNotificationReceived(NotificationEventArgs e)
        {
            EventHandler<NotificationEventArgs> handler = ShellToastNotificationReceived;
            if (handler != null) handler(this, e);
        }
    }

    class MockRegistrationManager : IRegistrationManager
    {
        public bool RegisterAsyncCalled;
        public bool UpdateExtraInfoAsyncCalled;
        public bool UnregisterAsyncCalled;
        public Uri InvokedNotificationUri;
        public IDictionary<string, string> InvokedExtraInfo;

        public void RegisterAsync(Uri notificationUri, IDictionary<string, string> extraInfo)
        {
            RegisterAsyncCalled = true;
            InvokedNotificationUri = notificationUri;
            InvokedExtraInfo = extraInfo;
        }

        public void UpdateExtraInfoAsync(IDictionary<string, string> extraInfo)
        {
            UpdateExtraInfoAsyncCalled = true;
            InvokedExtraInfo = extraInfo;
        }

        public void UnregisterAsync()
        {
            UnregisterAsyncCalled = true;
        }

        public event EventHandler<RegisterCompletedEventArgs> RegisterCompleted;

        public void InvokeRegisterCompleted(RegisterCompletedEventArgs e)
        {
            EventHandler<RegisterCompletedEventArgs> handler = RegisterCompleted;
            if (handler != null) handler(this, e);
        }

        public event EventHandler<UnregisterCompletedEventArgs> UnregisterCompleted;

        public void InvokeUnregisterCompleted(UnregisterCompletedEventArgs e)
        {
            EventHandler<UnregisterCompletedEventArgs> handler = UnregisterCompleted;
            if (handler != null) handler(this, e);
        }

        public event EventHandler<UpdateExtraInfoCompletedEventArgs> UpdateExtraInfoCompleted;

        public void InvokeUpdateExtraInfoCompleted(UpdateExtraInfoCompletedEventArgs e)
        {
            EventHandler<UpdateExtraInfoCompletedEventArgs> handler = UpdateExtraInfoCompleted;
            if (handler != null) handler(this, e);
        }
    }

    class MockRemoteTileManager : IRemoteTileManager
    {
        public bool StartCalled;
        public bool StopCalled;
        public string InvokedTileUri;
        public UpdateInterval InvokedUpdateInterval;

        public void Start(string tileUri, UpdateInterval updateInterval)
        {
            StartCalled = true;
            InvokedTileUri = tileUri;
            InvokedUpdateInterval = updateInterval;
        }

        public void Stop()
        {
            StopCalled = true;
        }
    }

    #endregion

    [TestClass]
    public class EntileClientTest : SilverlightTest
    {
        private MockChannelManager _mockChannelManager;
        private MockRegistrationManager _mockRegistrationManager;
        private MockRemoteTileManager _mockRemoteTileManager;
        private MockSettingsProvider _mockSettingsProvider;

        private Uri _testChannelUri = new Uri("http://test.com/channel");

        private string _testTileUri = "http://test.com/remotetile.png";

        [TestInitialize]
        public void Initialize()
        {
            _mockChannelManager = new MockChannelManager();
            _mockRegistrationManager = new MockRegistrationManager();
            _mockRemoteTileManager = new MockRemoteTileManager();
            _mockSettingsProvider = new MockSettingsProvider();
        }

        // Enable/Disable
        [TestMethod]
        public void Enable_When_Disabled_Opens_Channel()
        {
            _mockSettingsProvider.Enabled = false;
            var target = new EntileClient(_mockChannelManager, _mockRegistrationManager, _mockRemoteTileManager, _mockSettingsProvider);

            target.Enable = true;

            Assert.IsTrue(_mockChannelManager.OpenChannelAsyncCalled);
        }

        [TestMethod]
        public void Enable_When_Enabled_Does_Not_Open_Channel()
        {
            _mockSettingsProvider.Enabled = true;
            var target = new EntileClient(_mockChannelManager, _mockRegistrationManager, _mockRemoteTileManager, _mockSettingsProvider);

            target.Enable = true;

            Assert.IsFalse(_mockChannelManager.OpenChannelAsyncCalled);
        }

        [TestMethod]
        public void Disable_When_Enabled_Closes_Channel()
        {
            _mockSettingsProvider.Enabled = true;
            var target = new EntileClient(_mockChannelManager, _mockRegistrationManager, _mockRemoteTileManager, _mockSettingsProvider);

            target.Enable = false;

            Assert.IsTrue(_mockChannelManager.CloseChannelAsyncCalled);
        }

        [TestMethod]
        public void Disable_When_Disabled_Does_Not_Close_Channel()
        {
            _mockSettingsProvider.Enabled = false;
            var target = new EntileClient(_mockChannelManager, _mockRegistrationManager, _mockRemoteTileManager, _mockSettingsProvider);

            target.Enable = false;

            Assert.IsFalse(_mockChannelManager.CloseChannelAsyncCalled);
        }

        // Busy flag
        [TestMethod]
        public void Busy_Flag_Set_When_Enabling()
        {
            _mockSettingsProvider.Enabled = false;

            var target = new EntileClient(_mockChannelManager, _mockRegistrationManager, _mockRemoteTileManager, _mockSettingsProvider);

            target.Enable = true;

            Assert.IsTrue(target.Busy);
        }

        //[TestMethod] Deliberately ignored for now. But we may want to do this...
        public void Busy_Flag_Set_When_Disabling()
        {
            _mockSettingsProvider.Enabled = true;

            var target = new EntileClient(_mockChannelManager, _mockRegistrationManager, _mockRemoteTileManager, _mockSettingsProvider);

            target.Enable = false;

            Assert.IsTrue(target.Busy);
        }

        [TestMethod]
        public void Busy_Flag_Reset_When_Open_Channel_Failed()
        {
            _mockSettingsProvider.Enabled = false;

            var target = new EntileClient(_mockChannelManager, _mockRegistrationManager, _mockRemoteTileManager, _mockSettingsProvider);

            target.Enable = true;

            _mockChannelManager.InvokeOpenChannelCompleted(new OpenChannelCompletedEventArgs(ChannelErrorType.ChannelOpenFailed));

            Assert.IsFalse(target.Busy);
        }

        [TestMethod]
        public void Registration_Started_When_Open_Channel_Complete()
        {
            _mockSettingsProvider.Enabled = false;

            var target = new EntileClient(_mockChannelManager, _mockRegistrationManager, _mockRemoteTileManager, _mockSettingsProvider);

            target.Enable = true;

            _mockChannelManager.InvokeOpenChannelCompleted(new OpenChannelCompletedEventArgs(_testChannelUri));

            Assert.IsTrue(_mockRegistrationManager.RegisterAsyncCalled);
        }

        [TestMethod]
        public void Registration_Invoked_With_Returned_Channel_When_Open_Channel_Complete()
        {
            _mockSettingsProvider.Enabled = false;

            var target = new EntileClient(_mockChannelManager, _mockRegistrationManager, _mockRemoteTileManager, _mockSettingsProvider);

            target.Enable = true;

            _mockChannelManager.InvokeOpenChannelCompleted(new OpenChannelCompletedEventArgs(_testChannelUri));

            Assert.AreEqual(_testChannelUri, _mockRegistrationManager.InvokedNotificationUri);
        }

        [Asynchronous]
        [TestMethod]
        public void Error_Raised_When_Open_Channel_Failed()
        {
            _mockSettingsProvider.Enabled = false;

            var target = new EntileClient(_mockChannelManager, _mockRegistrationManager, _mockRemoteTileManager, _mockSettingsProvider);
            target.ErrorOccured += (s, e) =>
                                       {
                                           Assert.AreEqual("Error while opening channel", e.ErrorMessage);
                                           EnqueueTestComplete();
                                       };

            target.Enable = true;

            _mockChannelManager.InvokeOpenChannelCompleted(new OpenChannelCompletedEventArgs(ChannelErrorType.ChannelOpenFailed));

        }

        [TestMethod]
        public void Busy_Flag_Reset_When_Registration_Failed()
        {
            _mockSettingsProvider.Enabled = false;

            var target = new EntileClient(_mockChannelManager, _mockRegistrationManager, _mockRemoteTileManager, _mockSettingsProvider);

            _mockRegistrationManager.InvokeRegisterCompleted(new RegisterCompletedEventArgs(false, null, null));

            Assert.IsFalse(target.Busy);
        }

        [Asynchronous]
        [TestMethod]
        public void Error_Raised_When_Registration_Failed()
        {
            _mockSettingsProvider.Enabled = false;

            var target = new EntileClient(_mockChannelManager, _mockRegistrationManager, _mockRemoteTileManager, _mockSettingsProvider);
            target.ErrorOccured += (s, e) =>
            {
                Assert.AreEqual("Error while registering with provider", e.ErrorMessage);
                EnqueueTestComplete();
            };

            _mockRegistrationManager.InvokeRegisterCompleted(new RegisterCompletedEventArgs(false, null, null));

        }

        [TestMethod]
        public void Busy_Flag_Reset_When_Registration_Complete()
        {
            _mockSettingsProvider.Enabled = false;

            var target = new EntileClient(_mockChannelManager, _mockRegistrationManager, _mockRemoteTileManager, _mockSettingsProvider);

            _mockRegistrationManager.InvokeRegisterCompleted(new RegisterCompletedEventArgs(true, _testTileUri, "EveryHour"));

            Assert.IsFalse(target.Busy);
        }

        [TestMethod]
        public void RemoteTile_Started_When_Registration_Completes_With_Tile()
        {
            _mockSettingsProvider.Enabled = false;

            var target = new EntileClient(_mockChannelManager, _mockRegistrationManager, _mockRemoteTileManager, _mockSettingsProvider);

            _mockRegistrationManager.InvokeRegisterCompleted(new RegisterCompletedEventArgs(true, _testTileUri, "EveryHour"));

            Assert.IsTrue(_mockRemoteTileManager.StartCalled);
        }

        [TestMethod]
        public void RemoteTile_Not_Started_When_Registration_Completes_Without_Tile()
        {
            _mockSettingsProvider.Enabled = false;

            var target = new EntileClient(_mockChannelManager, _mockRegistrationManager, _mockRemoteTileManager, _mockSettingsProvider);

            _mockRegistrationManager.InvokeRegisterCompleted(new RegisterCompletedEventArgs(true, null, null));

            Assert.IsFalse(_mockRemoteTileManager.StartCalled);
        }

        [TestMethod]
        public void RemoteTile_Correct_Uri_Set_When_Registration_Completes_With_Tile()
        {
            _mockSettingsProvider.Enabled = false;

            var target = new EntileClient(_mockChannelManager, _mockRegistrationManager, _mockRemoteTileManager, _mockSettingsProvider);

            _mockRegistrationManager.InvokeRegisterCompleted(new RegisterCompletedEventArgs(true, _testTileUri, "EveryHour"));

            Assert.AreEqual(_testTileUri, _mockRemoteTileManager.InvokedTileUri);
        }

        [TestMethod]
        public void RemoteTile_Interval_Set_When_Registration_Completes_With_Tile_EveryHour()
        {
            _mockSettingsProvider.Enabled = false;

            var target = new EntileClient(_mockChannelManager, _mockRegistrationManager, _mockRemoteTileManager, _mockSettingsProvider);

            _mockRegistrationManager.InvokeRegisterCompleted(new RegisterCompletedEventArgs(true, _testTileUri, "EveryHour"));

            Assert.AreEqual(UpdateInterval.EveryHour, _mockRemoteTileManager.InvokedUpdateInterval);
        }

        [TestMethod]
        public void RemoteTile_Interval_Set_When_Registration_Completes_With_Tile_EveryDay()
        {
            _mockSettingsProvider.Enabled = false;

            var target = new EntileClient(_mockChannelManager, _mockRegistrationManager, _mockRemoteTileManager, _mockSettingsProvider);

            _mockRegistrationManager.InvokeRegisterCompleted(new RegisterCompletedEventArgs(true, _testTileUri, "EveryDay"));

            Assert.AreEqual(UpdateInterval.EveryDay, _mockRemoteTileManager.InvokedUpdateInterval);
        }

        [TestMethod]
        public void RemoteTile_Interval_Set_When_Registration_Completes_With_Tile_EveryWeek()
        {
            _mockSettingsProvider.Enabled = false;

            var target = new EntileClient(_mockChannelManager, _mockRegistrationManager, _mockRemoteTileManager, _mockSettingsProvider);

            _mockRegistrationManager.InvokeRegisterCompleted(new RegisterCompletedEventArgs(true, _testTileUri, "EveryWeek"));

            Assert.AreEqual(UpdateInterval.EveryWeek, _mockRemoteTileManager.InvokedUpdateInterval);
        }

/*        [TestMethod]
        public void Busy_Flag_Reset_When_Open_Channel_Complete()
        {
            _mockSettingsProvider.Enabled = false;

            var target = new EntileClient(_mockChannelManager, _mockRegistrationManager, _mockRemoteTileManager, _mockSettingsProvider);

            target.Enable = true;

            _mockChannelManager.InvokeOpenChannelCompleted(new OpenChannelCompletedEventArgs(_testChannelUri));

            Assert.IsFalse(target.Busy);
        }*/

    }
}