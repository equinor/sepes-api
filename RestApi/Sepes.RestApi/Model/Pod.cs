namespace Sepes.RestApi.Model
{
    public class Pod{
        public ushort id {get;}
        public string name {get;}
        public int studyId {get;}

        public Pod(ushort id, string name, int studyId){
            this.id = id;
            this.name = name;
            this.studyId = studyId;
        }

        public string networkName => $"{studyId}-{name.Replace(" ", "-")}Network";
        public string addressSpace => $"10.{1 + id / 256}.{id % 256}.0/24";
    }
}