using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace System.Xml
{
    public class XmlElementCollectionBase<T> : XmlElementBase, IEnumerable<T> where T : XmlElement, IKeyedObject
    {
        public XmlElementCollectionBase(XmlDocument document, string localName) : base(string.Empty, localName, string.Empty, document)
        {
        }

        public new XmlNode ParentNode;

        public int Count
        {
            get { return GetItems().Count(); }
        }

        public void Clear()
        {
            foreach (T item in GetItems().ToList())
            {
                if (ParentNode == null)
                {
                    RemoveChild(item);
                }
                else
                {
                    ParentNode.RemoveChild(item);
                }
            }
        }

        public void Add(T item)
        {
            if (ParentNode == null)
            {
                AppendChild(item);
            }
            else
            {
                ParentNode.AppendChild(item);
            }
        }

        public T Get(int i)
        {
            return GetItems().ElementAt(i);
        }

        protected T Find(string name)
        {
            return (T)GetItems().Cast<IKeyedObject>().FirstOrDefault(e => e.Name == name);
        }

        protected virtual IEnumerable<T> GetItems()
        {
            if (ParentNode == null)
            {
                return ChildNodes.OfType<T>();
            }

            return ParentNode.ChildNodes.OfType<T>();
        }

        public new IEnumerator<T> GetEnumerator()
        {
            return GetItems().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
