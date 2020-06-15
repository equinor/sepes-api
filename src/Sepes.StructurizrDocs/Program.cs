using Sepes.StructurizrDocs.Models.Diagrams;
using Structurizr;
using Structurizr.Api;
using Structurizr.IO.C4PlantUML;
using Structurizr.IO.C4PlantUML.ModelExtensions;
using System;
using System.IO;
using System.Linq;

namespace Sepes.StructurizrDocs
{
    class Program
    {
        const string C1_PATH = @"D:\Workspace\SepesPlantUml\Sepes_C1_PlantUML.puml";
        const string C2_PATH = @"D:\Workspace\SepesPlantUml\Sepes_C2_PlantUML.puml";

        static void Main(string[] args)
        {
            CreateC1();
            CreateC2();
           
        }

        static void CreateC1()
        {
            var c1 = C1.Create();
            ExportDiagram(c1, C1_PATH);

        }

        static void CreateC2()
        {
            var c2 = C2.Create();
            ExportDiagram(c2, C2_PATH);

        }

        static void ExportDiagram(Workspace workspace, string filePath)
        {
            using (var stringWriter = new StringWriter())
            {
                var plantUmlWriter = new C4PlantUmlWriter();
                plantUmlWriter.Write(workspace, stringWriter);
                var plantUmlString = stringWriter.ToString();
                File.WriteAllText(filePath, plantUmlString);
                Console.WriteLine("Exporting diagram to: " + filePath);
                Console.WriteLine(plantUmlString);
            }
        }



    }
}
