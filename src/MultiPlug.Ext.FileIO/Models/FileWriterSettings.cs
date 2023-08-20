using MultiPlug.Base;
using MultiPlug.Base.Exchange;
using System.Runtime.Serialization;

namespace MultiPlug.Ext.FileIO.Models
{
    public class FileWriterSettings : MultiPlugBase
    {
        [DataMember]
        public string Guid { get; set; }
        [DataMember]
        public string FilePath { get; set; }
        [DataMember]
        public Subscription[] WriteSubscriptions { get; set; }
        [DataMember]
        public bool? Append { get; set; }
        [DataMember]
        public bool? WriteLine { get; set; }
        [DataMember]
        public string WritePrefix { get; set; }
        [DataMember]
        public string WriteSeparator { get; set; }
        [DataMember]
        public string WriteSuffix { get; set; }
    }
}
