namespace Sepes.RestApi.Model
{
    public class AzTokenClass
    {
        //Issue: 38  add parameters to token
        public string idToken { get; set; }
    }

    public class SepesTokenClass
    {
        //Issue: 38 Judge what parameters are needed for just refreshing. Username and such should be in token content body}
        public string idToken { get; set; }
    }
}