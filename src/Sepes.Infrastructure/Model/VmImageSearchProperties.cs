using System.ComponentModel.DataAnnotations;

namespace Sepes.Infrastructure.Model
{
    public class VmImageSearchProperties : BaseModel
    {
        [MaxLength(128)]
        public string Publisher { get; set; }

        [MaxLength(128)]
        public string Offer { get; set; }

        [MaxLength(128)]
        public string Sku { get; set; }

        [MaxLength(128)]
        public string DisplayValue { get; set; }

        [MaxLength(64)]
        public string Category { get; set; }

        public bool PartOfRecommended { get; set; }
    }
}
