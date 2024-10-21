using MigraDocXML.Exceptions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Xml;

namespace MigraDocXML.DOM
{
    public abstract class DOMElement
    {
        public abstract MigraDoc.DocumentObjectModel.DocumentObject GetModel();


        private List<DOMElement> _children = new List<DOMElement>();
        public IEnumerable<DOMElement> Children { get; private set; }

        public event DOMElementChildEventHandler ChildAdded;

        public event DOMElementChildEventHandler ChildRemoved;

        public void AddChild(DOMElement child)
        {
            _children.Add(child);
            if (child._parent != null)
                child._parent.RemoveChild(child);
            child._parent = this;
            ChildAdded?.Invoke(this, new DOMElementChildEventArgs(child));
            child.ParentSet?.Invoke(this, null);
        }

        public void RemoveChild(DOMElement child)
        {
            if (_children.Remove(child))
            {
                child._parent = null;
                ChildRemoved?.Invoke(this, new DOMElementChildEventArgs(child));
            }
        }

		public List<DOMElement> GetAllDescendents()
		{
			var output = new List<DOMElement>();
			PopulateAllDescendents(output);
			return output;
		}

		private void PopulateAllDescendents(List<DOMElement> descendents)
		{
			foreach(var child in Children)
			{
				descendents.Add(child);
				child.PopulateAllDescendents(descendents);
			}
		}


        private DOMElement _parent;

        public DOMElement GetParent() => _parent;

        public void SetParent(DOMElement parent)
        {
            if (parent == null)
            {
                if (_parent != null)
                    _parent.RemoveChild(this);
            }
            else
                parent.AddChild(this);
        }

        public DOMElement GetPresentableParent()
        {
            var parent = GetParent();
            while (parent != null && parent.GetParent() != null && !parent.IsPresentable)
                parent = parent.GetParent();
            return parent;
        }

        public virtual Document GetDocument()
        {
            return GetParent().GetDocument();
        }

        public event EventHandler ParentSet;


		/// <summary>
		/// Identifies that an element represents one of Migradoc's presentable elements, eg, paragraph, table, image, etc.
		/// </summary>
        public bool IsPresentable { get; protected set; } = true;

		/// <summary>
		/// Identifies that an element represents a logical element, such as a loop, conditional, variable declaration, etc.
		/// </summary>
        public bool IsLogical { get; protected set; } = false;

		/// <summary>
		/// Identifies that an element is part of the post-rendering graphics process
		/// </summary>
		public bool IsGraphical { get; protected set; } = false;


        private Dictionary<string, object> _variables = new Dictionary<string, object>();

        public bool OwnsVariable(string name) => _variables.ContainsKey(name);

        public bool InheritsVariable(string name) => OwnsVariable(name) || (GetParent()?.InheritsVariable(name) == true);

        public object GetVariable(string name)
        {
            if (name == "Parent")
                return GetPresentableParent();

            if (OwnsVariable(name))
                return _variables[name];

            if (!InheritsVariable(name))
                throw new UndefinedVariableException($"Unrecognised variable '{name}'");
			
            return GetParent().GetVariable(name);
        }

		/// <summary>
		/// Returns a list of the variables directly owned by this element
		/// </summary>
		public IEnumerable<KeyValuePair<string, object>> GetOwnedVariables() => _variables.ToList().AsReadOnly();

        /// <summary>
        /// Add a new variable to this element
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void NewVariable(string name, object value)
        {
            _variables[name] = value;
        }

        /// <summary>
        /// Set the value of an existing variable, throw an exception if it's not found
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void SetVariable(string name, object value)
        {
            if (_variables.ContainsKey(name))
                _variables[name] = value;

            else if (GetParent() == null)
                throw new UndefinedVariableException($"Unrecognised variable '{name}'");

            else
                GetParent()?.SetVariable(name, value);
        }

		/// <summary>
		/// Attempts to remove a variable owned by this element
		/// </summary>
		/// <param name="name"></param>
		public void RemoveOwnedVariable(string name)
		{
			if (_variables.ContainsKey(name))
				_variables.Remove(name);
		}


        public string Style { get; set; }

        protected void ApplyStyling()
        {
            ApplyStyling(this, this);
        }
        
        public void ApplyStyling(DOMElement obj, DOMElement styleHolder)
        {
            if (styleHolder.GetParent() != null)
                ApplyStyling(obj, styleHolder.GetParent());

            ApplyUnnamedStyles(obj, styleHolder);
            if (!string.IsNullOrWhiteSpace(obj.Style))
                ApplyNamedStyles(obj, styleHolder);
        }
        
        private void ApplyUnnamedStyles(DOMElement obj, DOMElement styleHolder)
        {
            if (styleHolder.GetParent() != null)
                ApplyUnnamedStyles(obj, styleHolder.GetParent());

            Type objType = obj.GetType();

			var styles = styleHolder.Children.OfType<Style>().Where(x => string.IsNullOrEmpty(x.Name)).ToList();
			foreach (var style in styles)
            {
                if (style.TestMatch(obj))
                {
                    foreach (var setter in style.Setters.GetItems())
                        PdfXmlReader.SetPropertyFromAttribute(obj, setter.Key, PdfXmlReader.InterpretString(setter.Value, obj)?.ToString());
                }
            }
        }
        
        private void ApplyNamedStyles(DOMElement obj, DOMElement styleHolder)
        {
            if (styleHolder.GetParent() != null)
                ApplyNamedStyles(obj, styleHolder.GetParent());

            Type objType = obj.GetType();

			var styles = styleHolder.Children.OfType<Style>().Where(x => !string.IsNullOrEmpty(x.Name)).ToList();
			foreach (var style in styles)
            {
                if (style.TestMatch(obj))
                {
                    foreach (var setter in style.Setters.GetItems())
                        PdfXmlReader.SetPropertyFromAttribute(obj, setter.Key, PdfXmlReader.InterpretString(setter.Value, obj)?.ToString());
                }
            }
        }


        /// <summary>
        /// Used for handling attributes that don't match up to a property on the object
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public virtual void SetUnknownAttribute(string name, object value)
        {
            throw new Exception($"Unrecognised attribute '{name}' on {GetType().Name}");
        }


        public virtual void SetTextValue(string value)
        {
            throw new NotImplementedException();
        }


        public event EventHandler FullyBuilt;

        public void DispatchFullyBuilt()
        {
            FullyBuilt?.Invoke(this, null);
        }


        /// <summary>
        /// Returns the order in which attributes should be processed, since for some element types (namely Row) this is important
        /// </summary>
        /// <param name="attributes"></param>
        /// <returns></returns>
        public virtual List<XmlAttribute> ArrangeAttributes(IEnumerable<XmlAttribute> attributes)
        {
            return attributes.ToList();
        }


        public DOMElement()
        {
            Children = new ReadOnlyCollection<DOMElement>(_children);
        }



        public string Tag { get => GetModel().Tag?.ToString(); set => GetModel().Tag = value; }
    }




    public class DOMElementChildEventArgs
    {
        public DOMElement Child { get; private set; }

        public DOMElementChildEventArgs(DOMElement child)
        {
            Child = child ?? throw new ArgumentNullException(nameof(child));
        }
    }

    public delegate void DOMElementChildEventHandler(object sender, DOMElementChildEventArgs args);
}
