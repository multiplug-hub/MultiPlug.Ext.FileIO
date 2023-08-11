using System.Linq;
using MultiPlug.Base.Attribute;
using MultiPlug.Base.Http;

namespace MultiPlug.Ext.FileIO.Controllers.Settings.Reader
{
    [Route("reader/path")]
    public class ReaderPathController : SettingsApp
    {
        public Response Get(string Id)
        {
       //     var dic = Context.QueryString.FirstOrDefault(q => q.Key == "id");

            var FileReader = Core.Instance.FileReaders.Find(t => t.Settings.Guid == Id);

            //if (FileReader == null)
            //{
            //    return new Response
            //    {
            //        StatusCode = System.Net.HttpStatusCode.NotFound
            //    };
            //}


            var model = new Models.Settings.Path
            {
                Guid = string.IsNullOrEmpty(Id) ? System.Guid.NewGuid().ToString(): Id,
                FilePath = (FileReader == null) ? "C:\\" : FileReader.Settings.FilePath,
                FilePathJsonEncoded = (FileReader == null) ? "C:\\\\" : FileReader.Settings.FilePath.Replace("\\", "\\\\")
             //   BackButton = "reader/?id=" + dic.Valu
            };


            return new Response
            {
                Model = model,
                Template = "GetReaderPathViewContents"
            };
        }

        public Response Post(Models.Settings.Path theModel)
        {
            var FileReader = Core.Instance.FileReaders.Find(t => t.Settings.Guid == theModel.Guid);

            FileReader.Apply(theModel);

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
