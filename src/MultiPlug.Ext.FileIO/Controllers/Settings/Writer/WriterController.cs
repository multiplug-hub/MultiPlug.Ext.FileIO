using System;

using MultiPlug.Base.Attribute;
using MultiPlug.Base.Exchange;
using MultiPlug.Base.Http;
using MultiPlug.Ext.FileIO.Models;
using MultiPlug.Ext.FileIO.Components.FileWriter;
using MultiPlug.Ext.FileIO.Models.Settings;

namespace MultiPlug.Ext.FileIO.Controllers.Settings.Writer
{
    [Route("writer")]
    public class WriterController : SettingsApp
    {
        public Response Get(WriterGet theModel)
        {
            FileWriterComponent writer = null;

            if (!string.IsNullOrEmpty(theModel.Id))
            {
                writer = Core.Instance.FileWriters.Find(FileWriter => FileWriter.Settings.Guid == theModel.Id);
            }

            FileWriterSettings ResponseModel;

            if (writer != null)
            {
                ResponseModel = writer.Settings;
            }
            else
            {
                ResponseModel = new FileWriterComponent(Guid.NewGuid().ToString()).Settings;
            }

            if (!string.IsNullOrEmpty(theModel.Path))
            {
                ResponseModel.FilePath = theModel.Path;
            }

            return new Response
            {
                Model = ResponseModel,
                Template = "GetWriterViewContents"
            };
        }

        public Response Post(WriterPost theModel)
        {
            Subscription[] Subscriptions = new Subscription[0];

            if (theModel.SubscriptionGuid != null && theModel.SubscriptionId != null && theModel.SubscriptionGuid.Length == theModel.SubscriptionId.Length)
            {
                Subscriptions = new Subscription[theModel.SubscriptionGuid.Length];
                for (int i = 0; i < theModel.SubscriptionGuid.Length; i++)
                {
                    Subscriptions[i] = new Subscription(theModel.SubscriptionGuid[i], theModel.SubscriptionId[i]);
                }
            }

            Core.Instance.Update(new FileWriterSettings[] { new FileWriterSettings
            {
                WriteSubscriptions = Subscriptions,
                Guid = theModel.Guid,
                FilePath = theModel.FilePath,
                Append = theModel.Append,
                WriteLine = theModel.Writeline,
                WritePrefix = theModel.WritePrefix,
                WriteSeparator = theModel.WriteSeparator,
                WriteSuffix = theModel.WriteSuffix
            } });
            return new Response { Location = new Uri(Context.Referrer, "?id=" + theModel.Guid), StatusCode = System.Net.HttpStatusCode.Moved };
        }
    }
}