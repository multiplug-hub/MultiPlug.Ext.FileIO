using System.Linq;
using MultiPlug.Base.Attribute;
using MultiPlug.Base.Http;

namespace MultiPlug.Ext.FileIO.Controllers.Settings.Writer
{
    [Route("writer/delete")]
    public class WriterDeleteController : SettingsApp
    {
        public Response Get(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                var FileWriter = Core.Instance.FileWriters.FirstOrDefault(t => t.Settings.Guid == id);

                if(FileWriter != null)
                {
                    Core.Instance.Changes.EnabledUpdates(false);
                    Core.Instance.Changes.Remove(FileWriter);
                    Core.Instance.FileWriters.Remove(FileWriter);
                    Core.Instance.Changes.EnabledUpdates(true);
                }
            }

            return new Response
            {
                StatusCode = System.Net.HttpStatusCode.Redirect,
                Location = Context.Referrer
            };
        }
    }
}
