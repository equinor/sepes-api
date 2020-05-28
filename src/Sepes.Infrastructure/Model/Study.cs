using System.ComponentModel.DataAnnotations;

namespace Sepes.Infrastructure.Model
{
    public class Study
    {
        public int Id { get; set; }

        public string Name { get; set; }

        [MaxLength(4000)]
        public string JsonData { get; set; } 
    }
}
