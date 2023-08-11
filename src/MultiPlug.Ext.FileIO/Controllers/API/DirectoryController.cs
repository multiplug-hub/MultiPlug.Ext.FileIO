using System.IO;
using System.Linq;

using MultiPlug.Base.Attribute;
using MultiPlug.Base.Http;

namespace MultiPlug.Ext.FileIO.Views.API
{
    [Route("directory/")]
    public class DirectoryController : APIApp
    {
        public Response Get( string path)
        {
            if (path != null)
            {
                var DR = new DirectoryResult
                {
                    path = File.Exists(path)? Path.GetDirectoryName(path) : path
                };

                var Parent = Directory.GetParent(DR.path);

                DR.parent = Parent != null ? Parent.FullName : DR.path;

                if (Directory.Exists(DR.path))
                { 
                    try
                    {
                        DR.directories = Directory.GetDirectories(DR.path);
                    }
                    catch { }

                    try
                    {
                        DR.files = Directory.GetFiles(DR.path);
                    }
                    catch { }
                }

                DriveInfo[] allDrives = DriveInfo.GetDrives();

                DR.drives = allDrives.Select(d => d.Name).ToArray();

                return new Response
                {
                    Model = DR,
                    MediaType = "application/json"
                };
            }
            else
            {
                return new Response
                {
                    StatusCode = System.Net.HttpStatusCode.NotFound
                };
            }
        }
    }

    class DirectoryResult
    {
        public string path { get; set; }
        public string parent { get; set; }
        public string[] directories { get; set; } = new string[0];
        public string[] files { get; set; } = new string[0];
        public string[] drives { get; set; } = new string[0];
    }
}
