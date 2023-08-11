using System.Drawing;
using MultiPlug.Base.Attribute;
using MultiPlug.Base.Http;
using MultiPlug.Ext.FileIO.Properties;

namespace MultiPlug.Ext.FileIO.Controllers.Assets
{
    [Route("images/*")]
    public class ImageController : AssetsHandler
    {
        public Response Get(string theName)
        {
            ImageConverter converter = new ImageConverter();
            return new Response { RawBytes = (byte[])converter.ConvertTo(Resources.file, typeof(byte[])), MediaType = "image/png" };
        }
    }
}
