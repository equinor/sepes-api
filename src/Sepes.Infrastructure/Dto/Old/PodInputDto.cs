//using System.Collections.Generic;
//using System.Linq;

//namespace Sepes.Infrastructure.Dto
//{
//    public class PodInputDto
//    {
//        public ushort? podId { get; set; }
//        public string podName { get; set; }
//        public int studyId { get; set; }
//        public string tag { get; set; }
//        public RuleInputDto[] incoming { get; set; } = new RuleInputDto[]{};
//        public RuleInputDto[] outgoing { get; set; } = new RuleInputDto[]{};
//        //Corresponds to allowAll internally
//        public bool openInternet { get; set; }


//        public PodDto ToPod()
//        {
//            var podIncoming = incoming.Select(r => r.ToRule());
//            var podOutgoing = outgoing.Select(r => r.ToRule());

//            return new PodDto(podId, podName, studyId, openInternet, podIncoming, podOutgoing, new List<DataSet>(), new List<DataSet>());
//        }
//    }
//}
