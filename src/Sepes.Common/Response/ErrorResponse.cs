namespace Sepes.Common.Response
{
    public class ErrorResponse
    {
        public bool Critical { get; set; }
        public string Message { get; set; }
        public string RequestId { get; set; }       
    }
}
