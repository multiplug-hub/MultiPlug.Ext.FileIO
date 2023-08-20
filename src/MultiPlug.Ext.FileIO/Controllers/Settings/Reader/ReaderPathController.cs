using System;
using System.IO;
using MultiPlug.Base.Attribute;
using MultiPlug.Base.Http;
using MultiPlug.Ext.FileIO.Components.FileReader;

namespace MultiPlug.Ext.FileIO.Controllers.Settings.Reader
{
    [Route("reader/path")]
    public class ReaderPathController : SettingsApp
    {
        public Response Get(string Id)
        {
            FileReaderComponent FileReader = null;

            if (! string.IsNullOrEmpty(Id))
            {
                FileReader = Core.Instance.FileReaders.Find(t => t.Settings.Guid == Id);
            }

            var model = new Models.Settings.Path
            {
                Guid = string.IsNullOrEmpty(Id) ? string.Empty: Id,
                FilePath = (FileReader == null) ? Path.GetPathRoot(AppDomain.CurrentDomain.BaseDirectory) : FileReader.Settings.FilePath,
                FilePathJsonEncoded = (FileReader == null) ? Path.GetPathRoot(AppDomain.CurrentDomain.BaseDirectory).Replace("\\", "\\\\") : FileReader.Settings.FilePath.Replace("\\", "\\\\"),
                BackButton = string.IsNullOrEmpty(Id) ? string.Empty : "reader/?id=" + Id
            };

            return new Response
            {
                Model = model,
                Template = "GetReaderPathViewContents"
            };
        }
    }
}
