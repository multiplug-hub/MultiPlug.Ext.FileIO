using System.Linq;
using MultiPlug.Base.Attribute;
using MultiPlug.Base.Http;

namespace MultiPlug.Ext.FileIO.Controllers.Settings.Reader
{
    [Route("reader/delete")]
    public class ReaderDeleteController : SettingsApp
    {
        public Response Get()
        {
            var dic = Context.QueryString.First(q => q.Key == "id");

            var reader = Core.Instance.FileReaders.Find(t => t.Settings.Guid == dic.Value);

            Core.Instance.Changes.EnabledUpdates(false);
            Core.Instance.Changes.Remove(reader);
            Core.Instance.FileReaders.Remove(reader);
            Core.Instance.Changes.EnabledUpdates(true);

            return new Response
            {
                StatusCode = System.Net.HttpStatusCode.Redirect,
                Location = Context.Referrer
            };
        }
    }
}
