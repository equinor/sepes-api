using Microsoft.Azure.Management.ResourceManager.Fluent.Core;

namespace Sepes.Azure.Util
{
    public static class RegionStringConverter
    {
        public static Region Convert(string regionString)
        {
            return regionString switch
            {
                "norwayeast" => Region.NorwayEast,  
                "europewest" => Region.EuropeWest,
                "europenorth" => Region.EuropeNorth,
                _ => Region.NorwayEast,
            };
        }
    }
}
