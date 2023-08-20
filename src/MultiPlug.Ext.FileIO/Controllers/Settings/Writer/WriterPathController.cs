using System;
using System.IO;
using MultiPlug.Base.Attribute;
using MultiPlug.Base.Http;
using MultiPlug.Ext.FileIO.Components.FileWriter;

namespace MultiPlug.Ext.FileIO.Controllers.Settings.Writer
{
    [Route("writer/path")]
    public class WriterPathController : SettingsApp
    {
        public Response Get(string Id)
        {
            FileWriterComponent FileWriter = null;

            if (!string.IsNullOrEmpty(Id))
            {
                FileWriter = Core.Instance.FileWriters.Find(t => t.Settings.Guid == Id);
            }

            var model = new Models.Settings.Path
            {
                Guid = string.IsNullOrEmpty(Id) ? string.Empty : Id,
                FilePath = (FileWriter == null) ? Path.GetPathRoot(AppDomain.CurrentDomain.BaseDirectory) : FileWriter.Settings.FilePath,
                FilePathJsonEncoded = (FileWriter == null) ? Path.GetPathRoot(AppDomain.CurrentDomain.BaseDirectory).Replace("\\", "\\\\") : FileWriter.Settings.FilePath.Replace("\\", "\\\\"),
                BackButton = string.IsNullOrEmpty(Id) ? string.Empty : "writer/?id=" + Id
            };

            return new Response
            {
                Model = model,
                Template = "GetWriterPathViewContents"
            };
        }
    }
}
