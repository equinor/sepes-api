using System.ComponentModel.DataAnnotations;

namespace Sepes.Infrastructure.Model
{
    public class Sandbox : UpdateableBaseModel
    { 
        public string Name { get; set; }

        public Study Study { get; set; }

        public int StudyId { get; set; }
    }
}
