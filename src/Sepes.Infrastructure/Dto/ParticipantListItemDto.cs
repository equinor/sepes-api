namespace Sepes.Infrastructure.Dto
{
    public class ParticipantListItemDto : LookupBaseDto
    {
        public string Source { get; set; }
        public int? DatabaseId { get; set; }
        public string AzureObjectId { get; set; }
        public string EmailAddress { get; set; }
        public string UserName { get; set; }
    }
}
