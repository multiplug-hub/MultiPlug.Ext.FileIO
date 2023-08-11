using System;

using MultiPlug.Base.Attribute;
using MultiPlug.Base.Exchange;
using MultiPlug.Base.Http;
using MultiPlug.Ext.FileIO.Models;
using MultiPlug.Ext.FileIO.Models.Settings;
using MultiPlug.Ext.FileIO.Components.FileReader;

namespace MultiPlug.Ext.FileIO.Controllers.Settings.Reader
{
    [Route("reader")]
    public class ReaderController : SettingsApp
    {
        public Response Get(ReaderGet theModel)
        {
            FileReaderComponent Reader = null;

            if ( ! string.IsNullOrEmpty(theModel.Id))
            {
                Reader = Core.Instance.FileReaders.Find(t => t.Settings.Guid == theModel.Id);
            }

            FileReaderSettings model;

            if (Reader != null)
            {
                model = Reader.Settings;

                if( ! string.IsNullOrEmpty( theModel.Path ) )
                {
                    model.FilePath = theModel.Path;
                }
            }
            else
            {
                model = new FileReaderSettings
                {
                    Guid = Guid.NewGuid().ToString(),
                    FilePath = string.IsNullOrEmpty(theModel.Path)? "C:\\" : theModel.Path,
                    FileChanged = new Event { Guid = Guid.NewGuid().ToString(), Description = "File Changed Event", Id = Guid.NewGuid().ToString() },
                    nFDN = true,
                    nFFN = true,
                    nFLA = true,
                    nFLW = true,
                    ReadSubscriptions = new Subscription[0],
                    Subject = "value"
                };
            }

            return new Response
            {
                Model = model,
                Template = "GetReaderViewContents"
            };
        }

        public Response Post(ReaderPost theModel)
        {
            Subscription[] Subscriptions;

            if ( theModel.SubscriptionGuid != null)
            {
                Subscriptions = new Subscription[theModel.SubscriptionGuid.Length];

                for( int i = 0; i < theModel.SubscriptionGuid.Length; i++)
                {
                    Subscriptions[i] = new Subscription { Guid = theModel.SubscriptionGuid[i], Id = theModel.SubscriptionId[i] };
                }
            }
            else
            {
                Subscriptions = new Subscription[0];
            }

            Core.Instance.Update(new FileReaderSettings[] { new FileReaderSettings
            {
                ReadSubscriptions = Subscriptions,
                Guid = theModel.guid,
                FilePath = theModel.path,
                nFLA = theModel.nfla,
                nFLW = theModel.nflw,
                nFFN = theModel.nffn,
                nFDN = theModel.nfdn,
                UpdatePart = ( theModel.readaction == "full" )? 0 : theModel.updatepart,
                FileChanged = new Event {Guid = theModel.guid, Id = theModel.eventid, Description = theModel.eventdescription },
                Subject = theModel.subject

            } });

            return new Response { StatusCode = System.Net.HttpStatusCode.Moved, Location = new Uri(Context.Referrer, "?id=" + theModel.guid) };
        }
    }
}
