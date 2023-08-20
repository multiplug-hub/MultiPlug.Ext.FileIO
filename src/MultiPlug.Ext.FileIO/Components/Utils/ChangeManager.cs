using System;
using MultiPlug.Base;
using MultiPlug.Ext.FileIO.Components.FileWriter;
using MultiPlug.Ext.FileIO.Components.FileReader;

namespace MultiPlug.Ext.FileIO.Components.Utils
{
    public class ChangeManager : MultiPlugBase
    {
        public event Action EventsUpdated;
        public event Action SubscriptionsUpdated;

        private bool m_Enabled = true;
        private bool m_SubscriptionsUpdatesBuffered = false;
        private bool m_EventsUpdatesBuffered = false;

        public void Changes_SubscriptionsUpdated()
        {
            if (m_Enabled)
            {
                if (SubscriptionsUpdated != null)
                    SubscriptionsUpdated();
            }
            else
            {
                m_SubscriptionsUpdatesBuffered = true;
            }
        }

        public void Changes_EventsUpdated()
        {
            if (m_Enabled)
            {
                if (EventsUpdated != null)
                    EventsUpdated();
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
                Changes_SubscriptionsUpdated();
            }

            if (m_Enabled && m_EventsUpdatesBuffered)
            {
                m_EventsUpdatesBuffered = false;
                Changes_EventsUpdated();
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
            Changes_SubscriptionsUpdated();
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

            Changes_EventsUpdated();
        }
    }
}
