using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Entile.Test.Tests
{
    class MockSettingsProvider : ISettingsProvider
    {
        public bool GetUniqueIdCalled = false;
        public string UniqueIdGenerated;
        public string GetUniqueId()
        {
            GetUniqueIdCalled = true;
            UniqueIdGenerated = Guid.NewGuid().ToString();
            return UniqueIdGenerated;
        }

        public bool Enabled = false;
        public bool GetEnabled()
        {
            return Enabled;
        }

        public void SetEnabled(bool enabled)
        {
            Enabled = enabled;
        }

        public IDictionary<string, string> ExtraInfo;
        public IDictionary<string, string> GetExtraInfo()
        {
            return ExtraInfo;
        }

        public void SetExtraInfo(IDictionary<string, string> extraInfo)
        {
            ExtraInfo = extraInfo;
        }
    }

    class MockWebClient : IWebClient
    {
        public Uri InvokedUri;
        public string InvokedBody;
        public string InvokedContentType;
        public bool UploadStringAsyncCalled = false;

        public void SendStringAsync(Uri uri, string body, string contentType)
        {
            UploadStringAsyncCalled = true;
            InvokedUri = uri;
            InvokedBody = body;
            InvokedContentType = contentType;
        }

        public event EventHandler<SendCompletedEventArgs> SendCompleted;

        public void InvokeSendCompleted(SendCompletedEventArgs e)
        {
            EventHandler<SendCompletedEventArgs> handler = SendCompleted;
            if (handler != null) handler(this, e);
        }
    }

    class MockWebClientFactory : IWebClientFactory
    {
        public MockWebClient CreatedWebClient;
        public IWebClient CreateWebClient()
        {
            return CreatedWebClient = new MockWebClient();
        }
    }

    [TestClass]
    public class RegistrationManagerTest : SilverlightTest
    {
        private MockWebClientFactory _mockWebClientFactory = new MockWebClientFactory();
        private Uri _testNotificationUri = new Uri("http://test.com/Test");
        private Uri _testRegistrationServiceUri = new Uri("http://test.com/Registration");
        private Uri _testTileUri = new Uri("http://test.com/GetTile");

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_RegistrationUriNull_ThrowsException()
        {
            var target = new RegistrationManager(null, "my_id", _mockWebClientFactory);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_UniqueIdProviderNull_ThrowsException()
        {
            var target = new RegistrationManager(_testRegistrationServiceUri, null, _mockWebClientFactory);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_WebClientFactoryNull_ThrowsException()
        {
            var target = new RegistrationManager(_testRegistrationServiceUri, "my_id", null);
        }

        [TestMethod]
        [Asynchronous]
        public void SendCompleted_Invokes_RegisterCompletedEvent()
        {
            var target = new RegistrationManager(_testRegistrationServiceUri, "my_id", _mockWebClientFactory);
            target.RegisterCompleted += (sender, e) =>
                                            {
                                                Assert.AreEqual(true, e.Succeeded);
                                                EnqueueTestComplete();
                                            };
            target.RegisterAsync(_testNotificationUri, null);
            var jsonResult = new RegistrationManager.RemoteTileInfo() { Interval = "EveryHour", Uri = _testTileUri.ToString() }.ToJson();

            _mockWebClientFactory.CreatedWebClient.InvokeSendCompleted(new SendCompletedEventArgs(jsonResult));
        }

        [TestMethod]
        [Asynchronous]
        public void SendCompleted_Failed_Invokes_RegisterCompletedEvent()
        {
            var target = new RegistrationManager(_testRegistrationServiceUri, "my_id", _mockWebClientFactory);
            target.RegisterCompleted += (sender, e) =>
            {
                Assert.AreEqual(false, e.Succeeded);
                EnqueueTestComplete();
            };
            target.RegisterAsync(_testNotificationUri, null);
            _mockWebClientFactory.CreatedWebClient.InvokeSendCompleted(new SendCompletedEventArgs(new WebException("NotFound", WebExceptionStatus.UnknownError)));
        }

        [TestMethod]
        public void Register_Sends_Registration()
        {
            var target = new RegistrationManager(_testRegistrationServiceUri, "my_id", _mockWebClientFactory);
            target.RegisterAsync(_testNotificationUri, null);

            var reg = _mockWebClientFactory.CreatedWebClient.InvokedBody.FromJson<RegistrationManager.RegisterRequest>();
            
            Assert.IsNotNull(reg);
            reg.uniqueId = "my_id";
        }

        [TestMethod]
        public void Register_Sends_To_Registration_Uri()
        {
            var target = new RegistrationManager(_testRegistrationServiceUri, "my_id", _mockWebClientFactory);
            target.RegisterAsync(_testNotificationUri, null);

            var expectedUri = new Uri(_testRegistrationServiceUri + "/" + "Register");
            Assert.AreEqual(expectedUri, _mockWebClientFactory.CreatedWebClient.InvokedUri);
        }
    }
}