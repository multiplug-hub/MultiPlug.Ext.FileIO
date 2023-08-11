using MultiPlug.Base;
using MultiPlug.Ext.FileIO.Components.FileReader;
using MultiPlug.Ext.FileIO.Components.FileWriter;
using System.Collections.Generic;

namespace MultiPlug.Ext.FileIO.Models.Settings
{
    public class Files : MultiPlugBase
    {
        public List<FileWriterComponent> FileWriters { get; set; }
        public List<FileReaderComponent> FileReaders { get; set; }
    }
}
