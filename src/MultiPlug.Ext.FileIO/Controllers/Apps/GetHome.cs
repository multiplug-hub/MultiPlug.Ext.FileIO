using System.Collections.Generic;

using MultiPlug.Base.Exchange;
using MultiPlug.Base.Http;
using MultiPlug.Base.Attribute;
using MultiPlug.Ext.FileIO.Models;

namespace MultiPlug.Ext.FileIO.Controllers.Apps
{
    [Route("")]
    public class GetHome : CStatusView
    {
        private List<Subscription> m_Subscriptions = null;

        public Response Get()
        {
            var list = new List<KeyValuePair<string, string>>();
            Core.Instance.FileReaders.ForEach(Reader => list.Add(new KeyValuePair<string, string>(Reader.Settings.FileChanged.Id, Reader.Settings.FilePath)));

            if (m_Subscriptions == null)
            {
                m_Subscriptions = new List<Subscription>();

                Core.Instance.FileReaders.ForEach(Reader => m_Subscriptions.Add(new Subscription { Id = Reader.Settings.FileChanged.Id }));
            }


            var model = new ViewModel { Name = "File Monitor", Files = list };

            return new Response
            {
                Model = model,
                Subscriptions = m_Subscriptions,
                Template = "MultiPlug.Ext.FileIOFileMonitoring"
            };
        }
    }
}
