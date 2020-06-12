using Structurizr;

namespace Sepes.StructurizrDocs.Models
{
    public static class DefaultStyleDecorator
    {    

        public static void Decorate(ViewSet viewSet)
        {
            // colours, shapes and other diagram styling
            var styles = viewSet.Configuration.Styles;
            styles.Add(new ElementStyle(Tags.SoftwareSystem) { Background = "#1168bd", Color = "#ffffff" });
            styles.Add(new ElementStyle(Tags.Container) { Background = "#438dd5", Color = "#ffffff" });
            styles.Add(new ElementStyle(Tags.Component) { Background = "#85bbf0", Color = "#000000" });
            styles.Add(new ElementStyle(Tags.Person) { Background = "#08427b", Color = "#ffffff", Shape = Shape.Person, FontSize = 22 });
            styles.Add(new ElementStyle(Constants.ExistingSystemTag) { Background = "#999999", Color = "#ffffff" });   
            styles.Add(new ElementStyle(Constants.WebBrowserTag) { Shape = Shape.WebBrowser });          
            styles.Add(new ElementStyle(Constants.DatabaseTag) { Shape = Shape.Cylinder });     
   
        }
    }
}
