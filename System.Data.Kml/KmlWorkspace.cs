using System.Workspace;
using System.Xml;

namespace System.Data.Kml
{
    internal class KmlWorkspace : MapWorkspace
    {
        public KmlWorkspace(XmlDocument document) : base(document)
        {
        }

        //public override MapLayer CreateLayer(Guid clsId)
        //{
        //    return base.CreateLayer(clsId) ?? new KmlMapLayer { Command = new KmlCommand() };
        //}

        //public void LoadXml(XmlElement e)
        //{
        //    var nodeSet = new HashSet<XmlNode>();
        //    var categorySet = new HashSet<string>();

        //    Id = e.Get(Guid.NewGuid, Constants.Xml.Id);
        //    Name = e.Get(Name, Constants.Xml.Name);
        //    Description = e.Get(Description, Constants.Xml.Description);
        //    Open = e.Get(true, Constants.Xml.Open);
        //    Visibility = e.Get(true, Constants.Xml.Visibility);

        //    Srid = e.Get(0, "srid");
        //    ActiveLayer = e.Get(Guid.Empty, "activeLayer");
        //    CurrentWindow.LoadXml(e);
        //    Styles.LoadXml(e);

        //    categorySet.Add(Name);

        //    foreach (XmlNode placemark in e.GetElementsByTagName(Constants.Xml.Placemark))
        //    {
        //        XmlNode folder = placemark.ParentNode;

        //        if (nodeSet.Add(folder))
        //        {
        //            MapCategory category = FindCategory(folder.ParentNode.Get(Constants.Xml.Name)) ?? this;

        //            if (categorySet.Add(folder.ParentNode.Get(Constants.Xml.Name)))
        //            {
        //                category = AddNewCategory(categorySet.Last());
        //            }

        //            MapLayer layer = NewLayer(folder.Get(default(string), Constants.Xml.Name));
        //            layer.LoadXml(folder);
        //            category.Layers.Add(layer);
        //        }
        //    }
        //}

        //public void SaveXml(XmlElement e)
        //{
        //    e.SetChild(Constants.Xml.Id, Id);
        //    e.SetChild(Constants.Xml.Name, Name);
        //    e.SetChild(Constants.Xml.Description, Description);
        //    e.SetChild(Constants.Xml.Open, Open);
        //    e.SetChild(Constants.Xml.Visibility, Visibility);

        //    e.SetChild("srid", Srid);
        //    e.SetChild("activeLayer", ActiveLayer);

        //    CurrentWindow.SaveXml(e);
        //    Styles.SaveXml(e);

        //    foreach(MapLayer item in GetLayers())
        //    {
        //        item.SaveXml(e);
        //    }
        //}

        //protected override void OnLayerCollectionChanged(CollectionChangeAction action, MapLayer item)
        //{
        //    var layer = item as KmlMapLayer;

        //    base.OnLayerCollectionChanged(action, item);

        //    if (layer == null)
        //    {
        //        return;
        //    }

        //    if (layer.Command.Folder == null)
        //    {
        //        layer.Command.Folder = (KmlFolder)Document.DocumentElement.AppendChild(Document.CreateElement(Constants.Xml.Folder));
        //        layer.Command.Folder.SetChild(Constants.Xml.Name, Name);
        //    }
        //}
    }
}
