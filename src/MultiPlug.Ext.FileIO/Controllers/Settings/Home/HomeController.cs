using MultiPlug.Base.Attribute;
using MultiPlug.Base.Http;

namespace MultiPlug.Ext.FileIO.Controllers.Settings.Home
{
    [Route("")]
    class HomeController : SettingsApp
    {
        public Response Get()
        {
            return new Response
            {
                Model = new Models.Settings.Files { FileReaders = Core.Instance.FileReaders, FileWriters = Core.Instance.FileWriters },
                Template = "FileSettingsView"
            };
        }
    }
}


