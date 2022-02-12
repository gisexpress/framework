using System.Windows.Modules;
using System.Windows.Modules.Controls;
using System.Xml;

namespace System.Data.Kml
{
    internal class KmlProject : BlankProject
    {
        public KmlProject(IApplication application) : base(application)
        {
        }

        public new KmlWorkspace Workspace
        {
            get { return (KmlWorkspace)base.Workspace; }
        }

        protected override bool OnLoad()
        {
            //XmlElement e = Workspace.Document.DocumentElement[Constants.Xml.Document];

            //if (e == null)
            //{
            //    return false;
            //}

            //Name = e.Get(Path.GetFileNameWithoutExtension(File.Name), Constants.Xml.Name);
            //FileInfo.PreviewData = e.Get(default(string), Constants.Xml.Preview);
            //FileInfo.Template = e.Get(default(string), Constants.Xml.Template);
            //Properties.LoadXml(e[Constants.Xml.Properties]);
            //Workspace.LoadXml(e);

            return true;
        }

        protected override bool OnSave()
        {
            //XmlElement e = Workspace.Document.DocumentElement[Constants.Xml.Document];

            //e.SetChild(Constants.Xml.Name, Name);
            //e.SetChild(Constants.Xml.Preview, FileInfo.PreviewData);
            //e.SetChild(Constants.Xml.Template, FileInfo.Template);

            //Properties.SaveXml(e.SetChild(Constants.Xml.Properties));
            //Workspace.SaveXml(e);

            return true;
        }

        public override XmlElement CreateElement(string prefix, string localName, string namespaceURI)
        {
            switch (localName)
            {
                case Constants.Xml.Placemark:
                    return new KmlPlacemark(this);
            }

            return base.CreateElement(prefix, localName, namespaceURI);
        }
    }
}
