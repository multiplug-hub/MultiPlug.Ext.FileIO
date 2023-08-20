using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;

using MultiPlug.Base;
using MultiPlug.Base.Exchange;
using MultiPlug.Ext.FileIO.Models;
using MultiPlug.Ext.FileIO.Components.Utils;

namespace MultiPlug.Ext.FileIO.Components.FileReader
{
    public class FileReaderComponent : MultiPlugBase
    {
        public event Action EventUpdated;
        public event Action SubscriptionsUpdated;

        private FileReaderSettings m_Settings;

        private FileSystemWatcher FSW = new FileSystemWatcher();
        private DateTime m_LastWriteTime = DateTime.MinValue;

        [DataMember]
        public FileReaderSettings Settings
        {
            get
            {
                return m_Settings;
            }
        }

        public FileReaderComponent(string theGuid)
        {
            m_Settings = new FileReaderSettings
            {
                Guid = theGuid,
                FilePath = Path.GetPathRoot(AppDomain.CurrentDomain.BaseDirectory),
                FileChanged = new Event { Guid = theGuid, Id = System.Guid.NewGuid().ToString(), Description = "", Subjects = new string[] { "update" }, Group = "File Reader" },
                ReadSubscriptions = new Subscription[0],
                nFDN = true,
                nFFN = true,
                nFLA = true,
                nFLW = true,
                UpdatePart = 0
            };

            if (m_Settings.ReadSubscriptions == null) // Platform should prevent this, but it isn't! TODO.
            {
                m_Settings.ReadSubscriptions = new Subscription[0];
            }

            foreach (var ReadSubscription in m_Settings.ReadSubscriptions)
            {
                ReadSubscription.Event += OnReadSubscriptionEvent;
                ReadSubscription.Guid = Guid.NewGuid().ToString();
            }

            ConfigureWatcher();
        }

        private void OnReadSubscriptionEvent(SubscriptionEvent obj)
        {
            m_Settings.FileChanged.Invoke(new Payload(m_Settings.FileChanged.Id, new PayloadSubject[] { new PayloadSubject(m_Settings.FileChanged.Subjects[0], FileHandler.ReadFileContent(m_Settings.FilePath, m_Settings.UpdatePart.Value)) }));
        }

        internal void UpdateProperties(FileReaderSettings theProperties)
        {
            bool ReConfigure = false;
            bool EvUpdated = false;
            bool SuUpdated = false;

            if (theProperties.Guid != m_Settings.Guid)
            {
                return;
            }

            if (theProperties.FileChanged != null && m_Settings.FileChanged.Id != theProperties.FileChanged.Id)
            {
                m_Settings.FileChanged.Id = theProperties.FileChanged.Id;
                EvUpdated = true;
            }
            if (theProperties.FileChanged != null && m_Settings.FileChanged.Description != theProperties.FileChanged.Description)
            {
                m_Settings.FileChanged.Description = theProperties.FileChanged.Description;
                EvUpdated = true;
            }
            if (theProperties.FilePath != null && m_Settings.FilePath != theProperties.FilePath)
            {
                m_Settings.FilePath = theProperties.FilePath;
                ReConfigure = true;
            }
            if (theProperties.UpdatePart != null && m_Settings.UpdatePart != theProperties.UpdatePart)
            {
                m_Settings.UpdatePart = theProperties.UpdatePart;
                ReConfigure = true;
            }
            if (theProperties.nFLA != null && m_Settings.nFLA != theProperties.nFLA)
            {
                m_Settings.nFLA = theProperties.nFLA;
                ReConfigure = true;
            }
            if (theProperties.nFLW != null && m_Settings.nFLW != theProperties.nFLW)
            {
                m_Settings.nFLW = theProperties.nFLW;
                ReConfigure = true;
            }
            if (theProperties.nFFN != null && m_Settings.nFFN != theProperties.nFFN)
            {
                m_Settings.nFFN = theProperties.nFFN;
                ReConfigure = true;
            }
            if (theProperties.nFDN != null && m_Settings.nFDN != theProperties.nFDN)
            {
                m_Settings.nFDN = theProperties.nFDN;
                ReConfigure = true;
            }

            if(theProperties.FileChanged != null)
            {
                if(Event.Merge(m_Settings.FileChanged, theProperties.FileChanged, true) )
                {
                    EvUpdated = true;
                }
            }

            if(theProperties.ReadSubscriptions != null)
            {
                var EDeleted = m_Settings.ReadSubscriptions.Where(e => Array.Find(theProperties.ReadSubscriptions, ne => ne.Guid == e.Guid) == null);
                if (EDeleted.Count() > 0) { SuUpdated = true; }
                m_Settings.ReadSubscriptions = m_Settings.ReadSubscriptions.Except(EDeleted).ToArray();
            }

            if(theProperties.ReadSubscriptions != null)
            {
                List<Subscription> NewSubscriptionList = new List<Subscription>();
                foreach( var ReadSubscription in theProperties.ReadSubscriptions)
                {
                    Subscription ExistingReadSubscription = Array.Find(m_Settings.ReadSubscriptions, ne => ne.Guid == ReadSubscription.Guid);

                    if( ExistingReadSubscription == null )
                    {
                        ReadSubscription.Guid = System.Guid.NewGuid().ToString();
                        ReadSubscription.Event += OnReadSubscriptionEvent;
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
            }

            if (ReConfigure)
            {
                ConfigureWatcher();
            }

            if (SuUpdated)
            {
                SubscriptionsUpdated?.Invoke();
            }

            if(EvUpdated)
            {
                EventUpdated?.Invoke();
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

            if (m_Settings.nFLA.Value) { FSW.NotifyFilter |= NotifyFilters.LastAccess; }
            if (m_Settings.nFLW.Value) { FSW.NotifyFilter |= NotifyFilters.LastWrite; }
            if (m_Settings.nFFN.Value) { FSW.NotifyFilter |= NotifyFilters.FileName; }
            if (m_Settings.nFDN.Value) { FSW.NotifyFilter |= NotifyFilters.DirectoryName; }
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
                    m_LastWriteTime = lastWriteTime;
                    m_Settings.FileChanged.Invoke( new Payload (m_Settings.FileChanged.Id, new PayloadSubject[] { new PayloadSubject(m_Settings.FileChanged.Subjects[0], FileHandler.ReadFileContent(e.FullPath)) } ) );
                }
                else if (m_Settings.UpdatePart == 1)
                {
                    var lines = Handler.ReadLineWithCheck(e.FullPath);

                    foreach (var line in lines)
                    {
                        m_Settings.FileChanged.Invoke( new Payload( m_Settings.FileChanged.Id, new PayloadSubject[] { new PayloadSubject(m_Settings.FileChanged.Subjects[0], line) } ) );
                    }
                }
                else
                {
                    m_Settings.FileChanged.Invoke( new Payload( m_Settings.FileChanged.Id, new PayloadSubject[] { new PayloadSubject(m_Settings.FileChanged.Subjects[0], FileHandler.ReadFileContent(e.FullPath, m_Settings.UpdatePart.Value)) } ) );
                }
            }
        }

        public string Read( int theLines )
        {
            return ( theLines == 0 ) ? FileHandler.ReadFileContent(m_Settings.FilePath) : FileHandler.ReadFileContent(m_Settings.FilePath, theLines);
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
