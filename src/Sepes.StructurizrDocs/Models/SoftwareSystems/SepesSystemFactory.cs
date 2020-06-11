using Structurizr;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sepes.StructurizrDocs.Models.SoftwareSystems.External
{
    public static class SepesSystemFactory
    {
        public static SoftwareSystem Sepes(Model model)
        {
            var sepesSystem = model.AddSoftwareSystem(Location.Internal, "Sepes", "Allows users to administer Studies and subcomponents.");
           

            return sepesSystem;
        }
    }
}
