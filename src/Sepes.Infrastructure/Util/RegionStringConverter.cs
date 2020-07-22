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
            Region region;
            switch (regionString)
            {
                case "NorwayEast":
                    region = Region.NorwayEast;
                    break;
                case "USWest":
                    region = Region.USWest;
                    break;
                case "USEast":
                    region = Region.USEast;
                    break;
                case "BrazilSouth":
                    region = Region.BrazilSouth;
                    break;
                case "GermanyCentral":
                    region = Region.GermanyCentral;
                    break;
                case "USCentral":
                    region = Region.USCentral;
                    break;
                case "EuropeWest":
                    region = Region.EuropeWest;
                    break;
                case "EuropeNorth":
                    region = Region.EuropeNorth;
                    break;
                default:
                    region = Region.NorwayEast;
                    break;
            }
            return region;
        }
    }
}
