using System;
using System.Collections.Generic;

namespace Sepes.Infrastructure.Dto
{
    public class LookupDto
    {
        public LookupDto()
        {
      
        }

        public LookupDto(string key, string displayValue)
        {
            Key = key;
            DisplayValue = displayValue;
        }

        public LookupDto(string key)
        {
            Key = key;
            DisplayValue = key;
        }

        public string Key { get; set; }

        public string DisplayValue { get; set; }

        public override String ToString()
        {
            return String.Format("{0} - {1}", Key, DisplayValue);
        }

        public override bool Equals(object obj)
        {
            return obj.ToString() == this.ToString();
        }

        public override int GetHashCode()
        {
            return (Key != null && DisplayValue != null ? ToString().GetHashCode() : 0);
        } 
    }

}
