using System.Runtime.Serialization;

namespace MultiPlug.Ext.FileIO.Models.Load
{
    public class Root
    {
        [DataMember]
        public FileReader[] FileReaders { get; set; }
        [DataMember]
        public FileWriter[] FileWriters { get; set; }
    }
}
