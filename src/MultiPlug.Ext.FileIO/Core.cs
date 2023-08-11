using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization;
using MultiPlug.Ext.FileIO.Models;
using MultiPlug.Base.Exchange;
using MultiPlug.Base;
using MultiPlug.Ext.FileIO.Components.Utils;
using MultiPlug.Ext.FileIO.Components.FileWriter;
using MultiPlug.Ext.FileIO.Components.FileReader;

namespace MultiPlug.Ext.FileIO
{
    public class Core : MultiPlugBase
    {
        private ChangeManager m_Changes = new ChangeManager();
        private List<Event> m_Events = new List<Event>();
        private List<Subscription> m_Subscriptions = new List<Subscription>();

        private static Core m_Instance = null;

        public static Core Instance
        {
            get
            {
                if (m_Instance == null)
                {
                    m_Instance = new Core();
                }
                return m_Instance;
            }
        }

        private Core()
        {
            FileWriters = new List<FileWriterComponent>();
            FileReaders = new List<FileReaderComponent>();
        }

        [DataMember]
        public List<FileWriterComponent> FileWriters { get; set; }

        [DataMember]
        public List<FileReaderComponent> FileReaders { get; set; }

        public List<Event> Events
        {
            get
            {
                m_Events.Clear();
                FileReaders.ForEach(d => m_Events.Add(d.Settings.FileChanged));  // Lasy? 
                return m_Events;
            }
        }
        public List<Subscription> Subscriptions
        {
            get
            {
                m_Subscriptions.Clear();
                FileWriters.ForEach(FW => m_Subscriptions.AddRange(FW.Settings.WriteSubscriptions) );  // Lasy? 
                FileReaders.ForEach(FR => m_Subscriptions.AddRange(FR.Settings.ReadSubscriptions) );
                return m_Subscriptions;
            }
        }

        public void Update(FileReaderSettings[] theSettings)
        {
            Changes.EnabledUpdates(false);
            UpdateFileReader(theSettings);
            Changes.EnabledUpdates(true);
        }

        internal void Update(FileWriterSettings[] theSettings)
        {
            Changes.EnabledUpdates(false);
            UpdateFileWriter(theSettings);
            Changes.EnabledUpdates(true);
        }

        internal void Update(FileWriterSettings[] fileWriterSettings, FileReaderSettings[] fileReaderSettings)
        {
            Changes.EnabledUpdates(false);
            UpdateFileReader(fileReaderSettings);
            UpdateFileWriter(fileWriterSettings);
            Changes.EnabledUpdates(true);
        }

        private void UpdateFileReader(FileReaderSettings[] theSettings)
        {
            foreach (var item in theSettings)
            {
                var file = FileReaders.Find(f => f.Settings.Guid == item.Guid);

                if (file != null)
                {
                    file.Stop();
                    file.Apply(item);
                    file.Start();
                }
                else
                {
                    var NewReader = new FileReaderComponent(item);
                    Changes.Add(NewReader);
                    FileReaders.Add(NewReader);
                    NewReader.Start();
                }
            }
        }

        private void UpdateFileWriter(FileWriterSettings[] theSettings)
        {
            foreach (var item in theSettings)
            {
                var file = FileWriters.Find(f => f.Settings.Guid == item.Guid);

                if (file != null)
                {

                    file.Apply(item);
                }
                else
                {
                    var NewWriter = new FileWriterComponent(item);
                    Changes.Add(NewWriter);
                    FileWriters.Add(NewWriter);
                }
            }
        }

        public ChangeManager Changes
        {
            get
            {
                return m_Changes;
            }
        }

        public void Log( Exception ex )
        {
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "error.txt");

            using (StreamWriter writer = new StreamWriter(filePath, true))
            {
                writer.WriteLine("Message :" + ex.Message + " " + System.Environment.NewLine + "StackTrace :" + ex.StackTrace +
                   "" + System.Environment.NewLine + "Date :" + DateTime.Now.ToString());
                writer.WriteLine(System.Environment.NewLine + "-----------------------------------------------------------------------------" + System.Environment.NewLine);
            }
        }
    }
}
