using System;
using System.Collections.Generic;

namespace Sepes.Infrastructure.Dto.Azure
{
    public class AzurePriceResponseItemDto
    {
        public string currencyCode { get; set; } //"USD",
        public double tierMinimumUnits { get; set; } //0.0,
        public double retailPrice { get; set; } //0.176346,
        public double unitPrice { get; set; } //0.176346,
        public string armRegionName { get; set; } //"westeurope",
        public string location { get; set; } //"EU West",
        public DateTime effectiveStartDate { get; set; } //"2020-08-01T00:00:00Z",
        public string meterId { get; set; } //"000a794b-bdb0-58be-a0cd-0c3a0f222923",
        public string meterName { get; set; } //"F16s Spot",
        public string productId { get; set; } //"DZH318Z0BQPS",
        public string skuId { get; set; } //"DZH318Z0BQPS/00TG",
        public string productName { get; set; } //"Virtual Machines FS Series Windows",
        public string skuName { get; set; } //"F16s Spot",
        public string serviceName { get; set; } //"Virtual Machines",
        public string serviceId { get; set; } //"DZH313Z7MMC8",
        public string serviceFamily { get; set; } //"Compute",
        public string unitOfMeasure { get; set; } //"1 Hour",
        public string type { get; set; } //"DevTestConsumption",
        public bool isPrimaryMeterRegion { get; set; } //true,
        public string armSkuName { get; set; } //"Standard_F16s"

    }
    public class AzureRetailPriceApiResponseDto
    {
        public string BillingCurrency { get; set; }
        public string CustomerEntityId { get; set; }
        public string CustomerEntityType { get; set; }


        public List<AzurePriceResponseItemDto> Items { get; set; }


    }
}

