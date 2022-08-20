using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MigraDoc.DocumentObjectModel;

namespace MigraDocXML.DOM.Drawing
{
	/// <summary>
	/// Base class for all elements that are designed to aid in drawing graphics in the post-rendering phase
	/// </summary>
	public abstract class GraphicalElement : DOMElement
	{
		public GraphicalElement()
		{
			IsGraphical = true;
			IsPresentable = false;
		}

		public override DocumentObject GetModel() => throw new NotImplementedException();

		public DOMElement GetGraphicalParent()
		{
			var parent = GetParent();
			while (parent != null && parent.GetParent() != null && !parent.IsGraphical)
				parent = parent.GetParent();
			return parent;
		}

		public Graphics GetGraphics()
		{
			var parent = GetParent();
			while (true)
			{
				if (parent == null || parent.IsPresentable)
					return null;
				if (parent is Graphics)
					return parent as Graphics;
				parent = parent.GetParent();
			}
		}
	}
}
