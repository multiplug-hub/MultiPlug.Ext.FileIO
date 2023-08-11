using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using MultiPlug.Base.Attribute;
using MultiPlug.Base.Exchange;
using MultiPlug.Base.Http;
using MultiPlug.Ext.FileIO.Models;
using MultiPlug.Ext.FileIO.Components.FileWriter;

namespace MultiPlug.Ext.FileIO.Controllers.Settings.Writer
{
    [Route("writer")]
    public class WriterController : SettingsApp
    {
        public Response Get()
        {
            FileWriterComponent writer = null;

            var dic = Context.QueryString.FirstOrDefault(q => q.Key == "id");

            if (!dic.Equals(new KeyValuePair<string, string>()))
            {
                writer = Core.Instance.FileWriters.Find(t => t.Settings.Guid == dic.Value);
            }

            FileWriterSettings model;

            if (writer != null)
            {
                model = writer.Settings;
            }
            else
            {
                model = new FileWriterSettings
                {
                    Guid = Guid.NewGuid().ToString(),
                    WriteSubscriptions = new List<Subscription>(),
                    FilePath = "C:\\",
                    GroupSelectKey = "value"
                };

                Core.Instance.Update(new FileWriterSettings[] { model });
            }

            return new Response
            {
                Model = model,
                Template = "GetWriterViewContents"
            };
        }

        public Response Post()
        {
            var form = Context.FormData;

            var SquareBracketsPattern = @"\[(.*?)\]";
            var SquareBracketsMatchCollection = new List<KeyValuePair<MatchCollection, string>>();

            foreach (var item in form)
            {
                var matches = Regex.Matches(item.Key, SquareBracketsPattern);

                if (matches.Count > 0)
                {
                    SquareBracketsMatchCollection.Add(new KeyValuePair<MatchCollection, string>(matches, item.Value));
                }
            }

            var EventGuid = SquareBracketsMatchCollection.Find(m => m.Key[1].Groups[1].Value == "guid");
            var FileAction = SquareBracketsMatchCollection.Find(m => m.Key[1].Groups[1].Value == "fileaction");
            var SelectKey = SquareBracketsMatchCollection.Find(m => m.Key[1].Groups[1].Value == "groupselectkey");

            if (!EventGuid.Equals(default(KeyValuePair<MatchCollection, string>)))
            {
                Core.Instance.Update(new FileWriterSettings[] { new FileWriterSettings
                {
                    WriteSubscriptions = PopulateSubscriptions(SquareBracketsMatchCollection),
                    Guid = EventGuid.Value,
                    FilePath = null,
                    Append = FileAction.Value == "append" ? true : false,
                    GroupSelectKey = SelectKey.Value,
                    WriteLine = PopulateCheckbox(SquareBracketsMatchCollection, "writeline"),
                } });
                return new Response { Location = new Uri(Context.Referrer, "?id=" + EventGuid.Value), StatusCode = System.Net.HttpStatusCode.Moved };
            }
            else
            {
                return new Response { Location = Context.Referrer, StatusCode = System.Net.HttpStatusCode.Moved };
            }
        }

        private bool PopulateCheckbox(List<KeyValuePair<MatchCollection, string>> theForm, string CheckBoxId)
        {
            var Value = theForm.Find(m => m.Key[1].Groups[1].Value == CheckBoxId);

            return (Value.Equals(default(KeyValuePair<MatchCollection, string>))) ? false : true;
        }

        private List<Subscription> PopulateSubscriptions(List<KeyValuePair<MatchCollection, string>> theForm)
        {
            var Starts = theForm.FindAll(m => m.Key[1].Groups[1].Value == "subid");

            return (Starts != null) ? Starts.Select(d => new Subscription { Guid = d.Key[2].Groups[1].Value, Id = d.Value }).ToList() : new List<Subscription>();
        }
    }
}