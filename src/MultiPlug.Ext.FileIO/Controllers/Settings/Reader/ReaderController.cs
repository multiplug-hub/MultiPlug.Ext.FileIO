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

            FileReaderSettings ResponseModel;

            if (Reader != null)
            {
                ResponseModel = Reader.Settings;
            }
            else
            {
                ResponseModel = new FileReaderComponent(Guid.NewGuid().ToString()).Settings;
            }

            if (!string.IsNullOrEmpty(theModel.Path))
            {
                ResponseModel.FilePath = theModel.Path;
            }

            return new Response
            {
                Model = ResponseModel,
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
                FilePath = theModel.FilePath,
                nFLA = theModel.nfla,
                nFLW = theModel.nflw,
                nFFN = theModel.nffn,
                nFDN = theModel.nfdn,
                UpdatePart = ( theModel.readaction == "full" )? 0 : theModel.updatepart,
                FileChanged = new Event {Guid = theModel.guid, Id = theModel.EventId, Description = theModel.EventDescription, Subjects = new string[] { theModel.EventSubject } }       
            } });

            return new Response { StatusCode = System.Net.HttpStatusCode.Moved, Location = new Uri(Context.Referrer, "?id=" + theModel.guid) };
        }
    }
}
