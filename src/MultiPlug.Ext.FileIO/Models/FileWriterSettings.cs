using MultiPlug.Base;
using MultiPlug.Base.Exchange;
using System.Collections.Generic;
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
        public List<Subscription> WriteSubscriptions { get; set; }
        [DataMember]
        public bool Append { get; set; }
        [DataMember]
        public bool WriteLine { get; set; }
        [DataMember]
        public string GroupSelectKey { get; set; }
        public string OverwriteEnabled { get { return !Append ? "checked" : ""; } }
        public string AppendEnabled { get { return Append ? "checked" : ""; } }
        public string WriteLineEnabled { get { return WriteLine ? "checked" : ""; } }
    }
}
