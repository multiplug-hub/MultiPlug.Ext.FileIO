using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;

using MultiPlug.Ext.FileIO.Models;
using MultiPlug.Base;
using MultiPlug.Base.Exchange;

using MultiPlug.Ext.FileIO.Components.Utils;

namespace MultiPlug.Ext.FileIO.Components.FileReader
{
    public class FileReaderComponent : MultiPlugBase
    {
        public event EventHandler EventUpdated;
        public event EventHandler SubscriptionsUpdated;

        private FileReaderSettings m_Settings = new FileReaderSettings { Guid = System.Guid.NewGuid().ToString() };

        private FileSystemWatcher FSW = new FileSystemWatcher();

        private SubscriptionsHandler m_SubscriptionsHandler;
        private EventsHandler m_EventHandlers;

        private DateTime m_LastWriteTime = DateTime.MinValue;

        [DataMember]
        public FileReaderSettings Settings
        {
            get
            {
                return m_Settings;
            }
        }

        public FileReaderComponent() : this(new FileReaderSettings
        {
            Guid = System.Guid.NewGuid().ToString(),
            FilePath = "",
            FileChanged = new Event { Guid = System.Guid.NewGuid().ToString(), Id = System.Guid.NewGuid().ToString(), Description = "" },
            ReadSubscriptions = new Subscription[0],
            nFDN = true,
            nFFN = true,
            nFLA = true,
            nFLW = true,
            Subject = "update"
        })
        { }

        public FileReaderComponent(FileReaderSettings theSettings)
        {
            m_Settings = theSettings;

            m_EventHandlers = new EventsHandler(m_Settings);
            m_SubscriptionsHandler = new SubscriptionsHandler(m_Settings, m_EventHandlers);


            m_Settings.FileChanged.Object = m_EventHandlers;

            if(m_Settings.ReadSubscriptions == null) // Platform should prevent this, but it isn't! TODO.
            {
                m_Settings.ReadSubscriptions = new Subscription[0];
            }

            foreach( var ReadSubscription in m_Settings.ReadSubscriptions)
            {
                ReadSubscription.EventConsumer = m_SubscriptionsHandler;
                ReadSubscription.Guid = Guid.NewGuid().ToString();
            }

            ConfigureWatcher();
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
                ConfigureWatcher();
            }
        }

        internal void Apply(FileReaderSettings theNewSettings)
        {
            bool ReConfigure = false;
            bool EvUpdated = false;
            bool SuUpdated = false;

            if (theNewSettings.Guid != m_Settings.Guid)
            {
                return;
            }

            if (m_Settings.FileChanged.Id != theNewSettings.FileChanged.Id)
            {
                m_Settings.FileChanged.Id = theNewSettings.FileChanged.Id;
                EvUpdated = true;
            }
            if (m_Settings.FileChanged.Description != theNewSettings.FileChanged.Description)
            {
                m_Settings.FileChanged.Description = theNewSettings.FileChanged.Description;
                EvUpdated = true;
            }
            if (theNewSettings.FilePath != null && m_Settings.FilePath != theNewSettings.FilePath)
            {
                m_Settings.FilePath = theNewSettings.FilePath;
                ReConfigure = true;
            }
            if (m_Settings.UpdatePart != theNewSettings.UpdatePart)
            {
                m_Settings.UpdatePart = theNewSettings.UpdatePart;
                ReConfigure = true;
            }
            if (m_Settings.nFLA != theNewSettings.nFLA)
            {
                m_Settings.nFLA = theNewSettings.nFLA;
                ReConfigure = true;
            }
            if (m_Settings.nFLW != theNewSettings.nFLW)
            {
                m_Settings.nFLW = theNewSettings.nFLW;
                ReConfigure = true;
            }
            if (m_Settings.nFFN != theNewSettings.nFFN)
            {
                m_Settings.nFFN = theNewSettings.nFFN;
                ReConfigure = true;
            }
            if (m_Settings.nFDN != theNewSettings.nFDN)
            {
                m_Settings.nFDN = theNewSettings.nFDN;
                ReConfigure = true;
            }
            if (m_Settings.Subject != theNewSettings.Subject)
            {
                m_Settings.Subject = theNewSettings.Subject;
            }

            var EDeleted = m_Settings.ReadSubscriptions.Where(e => Array.Find(theNewSettings.ReadSubscriptions, ne => ne.Guid == e.Guid) == null);
            if (EDeleted.Count() > 0) { SuUpdated = true; }
            m_Settings.ReadSubscriptions = m_Settings.ReadSubscriptions.Except(EDeleted).ToArray();


            List<Subscription> NewSubscriptionList = new List<Subscription>();
            foreach( var ReadSubscription in theNewSettings.ReadSubscriptions)
            {
                Subscription ExistingReadSubscription = Array.Find(m_Settings.ReadSubscriptions, ne => ne.Guid == ReadSubscription.Guid);

                if( ExistingReadSubscription == null )
                {
                    ReadSubscription.Guid = System.Guid.NewGuid().ToString();
                    ReadSubscription.EventConsumer = m_SubscriptionsHandler;
                    NewSubscriptionList.Add(ReadSubscription);
                    SuUpdated = true;
                }
                else
                {
                    if( ReadSubscription.Id != ExistingReadSubscription.Id)
                    {
                        ExistingReadSubscription.Id = ReadSubscription.Id;
                        SuUpdated = true;
                    }
                }
            }

            if( NewSubscriptionList.Any() )
            {
                var NewSubscriptionArray = NewSubscriptionList.ToArray();
                Subscription[] NewReadSubscriptions = new Subscription[m_Settings.ReadSubscriptions.Length + NewSubscriptionArray.Length];
                Array.Copy(m_Settings.ReadSubscriptions, NewReadSubscriptions, m_Settings.ReadSubscriptions.Length);
                Array.Copy(NewSubscriptionArray, 0, NewReadSubscriptions, m_Settings.ReadSubscriptions.Length, NewSubscriptionArray.Length);
                m_Settings.ReadSubscriptions = NewReadSubscriptions;
            }

            if (ReConfigure)
            {
                ConfigureWatcher();
            }

            if (SuUpdated && SubscriptionsUpdated != null)
            {
                SubscriptionsUpdated(this, EventArgs.Empty);
            }

            if( EvUpdated && EventUpdated != null)
            {
                EventUpdated(this, EventArgs.Empty);
            }
        }

        private void ConfigureWatcher()
        {
            if(m_Settings.FilePath == "")
            {
                return;
            }

            int idx = m_Settings.FilePath.LastIndexOf('\\');

            if(idx < 1)
            {
                return;
            }

            string stringdd = m_Settings.FilePath.Substring(0, idx);

            FSW.Path = m_Settings.FilePath.Substring(0, idx) + "\\";
            FSW.Filter = m_Settings.FilePath.Substring(idx + 1);

            if (m_Settings.nFLA) { FSW.NotifyFilter |= NotifyFilters.LastAccess; }
            if (m_Settings.nFLW) { FSW.NotifyFilter |= NotifyFilters.LastWrite; }
            if (m_Settings.nFFN) { FSW.NotifyFilter |= NotifyFilters.FileName; }
            if (m_Settings.nFDN) { FSW.NotifyFilter |= NotifyFilters.DirectoryName; }
        }

        public void Stop()
        {
            if (FSW != null && FSW.EnableRaisingEvents)
            {
                FSW.EnableRaisingEvents = false;
                FSW.Changed -= new FileSystemEventHandler(OnFileChanged);
            }
        }

        public void Start()
        {
            if (FSW != null && (!FSW.EnableRaisingEvents))
            {
                try
                {
                    FSW.EnableRaisingEvents = true;
                    FSW.Changed += new FileSystemEventHandler(OnFileChanged);
                }
                catch
                {

                }
            }
        }

        private FileHandler Handler = new FileHandler();

        public void OnFileChanged(object source, FileSystemEventArgs e)
        {
            DateTime lastWriteTime = File.GetLastWriteTime(e.FullPath);

            if (lastWriteTime != m_LastWriteTime)
            {
                Trace.WriteLine("Info: A file was changed at " + e.FullPath);

                m_LastWriteTime = lastWriteTime;

                if(m_Settings.UpdatePart == 0)
                {
                    var list = new List<Pair>(1);
                    list.Add(new Pair( string.Copy(m_Settings.Subject), FileHandler.ReadFileContent(e.FullPath )));

                    m_LastWriteTime = lastWriteTime;

                    m_EventHandlers.FireUpdate(new Payload (m_Settings.FileChanged.Id, list.ToArray() ) );
                }
                else if (m_Settings.UpdatePart == 1)
                {
                    var lines = Handler.ReadLineWithCheck(e.FullPath);

                    foreach (var line in lines)
                    {
                        var plist = new List<Pair>(1);
                        plist.Add(new Pair(string.Copy(m_Settings.Subject), line ));

                        m_EventHandlers.FireUpdate(new Payload( m_Settings.FileChanged.Id, plist.ToArray() ) );
                    }
                }
                else
                {
                    var list = new Pair[]
                    {   new Pair( string.Copy(m_Settings.Subject), FileHandler.ReadFileContent(e.FullPath, m_Settings.UpdatePart))
                    };

                    m_EventHandlers.FireUpdate( new Payload( m_Settings.FileChanged.Id, list ) );
                }
            }
        }

        public string Read( int theLines )
        {
            return ( theLines == 0 ) ? FileHandler.ReadFileContent(m_Settings.FilePath) : FileHandler.ReadFileContent(m_Settings.FilePath, theLines);
        }

        class EventsHandler : EventableBase
        {
            private FileReaderSettings m_Settings;

            public EventsHandler(FileReaderSettings theSettings)
            {
                this.m_Settings = theSettings;
            }

            public override Payload CachedValue()
            {
                throw new NotImplementedException();
            }

            public void FireUpdate(Payload theGroup)
            {
                Update?.Invoke(theGroup);
            }
        }

        class SubscriptionsHandler : EventConsumer
        {
            private EventsHandler m_EventHandlers;
            private FileReaderSettings m_Settings;

            public SubscriptionsHandler(FileReaderSettings theSettings, EventsHandler theEventHandlers)
            {
                this.m_EventHandlers = theEventHandlers;
                this.m_Settings = theSettings;
            }

            public override void OnEvent(Payload e)
            {

                var list = new Pair[]
                { new Pair( string.Copy(m_Settings.Subject), FileHandler.ReadFileContent(m_Settings.FilePath, m_Settings.UpdatePart) )
                };

                m_EventHandlers.FireUpdate(new Payload( m_Settings.FileChanged.Id, list ));
            }
        }

        class FileHandler
        {
            string m_LastLineRead = null;


            public List<string> ReadLineWithCheck(string thePath)
            {
                List<string> ReadLines = new List<string>();

                CReverseLineReader LineReader;

                using (var stream = new FileStream(thePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    Func<Stream> FileStreamFunc = () => stream;

                    LineReader = new CReverseLineReader(FileStreamFunc);

                    IEnumerator<string> Enumerator = LineReader.GetEnumerator();

                    while (Enumerator.MoveNext())
                    {
                        if (ReadLines.Count == 0)
                        {
                            ReadLines.Add(Enumerator.Current);
                        }
                        else
                        {
                            if( m_LastLineRead == null)
                            {
                                break;
                            }
                            else if (Enumerator.Current != m_LastLineRead)
                            {
                                ReadLines.Add(Enumerator.Current);
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                    if (ReadLines.Count > 0)
                    {
                        m_LastLineRead = ReadLines[0];
                    }
                }

                ReadLines.Reverse();

                return ReadLines;
            }

            public static string ReadFileContent(string thePath)
            {
                string Result;

                var fs = new FileStream(thePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                using (StreamReader sr = new StreamReader(fs))
                {
                    Result = sr.ReadToEnd();
                }

                return Result;
            }

            public static string ReadFileContent(string thePath, int lines)
            {
                string Result;

                CReverseLineReader LineReader;
                List<string> ReadLines = new List<string>(lines);

                using (var stream = new FileStream(thePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    Func<Stream> FileStreamFunc = () => stream;

                    LineReader = new CReverseLineReader(FileStreamFunc);

                    IEnumerator<string> Enumerator = LineReader.GetEnumerator();
                    int LineCount = 0;
                    while (Enumerator.MoveNext())
                    {
                        ReadLines.Add(Enumerator.Current);
                        LineCount++;
                        if (LineCount == lines)
                        {
                            break;
                        }
                    }
                }

                ReadLines.Reverse();

                Result = string.Join(System.Environment.NewLine, ReadLines.ToArray());

                return Result;
            }
        }
    }
}
