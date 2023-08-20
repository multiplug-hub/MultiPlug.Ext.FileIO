using MultiPlug.Base;
using MultiPlug.Base.Exchange;
using System.Runtime.Serialization;

namespace MultiPlug.Ext.FileIO.Models
{
    public class FileReaderSettings : MultiPlugBase
    {
        [DataMember]
        public string Guid { get; set; }
        [DataMember]
        public string FilePath { get; set; }
        [DataMember]
        public Event FileChanged { get; set; }
        [DataMember]
        public Subscription[] ReadSubscriptions { get; set; }
        [DataMember]
        public bool? nFLA { get; set; }
        [DataMember]
        public bool? nFLW { get; set; }
        [DataMember]
        public bool? nFFN { get; set; }
        [DataMember]
        public bool? nFDN { get; set; }
        [DataMember]
        public int? UpdatePart { get; set; }
    }
}
