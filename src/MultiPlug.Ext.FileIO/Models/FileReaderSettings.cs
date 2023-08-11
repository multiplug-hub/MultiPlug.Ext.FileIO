using BritishSystems.MultiPlug.Extension.Version1;
using MultiPlug.Base;
using MultiPlug.Base.Exchange;
using System.Collections.Generic;
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
        public bool nFLA { get; set; }
        [DataMember]
        public bool nFLW { get; set; }
        [DataMember]
        public bool nFFN { get; set; }
        [DataMember]
        public bool nFDN { get; set; }
        [DataMember]
        public int UpdatePart { get; set; }
        [DataMember]
        public string Subject { get; set; }

        public string ReadPartialLines{ get { return (UpdatePart > 0) ? UpdatePart.ToString() : ""; } }
        public string ReadActionFullEnabled { get { return (UpdatePart == 0) ? "checked" : ""; } }
        public string ReadActionPartialEnabled { get { return UpdatePart != 0 ? "checked" : ""; } }
        public string nFLAEnabled { get { return nFLA ? "checked" : ""; } }
        public string nFLWEnabled { get { return nFLW ? "checked" : ""; } }
        public string nFFNEnabled { get { return nFFN ? "checked" : ""; } }
        public string nFDNEnabled { get { return nFDN ? "checked" : ""; } }
        public string ReadPartialLinesDisabled { get { return UpdatePart == 0 ? "disabled" : ""; } }
    }
}
