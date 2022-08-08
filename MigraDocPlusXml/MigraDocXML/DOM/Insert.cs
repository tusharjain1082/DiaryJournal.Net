using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace MigraDocXML.DOM
{
    public class Insert : LogicalElement
    {
        public override MigraDoc.DocumentObjectModel.DocumentObject GetModel() => GetParent()?.GetModel();

        private Dictionary<string, string> _definitions = new Dictionary<string, string>();

		/// <summary>
		/// Keeps track of the variables that have been updated on the parent so they can be set back after
		/// </summary>
		private Dictionary<string, VariableUpdate> _variableUpdates = new Dictionary<string, VariableUpdate>();

        internal XmlElement ResourceXmlElement { get; private set; }


        public override void SetUnknownAttribute(string name, object value)
        {
            _definitions[name] = value?.ToString();
        }


        private XmlElement GetResource(string filePath, string resourceName)
        {
            string xmlText = System.IO.File.ReadAllText(filePath);
            xmlText = PdfXmlReader.PreprocessXmlString(xmlText);

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlText);
            return GetResource(xmlDoc, resourceName);
        }

        private XmlElement GetResource(XmlDocument xmlDoc, string resourceName)
        {
            var root = xmlDoc.DocumentElement;
            if (root.Name != "Document")
                throw new Exception("Invalid markup");
            
            return root.GetElementsByTagName("Resource").OfType<XmlElement>()
                 .FirstOrDefault(x =>
                     x.Attributes.OfType<XmlAttribute>().Any(y => y.Name == "Name" && y.Value == resourceName)
                 );
        }

		private XmlElement GetResource(string text)
		{
			string xmlText = $"<Document><Resource Name=\"DynamicText\">{text}</Resource></Document>";
			xmlText = PdfXmlReader.PreprocessXmlString(xmlText);

			XmlDocument xmlDoc = new XmlDocument();
			xmlDoc.LoadXml(xmlText);
			return GetResource(xmlDoc, "DynamicText");
		}


		public override void Run(Action childProcessor)
        {
            var doc = GetDocument();

			XmlElement resource = null;
			if (!string.IsNullOrEmpty(Text))
				resource = GetResource(Text);
			else if (string.IsNullOrEmpty(Path) && !string.IsNullOrEmpty(Resource))
				resource = GetResource(doc.GetXmlElement().OwnerDocument, Resource);
			else if (!string.IsNullOrEmpty(Path) && !string.IsNullOrEmpty(Resource))
				resource = GetResource(System.IO.Path.Combine(doc.ResourcePath, Path), Resource);
			else
				throw new Exception("No resource specified");
			
            if (resource == null)
                throw new Exception("Resource not found");
            ResourceXmlElement = resource;

			//Update the variables on the parent for any passed in parameters
            foreach (var def in _definitions)
            {
                var value = doc.ScriptRunner.Run(def.Value, s => GetParent().GetVariable(s));
				bool variableAdded = !GetParent().OwnsVariable(def.Key);
				object oldValue = variableAdded ? null : GetParent().GetVariable(def.Key);
				if (variableAdded)
					GetParent().NewVariable(def.Key, value);
				else
					GetParent().SetVariable(def.Key, value);
				_variableUpdates.Add(def.Key, new VariableUpdate(variableAdded, oldValue, value));
            }

			//Once everything has been built from the insert, revert the parent variables back to what they were
			FullyBuilt += OnFullyBuilt;

            childProcessor();
        }

		private void OnFullyBuilt(object sender, EventArgs e)
		{
			FullyBuilt -= OnFullyBuilt;
			foreach(var variableUpdate in _variableUpdates)
			{
				if (variableUpdate.Value.Added)
					GetParent().RemoveOwnedVariable(variableUpdate.Key);
				else
					GetParent().SetVariable(variableUpdate.Key, variableUpdate.Value.OldValue);
			}
		}



		public string Path { get; set; }

        public string Resource { get; set; }

		public string Text { get; set; }


		private class VariableUpdate
		{
			public bool Added { get; private set; }

			public object OldValue { get; private set; }

			public object NewValue { get; private set; }

			public VariableUpdate(bool added, object oldValue, object newValue)
			{
				Added = added;
				OldValue = oldValue;
				NewValue = newValue;
			}
		}
    }
}
