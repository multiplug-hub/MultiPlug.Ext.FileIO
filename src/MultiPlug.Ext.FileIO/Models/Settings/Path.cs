using MultiPlug.Base;
using System.Runtime.Serialization;

namespace MultiPlug.Ext.FileIO.Models.Settings
{
    public class Path : MultiPlugBase
    {
        [DataMember]
        public string Guid { get; set; }
        [DataMember]
        public string FilePath { get; set; }
        public string FilePathJsonEncoded { get; set; }

        public string BackButton { get; set; }
    }
}
