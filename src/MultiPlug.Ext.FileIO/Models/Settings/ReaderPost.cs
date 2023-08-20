
namespace MultiPlug.Ext.FileIO.Models.Settings
{
    public class ReaderPost
    {
        public string guid { get; set; }
        public string FilePath { get; set; }
        public string EventId { get; set; }
        public string EventDescription { get; set; }
        public bool nfla { get; set; }
        public bool nflw { get; set; }
        public bool nffn { get; set; }
        public bool nfdn { get; set; }
        public string EventSubject { get; set; }
        public string readaction { get; set; }
        public int updatepart { get; set; }

        public string[] SubscriptionGuid { get; set; }
        public string[] SubscriptionId { get; set; }
    }
}
