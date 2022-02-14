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
                "westeurope" => Region.EuropeWest,
                "northeurope" => Region.EuropeNorth,
                "norwaywest" => Region.NorwayWest,
                _ => Region.NorwayEast,
            };
        }
    }
}
