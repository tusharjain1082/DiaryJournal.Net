using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Linq;
using System.Xml.Linq;

namespace EvalScript.Evaluating
{
    public class XmlEvalObject
    {

        public XElement XmlElement { get; private set; }

        public List<XmlEvalObject> Children { get; private set; } = new List<XmlEvalObject>();

        public string TagName => XmlElement.Name.LocalName;


        public XmlEvalObject(XElement xmlElement)
        {
            XmlElement = xmlElement;
            foreach(var child in xmlElement.Descendants())
                Children.Add(new XmlEvalObject(child));
        }


        public object this[string query]
        {
            get
            {
                if (!query.StartsWith("@"))
                {
					if (query == nameof(TagName))
						return TagName;
                    object result = RunChildQuery(query);
                    if (result != null || query.StartsWith("~"))
                        return result;
                }
                return RunAttributeQuery(query);
            }
        }


        private string RunAttributeQuery(string query)
        {
            if (query.StartsWith("@"))
                query = query.Substring(1);
            return XmlElement.Attributes().FirstOrDefault(x => x.Name == query).Value;
        }


        private object RunChildQuery(string query)
        {
            if (query == "~First")
                return Children.FirstOrDefault();
            if (query.StartsWith("~First_"))
            {
                query = query.Substring("~First_".Length);
                return Children.FirstOrDefault(x => x.TagName == query);
            }

            if (query == "~Last")
                return Children.LastOrDefault();
            if (query.StartsWith("~Last_"))
            {
                query = query.Substring("~Last_".Length);
                return Children.LastOrDefault(x => x.TagName == query);
            }

            if (query == "~All")
                return Children;
            if (query.StartsWith("~All_"))
            {
                query = query.Substring("~All_".Length);
                return Children.Where(x => x.TagName == query).ToList();
            }

            if (query.StartsWith("~Index"))
            {
                string indexText = query.Substring("~Index".Length);
                int underscoreIndex = indexText.IndexOf('_');
                if (underscoreIndex > 0)
                    indexText = indexText.Substring(0, underscoreIndex);
                int index = int.Parse(indexText);

                if (query == "~Index" + index)
                    return (Children.Count > index) ? Children[index] : null;
                else
                {
                    query = query.Substring($"~Index{indexText}_".Length);
                    var list = Children.Where(x => x.TagName == query).ToList();
                    return (list.Count > index) ? list[index] : null;
                }
            }

            if (query == "~Value")
                return ToString();

            if (query.StartsWith("~"))
                query = query.Substring(1);
            return Children.FirstOrDefault(x => x.TagName == query);
        }


        public override string ToString()
        {
            if (XmlElement.FirstNode != null && XmlElement.FirstNode is XText)
                return (XmlElement.FirstNode as XText).Value.Trim();
            return XmlElement.Value;
        }

    }
}
