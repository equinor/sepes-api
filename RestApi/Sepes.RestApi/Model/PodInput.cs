namespace Sepes.RestApi.Model
{
    public class PodInput
    {
        public ushort podId { get; set; }
        public string podName { get; set; }
        public int studyId { get; set; }
        public string tag { get; set; }
        public RuleInput[] incoming { get; set; }
        public RuleInput[] outgoing { get; set; }
        public string[] users { get; set; }
    }
}
