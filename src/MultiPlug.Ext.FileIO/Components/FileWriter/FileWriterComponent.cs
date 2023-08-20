using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization;

using MultiPlug.Ext.FileIO.Models;
using MultiPlug.Base.Exchange;
using System.Text.RegularExpressions;

namespace MultiPlug.Ext.FileIO.Components.FileWriter
{
    public class FileWriterComponent
    {
        private FileWriterSettings m_Settings;

        public event Action SubscriptionsUpdated;

        private string m_WritePrefix = string.Empty;
        private string m_WriteSeparator = string.Empty;
        private string m_WriteSuffix = string.Empty;

        [DataMember]
        public FileWriterSettings Settings
        {
            get
            {
                return m_Settings;
            }
        }

        public FileWriterComponent(string theGuid)
        {
            m_Settings = new FileWriterSettings
            {
                Guid = theGuid,
                FilePath = Path.GetPathRoot(AppDomain.CurrentDomain.BaseDirectory),
                WriteSubscriptions = new Subscription[0],
                Append = false,
                WriteLine = false,
                WritePrefix = string.Empty,
                WriteSeparator = string.Empty,
                WriteSuffix = string.Empty
            };
        }

        internal void UpdateProperties(FileWriterSettings theProperties)
        {
            bool SuUpdated = false;

            if (theProperties.Guid != m_Settings.Guid)
                return;

            if (theProperties.FilePath != null && theProperties.FilePath != m_Settings.FilePath)
            {
                m_Settings.FilePath = theProperties.FilePath;
            }

            if (theProperties.Append != null && theProperties.Append != m_Settings.Append)
            {
                m_Settings.Append = theProperties.Append;
            }
            if(theProperties.WriteLine != null && theProperties.WriteLine != m_Settings.WriteLine)
            {
                m_Settings.WriteLine = theProperties.WriteLine;
            }

            if(theProperties.WritePrefix != null && theProperties.WritePrefix != m_Settings.WritePrefix)
            {
                m_Settings.WritePrefix = theProperties.WritePrefix;
                m_WritePrefix = m_Settings.WritePrefix != null ? Regex.Unescape(m_Settings.WritePrefix) : string.Empty;
            }

            if(theProperties.WriteSeparator != null && theProperties.WriteSeparator != m_Settings.WriteSeparator)
            {
                m_Settings.WriteSeparator = theProperties.WriteSeparator;
                m_WriteSeparator = m_Settings.WriteSeparator != null ? Regex.Unescape(m_Settings.WriteSeparator) : string.Empty;
            }

            if(theProperties.WriteSuffix != null && theProperties.WriteSuffix != m_Settings.WriteSuffix)
            {
                m_Settings.WriteSuffix = theProperties.WriteSuffix;
                m_WriteSuffix = m_Settings.WriteSuffix != null ? Regex.Unescape(m_Settings.WriteSuffix) : string.Empty;
            }

            if (theProperties.WriteSubscriptions != null)
            {
                var StEDeleted = m_Settings.WriteSubscriptions.Where(e => Array.Find(theProperties.WriteSubscriptions, ne => ne.Guid == e.Guid) == null);
                if (StEDeleted.Count() > 0) { SuUpdated = true; }
                m_Settings.WriteSubscriptions = m_Settings.WriteSubscriptions.Except(StEDeleted).ToArray();

                var StENew = theProperties.WriteSubscriptions.Where(e => Array.Find(m_Settings.WriteSubscriptions, ne => ne.Guid == e.Guid) == null);
                if (StENew.Count() > 0) { SuUpdated = true; }
                foreach (var item in StENew) { item.Guid = System.Guid.NewGuid().ToString(); item.Event += OnWriteSubscriptionEvent; }

                var list = new List<Subscription>(m_Settings.WriteSubscriptions);
                list.AddRange(StENew);
                m_Settings.WriteSubscriptions = list.ToArray();
            }

            if (SuUpdated)
            {
                SubscriptionsUpdated?.Invoke();
            }
        }

        private void OnWriteSubscriptionEvent(SubscriptionEvent theSubscriptionEvent)
        {
            string[] AllSubjectValues = theSubscriptionEvent.PayloadSubjects.Select(item => item.Value).ToArray();

            string WriteValue = string.Join(m_WriteSeparator, AllSubjectValues);

            using (StreamWriter Writer = m_Settings.Append.Value? File.AppendText(Settings.FilePath): File.CreateText(Settings.FilePath))
            {
                Writer.Write(m_WritePrefix + WriteValue + m_WriteSuffix);

                if (m_Settings.WriteLine.Value)
                {
                    Writer.Write(System.Environment.NewLine);
                }
            }
        }
    }
}
