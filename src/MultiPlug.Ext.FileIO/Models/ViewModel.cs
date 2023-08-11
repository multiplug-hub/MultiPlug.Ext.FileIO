using System;
using System.Collections.Generic;

namespace MultiPlug.Ext.FileIO.Models
{
    [Serializable]
    public class ViewModel: MarshalByRefObject
    {
        public string Name { get; set; }
        public List<KeyValuePair<string,string>> Files { get; set; }

    }
}
