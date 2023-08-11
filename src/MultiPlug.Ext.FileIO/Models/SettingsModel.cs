using System;
using System.Collections.Generic;

namespace MultiPlug.Ext.FileIO.Models
{
    [Serializable]
    public class SettingsModel : MarshalByRefObject
    {
        public List<FileReaderSettings> Files { get; set; }
    }
}
