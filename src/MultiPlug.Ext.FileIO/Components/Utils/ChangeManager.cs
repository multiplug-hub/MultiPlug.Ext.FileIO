using System;
using MultiPlug.Base;
using MultiPlug.Ext.FileIO.Components.FileWriter;
using MultiPlug.Ext.FileIO.Components.FileReader;

namespace MultiPlug.Ext.FileIO.Components.Utils
{
    public class ChangeManager : MultiPlugBase
    {
        public event EventHandler EventsUpdated;
        public event EventHandler SubscriptionsUpdated;

        private bool m_Enabled = true;
        private bool m_SubscriptionsUpdatesBuffered = false;
        private bool m_EventsUpdatesBuffered = false;

        public void Changes_SubscriptionsUpdated(object sender, EventArgs e)
        {
            if (m_Enabled)
            {
                if (SubscriptionsUpdated != null)
                    SubscriptionsUpdated(this, EventArgs.Empty);
            }
            else
            {
                m_SubscriptionsUpdatesBuffered = true;
            }
        }

        public void Changes_EventsUpdated(object sender, EventArgs e)
        {
            if (m_Enabled)
            {
                if (EventsUpdated != null)
                    EventsUpdated(this, EventArgs.Empty);
            }
            else
            {
                m_EventsUpdatesBuffered = true;
            }
        }

        public void EnabledUpdates(bool Enabled)
        {
            m_Enabled = Enabled;

            if (m_Enabled && m_SubscriptionsUpdatesBuffered)
            {
                m_SubscriptionsUpdatesBuffered = false;
                Changes_SubscriptionsUpdated(this, EventArgs.Empty);
            }

            if (m_Enabled && m_EventsUpdatesBuffered)
            {
                m_EventsUpdatesBuffered = false;
                Changes_EventsUpdated(this, EventArgs.Empty);
            }
        }

        public void Add(FileWriterComponent theWriter)
        {
            theWriter.SubscriptionsUpdated += Changes_SubscriptionsUpdated;

            if (!m_Enabled)
            {
                m_EventsUpdatesBuffered = true;
                m_SubscriptionsUpdatesBuffered = true;
            }
        }

        public void Remove(FileWriterComponent theWriter)
        {
            theWriter.SubscriptionsUpdated -= Changes_SubscriptionsUpdated;
            Changes_SubscriptionsUpdated(this, EventArgs.Empty);
        }

        public void Add(FileReaderComponent theReader)
        {
            theReader.EventUpdated += Changes_EventsUpdated;
            theReader.SubscriptionsUpdated += Changes_SubscriptionsUpdated;

            if (!m_Enabled)
            {
                m_EventsUpdatesBuffered = true;
                m_SubscriptionsUpdatesBuffered = true;
            }
        }

        public void Remove(FileReaderComponent theReader)
        {
            theReader.EventUpdated -= Changes_EventsUpdated;
            theReader.SubscriptionsUpdated -= Changes_SubscriptionsUpdated;

            Changes_EventsUpdated(this, EventArgs.Empty);
        }
    }
}
