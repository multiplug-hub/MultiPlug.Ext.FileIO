using MultiPlug.Ext.FileIO.Models;
using System.Runtime.Serialization;

namespace MultiPlug.Ext.FileIO.Models.Load
{
    public class FileReader
    {
        [DataMember]
        public FileReaderSettings Settings { get; set; }
    }
}
