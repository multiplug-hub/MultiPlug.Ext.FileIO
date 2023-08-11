using System;
using System.Linq;
using System.Collections.Generic;

using MultiPlug.Base.Exchange;
using MultiPlug.Extension.Core;

using MultiPlug.Ext.FileIO.Models;
using MultiPlug.Ext.FileIO.Properties;
using MultiPlug.Extension.Core.Http;

namespace MultiPlug.Ext.FileIO
{
    public class FileIO : MultiPlugExtension
    {
        private Dictionary<string, Payload> m_Settings = new Dictionary<string, Payload>();

        private List<Models.Load.Root> m_LoadQ = new List<Models.Load.Root>();

        private bool m_Running = false;

        public FileIO()
        {
            Core.Instance.Changes.EventsUpdated += Changes_EventsUpdated;
            Core.Instance.Changes.SubscriptionsUpdated += Changes_SubscriptionsUpdated;
        }

        private void Changes_SubscriptionsUpdated(object sender, EventArgs e)
        {
            MultiPlugActions.Extension.Updates.Subscriptions();
        }

        private void Changes_EventsUpdated(object sender, EventArgs e)
        {
            MultiPlugActions.Extension.Updates.Events();
        }

        public override Event[] Events
        {
            get
            {
                return Core.Instance.Events.ToArray();
            }
        }

        public override Subscription[] Subscriptions
        {
            get
            {
                return Core.Instance.Subscriptions.ToArray();
            }
        }

        public override RazorTemplate[] RazorTemplates
        {
            get
            {
                return new RazorTemplate[]
                {
                    new RazorTemplate("MultiPlug.Ext.FileIOFileMonitoring", Resources.index),
                    new RazorTemplate("GetWriterPathViewContents", Resources.Path),
                    new RazorTemplate("GetReaderPathViewContents", Resources.Path),
                    new RazorTemplate("GetWriterViewContents", Resources.Writer),
                    new RazorTemplate("GetReaderViewContents", Resources.Reader),
                    new RazorTemplate("FileSettingsView", Resources.Home)
                };
            }
        }

        public void OnError(string theValue)
        {
            throw new NotImplementedException();
        }

        public override void Start()
        {
            try
            {
                if (!m_Running)
                {
                    Core.Instance.FileReaders.ForEach(FR => FR.Start());
                    m_Running = true;
                }
            }
            catch( Exception ex)
            {
                Core.Instance.Log(ex);
            }
        }

        public override void Shutdown()
        {
            try
            {
                if (m_Running)
                {
                    Core.Instance.FileReaders.ForEach(FR => FR.Stop());
                    m_Running = false;
                }
            }
            catch( Exception ex)
            {
                Core.Instance.Log(ex);
            }
}

        public override object Save()
        {
            try
            {
                return Core.Instance;
            }
            catch( Exception ex)
            {
                Core.Instance.Log(ex);
                return null;
            }
        }

        public void Load(Models.Load.Root config)
        {
            m_LoadQ.Add(config);
        }

        public override void Initialise()
        {
            try
            {
                m_LoadQ.ForEach(LoadObject => {

                    Core.Instance.Update(
                    LoadObject.FileWriters != null ? LoadObject.FileWriters.Select(FileWriter => FileWriter.Settings).ToArray() : new FileWriterSettings[0],
                    LoadObject.FileReaders != null ? LoadObject.FileReaders.Select(FileReader => FileReader.Settings).ToArray() : new FileReaderSettings[0]
                    );
                }
                );

                m_LoadQ.Clear();
            }
            catch( Exception ex)
            {
                Core.Instance.Log(ex);
            }
        }
    }
}
