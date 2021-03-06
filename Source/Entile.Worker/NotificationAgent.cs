﻿using System;
using System.IO;
using System.Net;
using System.Xml.Linq;
using Entile.Common;

namespace Entile.Worker
{

    class NotificationAgent : INotificationAgent
    {
        enum NotificationKind
        {
            Tile = 1,
            Toast = 2,
            Raw = 3,
        }

        enum NotificationClass
        {
            Immediately = 0,
            Slow = 10,
            VerySlow = 20,
        }

        public NotificationResponse SendNotification(string channelUri, INotificationItem notification)
        {
            NotificationResponse response;
            var tileNotification = notification as TileNotification;
            if (tileNotification != null)
            {
                response = SendTileNotification(channelUri, tileNotification);
                response.NotificationItem = notification;
                return response;
            }

            var toastNotification = notification as ToastNotification;
            if (toastNotification != null)
            {
                response = SendToastNotification(channelUri, toastNotification);
                response.NotificationItem = notification;
                return response;
            }

            var rawNotification = notification as RawNotification;
            if (rawNotification != null)
            {
                response = SendRawNotification(channelUri, rawNotification.Body);
                response.NotificationItem = notification;
                return response;
            }

            return null;

        }

        private NotificationResponse SendTileNotification(string subscriptionUri, TileNotification notification)
        {
            XNamespace wp = "WPNotification";
            XDocument doc = new XDocument(new XDeclaration("1.0", "utf-8", null),
                new XElement(wp + "Notification", new XAttribute(XNamespace.Xmlns + "wp", "WPNotification"),
                    new XElement(wp + "Tile",
                        new XElement(wp + "BackgroundImage",
                            notification.BackgroundUri),
                        new XElement(wp + "Count",
                            notification.Counter),
                        new XElement(wp + "Title",
                            notification.Title)
                        ))
                );
            var payload = doc.Declaration + doc.ToString(SaveOptions.DisableFormatting);
            var notificationMessage = System.Text.Encoding.UTF8.GetBytes(payload);
            return SendRequest(subscriptionUri, notificationMessage, NotificationKind.Tile);
        }

        private NotificationResponse SendToastNotification(string subscriptionUri, ToastNotification notification)
        {
            XNamespace wp = "WPNotification";
            XDocument doc = new XDocument(new XDeclaration("1.0", "utf-8", null),
                new XElement(wp + "Notification", new XAttribute(XNamespace.Xmlns + "wp", "WPNotification"),
                    new XElement(wp + "Toast",
                        new XElement(wp + "Text1",
                            notification.Title),
                        new XElement(wp + "Text2",
                            notification.Body)
                        ))
                );
            var payload = doc.Declaration + doc.ToString(SaveOptions.DisableFormatting);
            var notificationMessage = System.Text.Encoding.UTF8.GetBytes(payload);
            return SendRequest(subscriptionUri, notificationMessage, NotificationKind.Toast);
        }

        private NotificationResponse SendRawNotification(string subscriptionUri, byte[] notification)
        {
            return SendRequest(subscriptionUri, notification, NotificationKind.Raw);
        }

        private NotificationResponse SendRequest(string subscriptionUri, byte[] notificationMessage, NotificationKind kind)
        {
            HttpWebRequest sendNotificationRequest = (HttpWebRequest)WebRequest.Create(subscriptionUri);

            // HTTP POST is the only allowed method to send the notification.
            sendNotificationRequest.Method = "POST";

            // The optional custom header X-MessageID uniquely identifies a notification message. If it is present, the 
            // same value is returned in the notification response. It must be a string that contains a UUID.
            sendNotificationRequest.Headers.Add("X-MessageID", Guid.NewGuid().ToString());
            if (kind == NotificationKind.Tile)
            {
                sendNotificationRequest.ContentType = "text/xml";
                sendNotificationRequest.Headers.Add("X-WindowsPhone-Target", "token");
            }
            else if (kind == NotificationKind.Toast)
            {
                sendNotificationRequest.ContentType = "text/xml";
                sendNotificationRequest.Headers.Add("X-WindowsPhone-Target", "toast");
            }

            // TODO Add support for other notification classes
            var notificationClass = ((int)NotificationClass.Immediately) + (int)kind;
            sendNotificationRequest.Headers.Add("X-NotificationClass", notificationClass.ToString());

            // Sets the web request content length.
            sendNotificationRequest.ContentLength = notificationMessage.Length;

            using (Stream requestStream = sendNotificationRequest.GetRequestStream())
            {
                requestStream.Write(notificationMessage, 0, notificationMessage.Length);
            }

            // Sends the notification and make sure we handle the response eventually
            HttpWebResponse response;
            try
            {
                response = (HttpWebResponse)sendNotificationRequest.GetResponse();
            } 
            catch(WebException ex)
            {
                response = ex.Response as HttpWebResponse;
            }

            var notResponse = new NotificationResponse
                                  {
                                      StatusCode = (int)response.StatusCode,
                                      NotificationStatus = response.Headers["X-NotificationStatus"],
                                      SubscriptionStatus = response.Headers["X-SubscriptionStatus"],
                                      DeviceConnectionStatus = response.Headers["X-DeviceConnectionStatus"]
                                  };

            return notResponse;
        }
    }
}