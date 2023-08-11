using System.Runtime.Serialization;
using MultiPlug.Ext.FileIO.Models;

namespace MultiPlug.Ext.FileIO.Models.Load
{
    public class FileWriter
    {
        [DataMember]
        public FileWriterSettings Settings { get; set; }
    }
}
