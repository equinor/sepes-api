using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sepes.Infrastructure.Util
{
    public static class RegionStringConverter
    {
        public static Region Convert(string regionString)
        {
            return regionString switch
            {
                "NorwayEast" => Region.NorwayEast,
                "USWest" => Region.USWest,
                "USEast" => Region.USEast,
                "BrazilSouth" => Region.BrazilSouth,
                "GermanyCentral" => Region.GermanyCentral,
                "USCentral" => Region.USCentral,
                "EuropeWest" => Region.EuropeWest,
                "EuropeNorth" => Region.EuropeNorth,
                _ => Region.NorwayEast,
            };
        }
    }
}
