
namespace MultiPlug.Ext.FileIO.Models.Settings
{
    public class WriterPost
    {
        public string Guid { get; set; }
        public string FilePath { get; set; }
        public bool Append { get; set; }
        public bool FileActionAppend { get; set; }
        public bool Writeline { get; set; }
        public string[] SubscriptionGuid { get; set; }
        public string[] SubscriptionId { get; set; }
        public string WritePrefix { get; set; }
        public string WriteSeparator { get; set; }
        public string WriteSuffix { get; set; }
    }
}
