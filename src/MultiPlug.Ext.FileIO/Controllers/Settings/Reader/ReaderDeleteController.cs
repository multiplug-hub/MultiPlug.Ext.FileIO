using System.Linq;
using MultiPlug.Base.Attribute;
using MultiPlug.Base.Http;

namespace MultiPlug.Ext.FileIO.Controllers.Settings.Reader
{
    [Route("reader/delete")]
    public class ReaderDeleteController : SettingsApp
    {
        public Response Get(string id)
        {
            if(!string.IsNullOrEmpty(id))
            {
                var FileReader = Core.Instance.FileReaders.FirstOrDefault(t => t.Settings.Guid == id);

                if(FileReader != null)
                {
                    Core.Instance.Changes.EnabledUpdates(false);
                    Core.Instance.Changes.Remove(FileReader);
                    Core.Instance.FileReaders.Remove(FileReader);
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
