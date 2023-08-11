using System.Collections.Generic;
using System.Linq;

using MultiPlug.Base.Attribute;
using MultiPlug.Base.Http;

namespace MultiPlug.Ext.FileIO.Views.API
{
    [Route("file/*")]
    public class FileController : APIApp
    {
        public Response Get( string id )
        {
            if (id != string.Empty)
            {
                int lines = 0;
                var Lines = Context.QueryString.FirstOrDefault(q => q.Key == "lines");

                if (!Lines.Equals(new KeyValuePair<string, string>()))
                {
                    int.TryParse(Lines.Value, out lines);
                }

                var f = Core.Instance.FileReaders.Find(t => t.Settings.Guid == id);

                return new Response
                {
                    Model = new { id = f.Settings.Guid, filepath = f.Settings.FilePath, contents = f.Read(lines) },
                    MediaType = "application/json"
                };

            }
            else
            {
                return new Response
                {
                    Model = Core.Instance.FileReaders.Select(f => new { id = f.Settings.Guid, filepath = f.Settings.FilePath }),
                    MediaType = "application/json"
                };
            }
        }
    }
}
