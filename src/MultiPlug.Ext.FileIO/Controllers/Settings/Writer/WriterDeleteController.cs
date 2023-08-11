using System.Linq;

using MultiPlug.Base.Attribute;
using MultiPlug.Base.Http;

namespace MultiPlug.Ext.FileIO.Controllers.Settings.Writer
{
    [Route("writer/delete")]
    public class WriterDeleteController : SettingsApp
    {
        public Response Get()
        {
            var dic = Context.QueryString.First(q => q.Key == "id");

            var writer = Core.Instance.FileWriters.Find(t => t.Settings.Guid == dic.Value);

            Core.Instance.Changes.EnabledUpdates(false);
            Core.Instance.Changes.Remove(writer);
            Core.Instance.FileWriters.Remove(writer);
            Core.Instance.Changes.EnabledUpdates(true);

            return new Response
            {
                StatusCode = System.Net.HttpStatusCode.Redirect,
                Location = Context.Referrer
            };
        }
    }
}
