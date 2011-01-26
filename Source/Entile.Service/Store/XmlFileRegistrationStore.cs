using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Hosting;
using System.Xml.Linq;
using Entile.Common;

namespace Entile.Service.Store
{
    public class XmlFileRegistrationStore : IRegistrationStore
    {
        private string _fileName;
        private object _lock = new object();

        public XmlFileRegistrationStore()
        {
            _fileName = HostingEnvironment.MapPath("~/App_Data/EntileRegistrations.xml");
        }

        public XmlFileRegistrationStore(string name)
        {
            _fileName = HostingEnvironment.MapPath(string.Format("~/App_Data/{0}.xml", name));
        }

        public void AddRegistration(Registration registration)
        {
            lock(_lock)
            {
                XDocument doc = GetOrCreateDocument();
            
                var registrationsNode = doc.Element("Registrations");
                registrationsNode.Add(new XElement("Registration", 
                    new XAttribute("NotificationChannel", registration.NotificationChannel),
                    new XAttribute("UniqueId", registration.UniqueId)));

                SaveDocument(doc);
            }
        }

        public void RemoveRegistration(string uniqueId)
        {
            lock (_lock)
            {
                var doc = GetOrCreateDocument();
                var registrationsNode = doc.Element("Registrations");
                var node = from reg in registrationsNode.Elements("Registration")
                           where reg.Attribute("UniqueId").Value == uniqueId
                           select reg;
                node.Remove();

                SaveDocument(doc);
            }
        }

        public void UpdateRegistration(Registration registration)
        {
            lock (_lock)
            {
                var doc = GetOrCreateDocument();
                var registrationsNode = doc.Element("Registrations");
                var node = (from reg in registrationsNode.Elements("Registration")
                            where reg.Attribute("UniqueId").Value == registration.UniqueId
                            select reg).FirstOrDefault();

                if (node != null)
                {
                    node.Attribute("NotificationChannel").Value = registration.NotificationChannel;
                }
                else
                {
                    registrationsNode.Add(new XElement("Registration",
                        new XAttribute("NotificationChannel", registration.NotificationChannel),
                        new XAttribute("UniqueId", registration.UniqueId)));
                }
                SaveDocument(doc);
            }
        }

        public IEnumerable<Registration> ListAllRegistrations()
        {
            var doc = GetOrCreateDocument();
            var registrationsNode = doc.Element("Registrations");

            return from reg in registrationsNode.Elements("Registration")
                   let content = reg.Element("Content")
                   select
                       new Registration()
                       {
                           UniqueId = reg.Attribute("UniqueId").Value,
                           NotificationChannel = reg.Attribute("NotificationChannel").Value
                       };
        }

        public Registration GetRegistration(string uniqueId)
        {
            var doc = GetOrCreateDocument();
            var registrationsNode = doc.Element("Registrations");

            return (from reg in registrationsNode.Elements("Registration")
                    let uid = reg.Attribute("UniqueId").Value
                    where uid == uniqueId
                    select
                        new Registration()
                        {
                            UniqueId = uid,
                            NotificationChannel = reg.Attribute("NotificationChannel").Value
                        }).FirstOrDefault();
        }

        public void UpdateExtraInfo(string uniqueId, IDictionary<string, string> extraInfo)
        {
            lock(_lock)
            {
                var doc = GetOrCreateDocument();
                var registrationsNode = doc.Element("Registrations");
                var node = (from reg in registrationsNode.Elements("Registration")
                            where reg.Attribute("UniqueId").Value == uniqueId
                            select reg).FirstOrDefault();
                if (node != null)
                {
                    node.Elements("ExtraInfo").Remove();
                    foreach(var entry in extraInfo)
                    {
                        node.Add(new XElement("ExtraInfo", new XAttribute("Key", entry.Key), new XCData(entry.Value)));
                    }
                    SaveDocument(doc);
                }
            }
        }

        public IDictionary<string, string> GetExtraInfo(string uniqueId)
        {
            var result = new Dictionary<string, string>();
            var doc = GetOrCreateDocument();
            var registrationsNode = doc.Element("Registrations");
            var node = (from reg in registrationsNode.Elements("Registration")
                        where reg.Attribute("UniqueId").Value == uniqueId
                        select reg).FirstOrDefault();
            if (node != null)
            {
                foreach (var extraInfo in node.Elements("ExtraInfo"))
                {
                    result[extraInfo.Attribute("Key").Value] = extraInfo.Value;
                }
            }
            return result;
        }

        public void RemoveExtraInfo(string uniqueId)
        {
            lock (_lock)
            {
                var doc = GetOrCreateDocument();
                var registrationsNode = doc.Element("Registrations");
                var node = (from reg in registrationsNode.Elements("Registration")
                            where reg.Attribute("UniqueId").Value == uniqueId
                            select reg).FirstOrDefault();
                if (node != null)
                {
                    node.Elements("ExtraInfo").Remove();
                    SaveDocument(doc);
                }
            }
        }

        private XDocument GetOrCreateDocument()
        {
            XDocument doc;
            if (!File.Exists(_fileName))
            {
                string directoryName = Path.GetDirectoryName(_fileName);
                if (!Directory.Exists(directoryName))
                    Directory.CreateDirectory(directoryName);
                doc = new XDocument(
                    new XElement("Registrations")
                    );

            }
            else
            {
                doc = XDocument.Load(_fileName);
            }
            return doc;
        }

        private void SaveDocument(XDocument doc)
        {
            doc.Save(_fileName);
        }

    }
}