using System.Linq;
using MultiPlug.Base.Attribute;
using MultiPlug.Base.Http;

namespace MultiPlug.Ext.FileIO.Controllers.Settings.Writer
{
    [Route("writer/path")]
    public class WriterPathController : SettingsApp
    {
        public Response Get()
        {
            var dic = Context.QueryString.FirstOrDefault(q => q.Key == "id");

            var FileWriter = Core.Instance.FileWriters.Find(t => t.Settings.Guid == dic.Value);

            if (FileWriter == null)
            {
                return new Response
                {
                    StatusCode = System.Net.HttpStatusCode.NotFound
                };
            }

            var model = new Models.Settings.Path
            {
                Guid = dic.Value,
                FilePath = FileWriter.Settings.FilePath,
                FilePathJsonEncoded = FileWriter.Settings.FilePath.Replace("\\", "\\\\"),
                BackButton = "writer/?id=" + dic.Value
            };


            return new Response
            {
                Model = model,
                Template = "GetWriterPathViewContents"
            };
        }

        public Response Post(Models.Settings.Path theModel)
        {
            var FileWriter = Core.Instance.FileWriters.Find(t => t.Settings.Guid == theModel.Guid);

            FileWriter.Apply(theModel);

            var referrer = Context.Referrer.ToString();
            referrer = referrer.Substring(0, referrer.LastIndexOf('/'));
            referrer = referrer.Substring(0, referrer.LastIndexOf('/'));
            referrer = referrer + "/?id=" + theModel.Guid;

            return new Response
            {
                StatusCode = System.Net.HttpStatusCode.Redirect,
                Location = new System.Uri(referrer)
            };
        }
    }
}
