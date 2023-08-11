using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization;

using MultiPlug.Ext.FileIO.Models;
using MultiPlug.Base.Exchange;

namespace MultiPlug.Ext.FileIO.Components.FileWriter
{
    public class FileWriterComponent : EventConsumer
    {
        private FileWriterSettings m_Settings;

        public event EventHandler SubscriptionsUpdated;

        [DataMember]
        public FileWriterSettings Settings
        {
            get
            {
                return m_Settings;
            }
        }

        public FileWriterComponent() : this( new FileWriterSettings
        {
            Guid = Guid.NewGuid().ToString(),
            FilePath = "",
            WriteSubscriptions = new List<Subscription>(),
            GroupSelectKey = "update",
            Append = false
        }) { }

        public FileWriterComponent(FileWriterSettings theSettings)
        {
            m_Settings = theSettings;

            if(m_Settings.WriteSubscriptions == null)  // Platform should prevent this, but it isn't! TODO.
            {
                m_Settings.WriteSubscriptions = new List<Subscription>();
            }

            m_Settings.WriteSubscriptions.ForEach(sub => 
            {
                sub.EventConsumer = this;
                sub.Guid = Guid.NewGuid().ToString();
            });
        }

        internal void Apply(Models.Settings.Path theNewSettings)
        {
            if (theNewSettings.Guid != m_Settings.Guid)
            {
                return;
            }

            if (m_Settings.FilePath != theNewSettings.FilePath)
            {
                m_Settings.FilePath = theNewSettings.FilePath;
            }
        }

        internal void Apply(FileWriterSettings theNewData)
        {
            bool SuUpdated = false;

            if (theNewData.Guid != m_Settings.Guid)
                return;

            if (theNewData.FilePath != null && theNewData.FilePath != m_Settings.FilePath)
            {
                m_Settings.FilePath = theNewData.FilePath;
            }

            if (theNewData.GroupSelectKey != m_Settings.GroupSelectKey)
            {
                m_Settings.GroupSelectKey = theNewData.GroupSelectKey;
            }

            if (theNewData.Append != m_Settings.Append)
            {
                m_Settings.Append = theNewData.Append;
            }
            if( theNewData.WriteLine != m_Settings.WriteLine)
            {
                m_Settings.WriteLine = theNewData.WriteLine;
            }

            var StEDeleted = m_Settings.WriteSubscriptions.Where(e => theNewData.WriteSubscriptions.Find(ne => ne.Guid == e.Guid) == null);
            if (StEDeleted.Count() > 0) { SuUpdated = true; }
            m_Settings.WriteSubscriptions = m_Settings.WriteSubscriptions.Except(StEDeleted).ToList();

            var StENew = theNewData.WriteSubscriptions.Where(e => m_Settings.WriteSubscriptions.Find(ne => ne.Guid == e.Guid) == null);
            if (StENew.Count() > 0) { SuUpdated = true; }
            foreach (var item in StENew) { item.Guid = System.Guid.NewGuid().ToString(); item.EventConsumer = this; }
            m_Settings.WriteSubscriptions.AddRange(StENew);

            if (SuUpdated && SubscriptionsUpdated != null)
            {
                SubscriptionsUpdated(this, EventArgs.Empty);
            }
        }

        public override void OnEvent(Payload e)
        {
            var value = e.Pairs.ToList().Find(p => p.Type.ToLower() == m_Settings.GroupSelectKey.ToLower());

            if (value != null)
            {
                if (m_Settings.Append)
                {
                    using (var w = File.AppendText(Settings.FilePath))
                    {
                        w.Write((m_Settings.WriteLine) ? value.Value + System.Environment.NewLine : value.Value);
                    }
                }
                else
                {
                    using (var w = File.CreateText(Settings.FilePath))
                    {
                        w.Write((m_Settings.WriteLine)? value.Value + System.Environment.NewLine : value.Value);
                    }
                }

            }
        }
    }
}
