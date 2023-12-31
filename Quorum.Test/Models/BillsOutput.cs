namespace Quorum.Test.Models
{
    public class BillsOutput
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int SupporterCount { get; set; }
        public int OpposerCount { get; set; }
        public string PrimarySponsor {  get; set; }
    }
}
