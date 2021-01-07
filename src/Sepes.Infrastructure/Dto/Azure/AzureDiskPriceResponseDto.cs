using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sepes.Infrastructure.Dto.Azure
{

    /*
    public class AzureDiskPriceResponseDto
    {
        public List<Offers> offers { get; set; }

    }

    public class Offers
    {
        public List<Prices> prices { get; set; }
    }

    public class Prices
    {
        public double value { get; set; }
    }
    */
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    public class Size
    {
        public string slug { get; set; }
        public string displayName { get; set; }
    }

    public class Tier
    {
        public string slug { get; set; }
        public string displayName { get; set; }
    }

    public class SnapshotRedundancy
    {
        public string slug { get; set; }
        public string displayName { get; set; }
    }

    public class UltraSize
    {
        public string slug { get; set; }
        public string displayName { get; set; }
    }

    public class MaxThroughputPerGb
    {
        public int _4 { get; set; }
        public int _8 { get; set; }
        public int _16 { get; set; }
        public int _32 { get; set; }
        public int _64 { get; set; }
        public int _128 { get; set; }
        public int _256 { get; set; }
        public int _512 { get; set; }
        public int _1024 { get; set; }
        public int _2048 { get; set; }
        public int _3072 { get; set; }
        public int _4096 { get; set; }
        public int _5120 { get; set; }
        public int _6144 { get; set; }
        public int _7168 { get; set; }
        public int _8192 { get; set; }
        public int _9216 { get; set; }
        public int _10240 { get; set; }
        public int _11264 { get; set; }
        public int _12288 { get; set; }
        public int _13312 { get; set; }
        public int _14336 { get; set; }
        public int _15360 { get; set; }
        public int _16384 { get; set; }
        public int _17408 { get; set; }
        public int _18432 { get; set; }
        public int _19456 { get; set; }
        public int _20480 { get; set; }
        public int _21504 { get; set; }
        public int _22528 { get; set; }
        public int _23552 { get; set; }
        public int _24576 { get; set; }
        public int _25600 { get; set; }
        public int _26624 { get; set; }
        public int _27648 { get; set; }
        public int _28672 { get; set; }
        public int _29696 { get; set; }
        public int _30720 { get; set; }
        public int _31744 { get; set; }
        public int _32768 { get; set; }
        public int _33792 { get; set; }
        public int _34816 { get; set; }
        public int _35840 { get; set; }
        public int _36864 { get; set; }
        public int _37888 { get; set; }
        public int _38912 { get; set; }
        public int _39936 { get; set; }
        public int _40960 { get; set; }
        public int _41984 { get; set; }
        public int _43008 { get; set; }
        public int _44032 { get; set; }
        public int _45056 { get; set; }
        public int _46080 { get; set; }
        public int _47104 { get; set; }
        public int _48128 { get; set; }
        public int _49152 { get; set; }
        public int _50176 { get; set; }
        public int _51200 { get; set; }
        public int _52224 { get; set; }
        public int _53248 { get; set; }
        public int _54272 { get; set; }
        public int _55296 { get; set; }
        public int _56320 { get; set; }
        public int _57344 { get; set; }
        public int _58368 { get; set; }
        public int _59392 { get; set; }
        public int _60416 { get; set; }
        public int _61440 { get; set; }
        public int _62464 { get; set; }
        public int _63488 { get; set; }
        public int _64512 { get; set; }
        public int _65536 { get; set; }
    }

    public class MaxIopsPerGb
    {
        public int _4 { get; set; }
        public int _8 { get; set; }
        public int _16 { get; set; }
        public int _32 { get; set; }
        public int _64 { get; set; }
        public int _128 { get; set; }
        public int _256 { get; set; }
        public int _512 { get; set; }
        public int _1024 { get; set; }
        public int _2048 { get; set; }
        public int _3072 { get; set; }
        public int _4096 { get; set; }
        public int _5120 { get; set; }
        public int _6144 { get; set; }
        public int _7168 { get; set; }
        public int _8192 { get; set; }
        public int _9216 { get; set; }
        public int _10240 { get; set; }
        public int _11264 { get; set; }
        public int _12288 { get; set; }
        public int _13312 { get; set; }
        public int _14336 { get; set; }
        public int _15360 { get; set; }
        public int _16384 { get; set; }
        public int _17408 { get; set; }
        public int _18432 { get; set; }
        public int _19456 { get; set; }
        public int _20480 { get; set; }
        public int _21504 { get; set; }
        public int _22528 { get; set; }
        public int _23552 { get; set; }
        public int _24576 { get; set; }
        public int _25600 { get; set; }
        public int _26624 { get; set; }
        public int _27648 { get; set; }
        public int _28672 { get; set; }
        public int _29696 { get; set; }
        public int _30720 { get; set; }
        public int _31744 { get; set; }
        public int _32768 { get; set; }
        public int _33792 { get; set; }
        public int _34816 { get; set; }
        public int _35840 { get; set; }
        public int _36864 { get; set; }
        public int _37888 { get; set; }
        public int _38912 { get; set; }
        public int _39936 { get; set; }
        public int _40960 { get; set; }
        public int _41984 { get; set; }
        public int _43008 { get; set; }
        public int _44032 { get; set; }
        public int _45056 { get; set; }
        public int _46080 { get; set; }
        public int _47104 { get; set; }
        public int _48128 { get; set; }
        public int _49152 { get; set; }
        public int _50176 { get; set; }
        public int _51200 { get; set; }
        public int _52224 { get; set; }
        public int _53248 { get; set; }
        public int _54272 { get; set; }
        public int _55296 { get; set; }
        public int _56320 { get; set; }
        public int _57344 { get; set; }
        public int _58368 { get; set; }
        public int _59392 { get; set; }
        public int _60416 { get; set; }
        public int _61440 { get; set; }
        public int _62464 { get; set; }
        public int _63488 { get; set; }
        public int _64512 { get; set; }
        public int _65536 { get; set; }
    }

    public class AustraliaCentral
    {
        public double value { get; set; }
    }

    public class AustraliaCentral2
    {
        public double value { get; set; }
    }

    public class AustraliaEast
    {
        public double value { get; set; }
    }

    public class AustraliaSoutheast
    {
        public double value { get; set; }
    }

    public class BrazilSouth
    {
        public double value { get; set; }
    }

    public class BrazilSoutheast
    {
        public double value { get; set; }
    }

    public class CanadaCentral
    {
        public double value { get; set; }
    }

    public class CanadaEast
    {
        public double value { get; set; }
    }

    public class CentralIndia
    {
        public double value { get; set; }
    }

    public class UsCentral
    {
        public double value { get; set; }
    }

    public class AsiaPacificEast
    {
        public double value { get; set; }
    }

    public class UsEast
    {
        public double value { get; set; }
    }

    public class UsEast2
    {
        public double value { get; set; }
    }

    public class FranceCentral
    {
        public double value { get; set; }
    }

    public class FranceSouth
    {
        public double value { get; set; }
    }

    public class GermanyCentral
    {
        public double value { get; set; }
    }

    public class GermanyNorth
    {
        public double value { get; set; }
    }

    public class GermanyNortheast
    {
        public double value { get; set; }
    }

    public class GermanyWestCentral
    {
        public double value { get; set; }
    }

    public class JapanEast
    {
        public double value { get; set; }
    }

    public class JapanWest
    {
        public double value { get; set; }
    }

    public class KoreaCentral
    {
        public double value { get; set; }
    }

    public class KoreaSouth
    {
        public double value { get; set; }
    }

    public class UsNorthCentral
    {
        public double value { get; set; }
    }

    public class EuropeNorth
    {
        public double value { get; set; }
    }

    public class NorwayEast
    {
        public double value { get; set; }
    }

    public class NorwayWest
    {
        public double value { get; set; }
    }

    public class SouthAfricaNorth
    {
        public double value { get; set; }
    }

    public class SouthAfricaWest
    {
        public double value { get; set; }
    }

    public class UsSouthCentral
    {
        public double value { get; set; }
    }

    public class AsiaPacificSoutheast
    {
        public double value { get; set; }
    }

    public class SouthIndia
    {
        public double value { get; set; }
    }

    public class SwitzerlandNorth
    {
        public double value { get; set; }
    }

    public class SwitzerlandWest
    {
        public double value { get; set; }
    }

    public class UaeCentral
    {
        public double value { get; set; }
    }

    public class UaeNorth
    {
        public double value { get; set; }
    }

    public class UnitedKingdomSouth
    {
        public double value { get; set; }
    }

    public class UnitedKingdomWest
    {
        public double value { get; set; }
    }

    public class UsgovArizona
    {
        public double value { get; set; }
    }

    public class UsgovTexas
    {
        public double value { get; set; }
    }

    public class UsgovVirginia
    {
        public double value { get; set; }
    }

    public class UsWestCentral
    {
        public double value { get; set; }
    }

    public class EuropeWest
    {
        public double value { get; set; }
    }

    public class WestIndia
    {
        public double value { get; set; }
    }

    public class UsWest
    {
        public double value { get; set; }
    }

    public class UsWest2
    {
        public double value { get; set; }
    }

    public class Prices
    {
        [JsonProperty("australia-central")]
        public AustraliaCentral AustraliaCentral { get; set; }
        [JsonProperty("australia-central-2")]
        public AustraliaCentral2 AustraliaCentral2 { get; set; }
        [JsonProperty("australia-east")]
        public AustraliaEast AustraliaEast { get; set; }
        [JsonProperty("australia-southeast")]
        public AustraliaSoutheast AustraliaSoutheast { get; set; }
        [JsonProperty("brazil-south")]
        public BrazilSouth BrazilSouth { get; set; }
        [JsonProperty("brazil-southeast")]
        public BrazilSoutheast BrazilSoutheast { get; set; }
        [JsonProperty("canada-central")]
        public CanadaCentral CanadaCentral { get; set; }
        [JsonProperty("canada-east")]
        public CanadaEast CanadaEast { get; set; }
        [JsonProperty("central-india")]
        public CentralIndia CentralIndia { get; set; }
        [JsonProperty("us-central")]
        public UsCentral UsCentral { get; set; }
        [JsonProperty("asia-pacific-east")]
        public AsiaPacificEast AsiaPacificEast { get; set; }
        [JsonProperty("us-east")]
        public UsEast UsEast { get; set; }
        [JsonProperty("us-east-2")]
        public UsEast2 UsEast2 { get; set; }
        [JsonProperty("france-central")]
        public FranceCentral FranceCentral { get; set; }
        [JsonProperty("france-south")]
        public FranceSouth FranceSouth { get; set; }
        [JsonProperty("germany-central")]
        public GermanyCentral GermanyCentral { get; set; }
        [JsonProperty("germany-north")]
        public GermanyNorth GermanyNorth { get; set; }
        [JsonProperty("germany-northeast")]
        public GermanyNortheast GermanyNortheast { get; set; }
        [JsonProperty("germany-west-central")]
        public GermanyWestCentral GermanyWestCentral { get; set; }
        [JsonProperty("japan-east")]
        public JapanEast JapanEast { get; set; }
        [JsonProperty("japan-west")]
        public JapanWest JapanWest { get; set; }
        [JsonProperty("korea-central")]
        public KoreaCentral KoreaCentral { get; set; }
        [JsonProperty("korea-south")]
        public KoreaSouth KoreaSouth { get; set; }
        [JsonProperty("us-north-central")]
        public UsNorthCentral UsNorthCentral { get; set; }
        [JsonProperty("europe-north")]
        public EuropeNorth EuropeNorth { get; set; }
        [JsonProperty("norway-east")]
        public NorwayEast NorwayEast { get; set; }
        [JsonProperty("norway-west")]
        public NorwayWest NorwayWest { get; set; }
        [JsonProperty("south-africa-north")]
        public SouthAfricaNorth SouthAfricaNorth { get; set; }
        [JsonProperty("south-africa-west")]
        public SouthAfricaWest SouthAfricaWest { get; set; }
        [JsonProperty("us-south-central")]
        public UsSouthCentral UsSouthCentral { get; set; }
        [JsonProperty("asia-pacific-southeast")]
        public AsiaPacificSoutheast AsiaPacificSoutheast { get; set; }
        [JsonProperty("south-india")]
        public SouthIndia SouthIndia { get; set; }
        [JsonProperty("switzerland-north")]
        public SwitzerlandNorth SwitzerlandNorth { get; set; }
        [JsonProperty("switzerland-west")]
        public SwitzerlandWest SwitzerlandWest { get; set; }
        [JsonProperty("uae-central")]
        public UaeCentral UaeCentral { get; set; }
        [JsonProperty("uae-north")]
        public UaeNorth UaeNorth { get; set; }
        [JsonProperty("united-kingdom-south")]
        public UnitedKingdomSouth UnitedKingdomSouth { get; set; }
        [JsonProperty("united-kingdom-west")]
        public UnitedKingdomWest UnitedKingdomWest { get; set; }
        [JsonProperty("usgov-arizona")]
        public UsgovArizona UsgovArizona { get; set; }
        [JsonProperty("usgov-texas")]
        public UsgovTexas UsgovTexas { get; set; }
        [JsonProperty("usgov-virginia")]
        public UsgovVirginia UsgovVirginia { get; set; }
        [JsonProperty("us-west-central")]
        public UsWestCentral UsWestCentral { get; set; }
        [JsonProperty("europe-west")]
        public EuropeWest EuropeWest { get; set; }
        [JsonProperty("west-india")]
        public WestIndia WestIndia { get; set; }
        [JsonProperty("us-west")]
        public UsWest UsWest { get; set; }
        [JsonProperty("us-west-2")]
        public UsWest2 UsWest2 { get; set; }
    }

    public class TransactionsHdd
    {
        public Prices prices { get; set; }
        public string pricingTypes { get; set; }
    }

    public class TransactionsSsd
    {
        public Prices prices { get; set; }
        public string pricingTypes { get; set; }
    }

    public class PremiumssdSnapshot
    {
        public Prices prices { get; set; }
        public string pricingTypes { get; set; }
    }

    public class PremiumssdP1
    {
        public int size { get; set; }
        public int speed { get; set; }
        public int iops { get; set; }
        public Prices prices { get; set; }
        public string pricingTypes { get; set; }
    }

    public class PremiumssdP2
    {
        public int size { get; set; }
        public int speed { get; set; }
        public int iops { get; set; }
        public Prices prices { get; set; }
        public string pricingTypes { get; set; }
    }

    public class PremiumssdP3
    {
        public int size { get; set; }
        public int speed { get; set; }
        public int iops { get; set; }
        public Prices prices { get; set; }
        public string pricingTypes { get; set; }
    }

    public class PremiumssdP4
    {
        public int size { get; set; }
        public int speed { get; set; }
        public int iops { get; set; }
        public Prices prices { get; set; }
        public string pricingTypes { get; set; }
    }

    public class PremiumssdP6
    {
        public int size { get; set; }
        public int speed { get; set; }
        public int iops { get; set; }
        public Prices prices { get; set; }
        public string pricingTypes { get; set; }
    }

    public class PremiumssdP10
    {
        public int size { get; set; }
        public int speed { get; set; }
        public int iops { get; set; }
        public Prices prices { get; set; }
        public string pricingTypes { get; set; }
    }

    public class PremiumssdP15
    {
        public int size { get; set; }
        public int speed { get; set; }
        public int iops { get; set; }
        public Prices prices { get; set; }
        public string pricingTypes { get; set; }
    }

    public class PremiumssdP15DiskMount
    {
        public int size { get; set; }
        public int speed { get; set; }
        public int iops { get; set; }
        public Prices prices { get; set; }
        public string pricingTypes { get; set; }
    }

    public class PremiumssdP20
    {
        public int size { get; set; }
        public int speed { get; set; }
        public int iops { get; set; }
        public Prices prices { get; set; }
        public string pricingTypes { get; set; }
    }

    public class PremiumssdP20DiskMount
    {
        public int size { get; set; }
        public int speed { get; set; }
        public int iops { get; set; }
        public Prices prices { get; set; }
        public string pricingTypes { get; set; }
    }

    public class PremiumssdP30
    {
        public int size { get; set; }
        public int speed { get; set; }
        public int iops { get; set; }
        public Prices prices { get; set; }
        public string pricingTypes { get; set; }
    }

    public class PremiumssdP30DiskMount
    {
        public int size { get; set; }
        public int speed { get; set; }
        public int iops { get; set; }
        public Prices prices { get; set; }
        public string pricingTypes { get; set; }
    }

    public class PremiumssdP30OneYear
    {
        public int size { get; set; }
        public int speed { get; set; }
        public int iops { get; set; }
        public Prices prices { get; set; }
        public string pricingTypes { get; set; }
    }

    public class PremiumssdP40
    {
        public int size { get; set; }
        public int speed { get; set; }
        public int iops { get; set; }
        public Prices prices { get; set; }
        public string pricingTypes { get; set; }
    }

    public class PremiumssdP40DiskMount
    {
        public int size { get; set; }
        public int speed { get; set; }
        public int iops { get; set; }
        public Prices prices { get; set; }
        public string pricingTypes { get; set; }
    }

    public class PremiumssdP40OneYear
    {
        public int size { get; set; }
        public int speed { get; set; }
        public int iops { get; set; }
        public Prices prices { get; set; }
        public string pricingTypes { get; set; }
    }

    public class PremiumssdP50
    {
        public int size { get; set; }
        public int speed { get; set; }
        public int iops { get; set; }
        public Prices prices { get; set; }
        public string pricingTypes { get; set; }
    }

    public class PremiumssdP50DiskMount
    {
        public int size { get; set; }
        public int speed { get; set; }
        public int iops { get; set; }
        public Prices prices { get; set; }
        public string pricingTypes { get; set; }
    }

    public class PremiumssdP50OneYear
    {
        public int size { get; set; }
        public int speed { get; set; }
        public int iops { get; set; }
        public Prices prices { get; set; }
        public string pricingTypes { get; set; }
    }

    public class PremiumssdP60
    {
        public int size { get; set; }
        public int speed { get; set; }
        public int iops { get; set; }
        public Prices prices { get; set; }
        public string pricingTypes { get; set; }
    }

    public class PremiumssdP60DiskMount
    {
        public int size { get; set; }
        public int speed { get; set; }
        public int iops { get; set; }
        public Prices prices { get; set; }
        public string pricingTypes { get; set; }
    }

    public class PremiumssdP60OneYear
    {
        public int size { get; set; }
        public int speed { get; set; }
        public int iops { get; set; }
        public Prices prices { get; set; }
        public string pricingTypes { get; set; }
    }

    public class PremiumssdP70
    {
        public int size { get; set; }
        public int speed { get; set; }
        public int iops { get; set; }
        public Prices prices { get; set; }
        public string pricingTypes { get; set; }
    }

    public class PremiumssdP70DiskMount
    {
        public int size { get; set; }
        public int speed { get; set; }
        public int iops { get; set; }
        public Prices prices { get; set; }
        public string pricingTypes { get; set; }
    }

    public class PremiumssdP70OneYear
    {
        public int size { get; set; }
        public int speed { get; set; }
        public int iops { get; set; }
        public Prices prices { get; set; }
        public string pricingTypes { get; set; }
    }

    public class PremiumssdP80
    {
        public int size { get; set; }
        public int speed { get; set; }
        public int iops { get; set; }
        public Prices prices { get; set; }
        public string pricingTypes { get; set; }
    }

    public class PremiumssdP80DiskMount
    {
        public int size { get; set; }
        public int speed { get; set; }
        public int iops { get; set; }
        public Prices prices { get; set; }
        public string pricingTypes { get; set; }
    }

    public class PremiumssdP80OneYear
    {
        public int size { get; set; }
        public int speed { get; set; }
        public int iops { get; set; }
        public Prices prices { get; set; }
        public string pricingTypes { get; set; }
    }

    public class StandardhddSnapshotLrs
    {
        public Prices prices { get; set; }
        public string pricingTypes { get; set; }
    }

    public class StandardhddSnapshotZrs
    {
        public Prices prices { get; set; }
        public string pricingTypes { get; set; }
    }

    public class StandardhddS4
    {
        public int size { get; set; }
        public int speed { get; set; }
        public int iops { get; set; }
        public Prices prices { get; set; }
        public string pricingTypes { get; set; }
    }

    public class StandardhddS6
    {
        public int size { get; set; }
        public int speed { get; set; }
        public int iops { get; set; }
        public Prices prices { get; set; }
        public string pricingTypes { get; set; }
    }

    public class StandardhddS10
    {
        public int size { get; set; }
        public int speed { get; set; }
        public int iops { get; set; }
        public Prices prices { get; set; }
        public string pricingTypes { get; set; }
    }

    public class StandardhddS15
    {
        public int size { get; set; }
        public int speed { get; set; }
        public int iops { get; set; }
        public Prices prices { get; set; }
        public string pricingTypes { get; set; }
    }

    public class StandardhddS20
    {
        public int size { get; set; }
        public int speed { get; set; }
        public int iops { get; set; }
        public Prices prices { get; set; }
        public string pricingTypes { get; set; }
    }

    public class StandardhddS30
    {
        public int size { get; set; }
        public int speed { get; set; }
        public int iops { get; set; }
        public Prices prices { get; set; }
        public string pricingTypes { get; set; }
    }

    public class StandardhddS40
    {
        public int size { get; set; }
        public int speed { get; set; }
        public int iops { get; set; }
        public Prices prices { get; set; }
        public string pricingTypes { get; set; }
    }

    public class StandardhddS50
    {
        public int size { get; set; }
        public int speed { get; set; }
        public int iops { get; set; }
        public Prices prices { get; set; }
        public string pricingTypes { get; set; }
    }

    public class StandardhddS60
    {
        public int size { get; set; }
        public int speed { get; set; }
        public int iops { get; set; }
        public Prices prices { get; set; }
        public string pricingTypes { get; set; }
    }

    public class StandardhddS70
    {
        public int size { get; set; }
        public int speed { get; set; }
        public int iops { get; set; }
        public Prices prices { get; set; }
        public string pricingTypes { get; set; }
    }

    public class StandardhddS80
    {
        public int size { get; set; }
        public int speed { get; set; }
        public int iops { get; set; }
        public Prices prices { get; set; }
        public string pricingTypes { get; set; }
    }

    public class StandardssdSnapshot
    {
        public Prices prices { get; set; }
        public string pricingTypes { get; set; }
    }

    public class StandardssdE1
    {
        public int size { get; set; }
        public int speed { get; set; }
        public int iops { get; set; }
        public Prices prices { get; set; }
        public string pricingTypes { get; set; }
    }

    public class StandardssdE2
    {
        public int size { get; set; }
        public int speed { get; set; }
        public int iops { get; set; }
        public Prices prices { get; set; }
        public string pricingTypes { get; set; }
    }

    public class StandardssdE3
    {
        public int size { get; set; }
        public int speed { get; set; }
        public int iops { get; set; }
        public Prices prices { get; set; }
        public string pricingTypes { get; set; }
    }

    public class StandardssdE4
    {
        public int size { get; set; }
        public int speed { get; set; }
        public int iops { get; set; }
        public Prices prices { get; set; }
        public string pricingTypes { get; set; }
    }

    public class StandardssdE6
    {
        public int size { get; set; }
        public int speed { get; set; }
        public int iops { get; set; }
        public Prices prices { get; set; }
        public string pricingTypes { get; set; }
    }

    public class StandardssdE10
    {
        public int size { get; set; }
        public int speed { get; set; }
        public int iops { get; set; }
        public Prices prices { get; set; }
        public string pricingTypes { get; set; }
    }

    public class StandardssdE15
    {
        public int size { get; set; }
        public int speed { get; set; }
        public int iops { get; set; }
        public Prices prices { get; set; }
        public string pricingTypes { get; set; }
    }

    public class StandardssdE20
    {
        public int size { get; set; }
        public int speed { get; set; }
        public int iops { get; set; }
        public Prices prices { get; set; }
        public string pricingTypes { get; set; }
    }

    public class StandardssdE30
    {
        public int size { get; set; }
        public int speed { get; set; }
        public int iops { get; set; }
        public Prices prices { get; set; }
        public string pricingTypes { get; set; }
    }

    public class StandardssdE40
    {
        public int size { get; set; }
        public int speed { get; set; }
        public int iops { get; set; }
        public Prices prices { get; set; }
        public string pricingTypes { get; set; }
    }

    public class StandardssdE50
    {
        public int size { get; set; }
        public int speed { get; set; }
        public int iops { get; set; }
        public Prices prices { get; set; }
        public string pricingTypes { get; set; }
    }

    public class StandardssdE60
    {
        public int size { get; set; }
        public int speed { get; set; }
        public int iops { get; set; }
        public Prices prices { get; set; }
        public string pricingTypes { get; set; }
    }

    public class StandardssdE70
    {
        public int size { get; set; }
        public int speed { get; set; }
        public int iops { get; set; }
        public Prices prices { get; set; }
        public string pricingTypes { get; set; }
    }

    public class StandardssdE80
    {
        public int size { get; set; }
        public int speed { get; set; }
        public int iops { get; set; }
        public Prices prices { get; set; }
        public string pricingTypes { get; set; }
    }

    public class UltrassdIops
    {
        public Prices prices { get; set; }
        public string pricingTypes { get; set; }
    }

    public class UltrassdStored
    {
        public Prices prices { get; set; }
        public string pricingTypes { get; set; }
    }

    public class UltrassdThroughput
    {
        public Prices prices { get; set; }
        public string pricingTypes { get; set; }
    }

    public class UltrassdVcpu
    {
        public Prices prices { get; set; }
        public string pricingTypes { get; set; }
    }

    public class Offers
    {
        [JsonProperty("transactions-hdd")]
        public TransactionsHdd TransactionsHdd { get; set; }
        [JsonProperty("transactions-ssd")]
        public TransactionsSsd TransactionsSsd { get; set; }
        [JsonProperty("premiumssd-snapshot")]
        public PremiumssdSnapshot PremiumssdSnapshot { get; set; }
        [JsonProperty("premiumssd-p1")]
        public PremiumssdP1 PremiumssdP1 { get; set; }
        [JsonProperty("premiumssd-p2")]
        public PremiumssdP2 PremiumssdP2 { get; set; }
        [JsonProperty("premiumssd-p3")]
        public PremiumssdP3 PremiumssdP3 { get; set; }
        [JsonProperty("premiumssd-p4")]
        public PremiumssdP4 PremiumssdP4 { get; set; }
        [JsonProperty("premiumssd-p6")]
        public PremiumssdP6 PremiumssdP6 { get; set; }
        [JsonProperty("premiumssd-p10")]
        public PremiumssdP10 PremiumssdP10 { get; set; }
        [JsonProperty("premiumssd-p15")]
        public PremiumssdP15 PremiumssdP15 { get; set; }
        [JsonProperty("premiumssd-p15-disk-mount")]
        public PremiumssdP15DiskMount PremiumssdP15DiskMount { get; set; }
        [JsonProperty("premiumssd-p20")]
        public PremiumssdP20 PremiumssdP20 { get; set; }
        [JsonProperty("premiumssd-p20-disk-mount")]
        public PremiumssdP20DiskMount PremiumssdP20DiskMount { get; set; }
        [JsonProperty("premiumssd-p30")]
        public PremiumssdP30 PremiumssdP30 { get; set; }
        [JsonProperty("premiumssd-p30-disk-mount")]
        public PremiumssdP30DiskMount PremiumssdP30DiskMount { get; set; }
        [JsonProperty("premiumssd-p30-one-year")]
        public PremiumssdP30OneYear PremiumssdP30OneYear { get; set; }
        [JsonProperty("premiumssd-p40")]
        public PremiumssdP40 PremiumssdP40 { get; set; }
        [JsonProperty("premiumssd-p40-disk-mount")]
        public PremiumssdP40DiskMount PremiumssdP40DiskMount { get; set; }
        [JsonProperty("premiumssd-p40-one-year")]
        public PremiumssdP40OneYear PremiumssdP40OneYear { get; set; }
        [JsonProperty("premiumssd-p50")]
        public PremiumssdP50 PremiumssdP50 { get; set; }
        [JsonProperty("premiumssd-p50-disk-mount")]
        public PremiumssdP50DiskMount PremiumssdP50DiskMount { get; set; }
        [JsonProperty("premiumssd-p50-one-year")]
        public PremiumssdP50OneYear PremiumssdP50OneYear { get; set; }
        [JsonProperty("premiumssd-p60")]
        public PremiumssdP60 PremiumssdP60 { get; set; }
        [JsonProperty("premiumssd-p60-disk-mount")]
        public PremiumssdP60DiskMount PremiumssdP60DiskMount { get; set; }
        [JsonProperty("premiumssd-p60-one-year")]
        public PremiumssdP60OneYear PremiumssdP60OneYear { get; set; }
        [JsonProperty("premiumssd-p70")]
        public PremiumssdP70 PremiumssdP70 { get; set; }
        [JsonProperty("premiumssd-p70-disk-mount")]
        public PremiumssdP70DiskMount PremiumssdP70DiskMount { get; set; }
        [JsonProperty("premiumssd-p70-one-year")]
        public PremiumssdP70OneYear PremiumssdP70OneYear { get; set; }
        [JsonProperty("premiumssd-p80")]
        public PremiumssdP80 PremiumssdP80 { get; set; }
        [JsonProperty("premiumssd-p80-disk-mount")]
        public PremiumssdP80DiskMount PremiumssdP80DiskMount { get; set; }
        [JsonProperty("premiumssd-p80-one-year")]
        public PremiumssdP80OneYear PremiumssdP80OneYear { get; set; }
        [JsonProperty("standardhdd-snapshot-lrs")]
        public StandardhddSnapshotLrs StandardhddSnapshotLrs { get; set; }
        [JsonProperty("standardhdd-snapshot-zrs")]
        public StandardhddSnapshotZrs StandardhddSnapshotZrs { get; set; }
        [JsonProperty("standardhdd-s4")]
        public StandardhddS4 StandardhddS4 { get; set; }
        [JsonProperty("standardhdd-s6")]
        public StandardhddS6 StandardhddS6 { get; set; }
        [JsonProperty("standardhdd-s10")]
        public StandardhddS10 StandardhddS10 { get; set; }
        [JsonProperty("standardhdd-s15")]
        public StandardhddS15 StandardhddS15 { get; set; }
        [JsonProperty("standardhdd-s20")]
        public StandardhddS20 StandardhddS20 { get; set; }
        [JsonProperty("standardhdd-s30")]
        public StandardhddS30 StandardhddS30 { get; set; }
        [JsonProperty("standardhdd-s40")]
        public StandardhddS40 StandardhddS40 { get; set; }
        [JsonProperty("standardhdd-s50")]
        public StandardhddS50 StandardhddS50 { get; set; }
        [JsonProperty("standardhdd-s60")]
        public StandardhddS60 StandardhddS60 { get; set; }
        [JsonProperty("standardhdd-s70")]
        public StandardhddS70 StandardhddS70 { get; set; }
        [JsonProperty("standardhdd-s80")]
        public StandardhddS80 StandardhddS80 { get; set; }
        [JsonProperty("standardssd-snapshot")]
        public StandardssdSnapshot StandardssdSnapshot { get; set; }
        [JsonProperty("standardssd-e1")]
        public StandardssdE1 StandardssdE1 { get; set; }
        [JsonProperty("standardssd-e2")]
        public StandardssdE2 StandardssdE2 { get; set; }
        [JsonProperty("standardssd-e3")]
        public StandardssdE3 StandardssdE3 { get; set; }
        [JsonProperty("standardssd-e4")]
        public StandardssdE4 StandardssdE4 { get; set; }
        [JsonProperty("standardssd-e6")]
        public StandardssdE6 StandardssdE6 { get; set; }
        [JsonProperty("standardssd-e10")]
        public StandardssdE10 StandardssdE10 { get; set; }
        [JsonProperty("standardssd-e15")]
        public StandardssdE15 StandardssdE15 { get; set; }
        [JsonProperty("standardssd-e20")]
        public StandardssdE20 StandardssdE20 { get; set; }
        [JsonProperty("standardssd-e30")]
        public StandardssdE30 StandardssdE30 { get; set; }
        [JsonProperty("standardssd-e40")]
        public StandardssdE40 StandardssdE40 { get; set; }
        [JsonProperty("standardssd-e50")]
        public StandardssdE50 StandardssdE50 { get; set; }
        [JsonProperty("standardssd-e60")]
        public StandardssdE60 StandardssdE60 { get; set; }
        [JsonProperty("standardssd-e70")]
        public StandardssdE70 StandardssdE70 { get; set; }
        [JsonProperty("standardssd-e80")]
        public StandardssdE80 StandardssdE80 { get; set; }
        [JsonProperty("ultrassd-iops")]
        public UltrassdIops UltrassdIops { get; set; }
        [JsonProperty("ultrassd-stored")]
        public UltrassdStored UltrassdStored { get; set; }
        [JsonProperty("ultrassd-throughput")]
        public UltrassdThroughput UltrassdThroughput { get; set; }
        [JsonProperty("ultrassd-vcpu")]
        public UltrassdVcpu UltrassdVcpu { get; set; }
    }

    public class Region2
    {
        public string slug { get; set; }
        public string displayName { get; set; }
    }

    public class Resources
    {
        public string per_additional_mount { get; set; }
        public string shared_disks { get; set; }
        public string add_snapshot { get; set; }
        public string enable_shared_disks { get; set; }
        public string premiumssd { get; set; }
        public string enable_another_shared_disk { get; set; }
        public string storage_costs { get; set; }
        public string mbps_throughput_1 { get; set; }
        public string managed_disks_info { get; set; }
        public string uptombsec_1 { get; set; }
        public string e1 { get; set; }
        public string e2 { get; set; }
        public string e3 { get; set; }
        public string e4 { get; set; }
        public string e6 { get; set; }
        public string s6 { get; set; }
        public string s4 { get; set; }
        public string p4 { get; set; }
        public string p6 { get; set; }
        public string p1 { get; set; }
        public string p3 { get; set; }
        public string p2 { get; set; }
        public string standardssd { get; set; }
        public string standardhdd { get; set; }
        public string e10 { get; set; }
        public string e15 { get; set; }
        public string e30 { get; set; }
        public string e20 { get; set; }
        public string e50 { get; set; }
        public string e40 { get; set; }
        public string e70 { get; set; }
        public string e60 { get; set; }
        public string e80 { get; set; }
        public string lrs { get; set; }
        public string s70 { get; set; }
        public string s60 { get; set; }
        public string s50 { get; set; }
        public string s40 { get; set; }
        public string s30 { get; set; }
        public string s20 { get; set; }
        public string s15 { get; set; }
        public string s10 { get; set; }
        public string s80 { get; set; }
        public string p50 { get; set; }
        public string p70 { get; set; }
        public string p40 { get; set; }
        public string p15 { get; set; }
        public string p10 { get; set; }
        public string p60 { get; set; }
        public string p30 { get; set; }
        public string p20 { get; set; }
        public string p80 { get; set; }
        public string zrs { get; set; }
        public string throughput { get; set; }
        public string ultrassd { get; set; }
        public string summary_shared_disks_2 { get; set; }
        public string snapshot { get; set; }
        public string iops_note_3 { get; set; }
        public string throughput_note_3 { get; set; }
        public string shared_disk_num_1 { get; set; }
        public string additional_mounts { get; set; }
        public string iops { get; set; }
    }

    public class AzureDiskPriceResponseDto
    {
        public List<Size> sizes { get; set; }
        public List<Tier> tiers { get; set; }
        public List<SnapshotRedundancy> snapshotRedundancies { get; set; }
        public List<UltraSize> ultraSizes { get; set; }
        public int ultraMaxSizeGb { get; set; }
        public MaxThroughputPerGb maxThroughputPerGb { get; set; }
        public int minThroughput { get; set; }
        public MaxIopsPerGb maxIopsPerGb { get; set; }
        public int minIops { get; set; }
        public Offers offers { get; set; }
        public List<Region2> regions { get; set; }
        public List<object> zones { get; set; }
        public List<object> discounts { get; set; }
        public Resources resources { get; set; }
    }


}
