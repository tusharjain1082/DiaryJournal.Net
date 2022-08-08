using MigraDocXML.DOM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MigraDocXML
{
	public class RenderArea
	{
		public int Page { get; private set; }

		public Unit Top { get; private set; }

		public Unit Left { get; private set; }

		public Unit Width { get; private set; }

		public Unit Height { get; private set; }

		public Unit Right => Left + Width;

		public Unit Bottom => Top + Height;

		public Unit CenterX => Left + (Width / 2);

		public Unit CenterY => Top + (Height / 2);

		public RenderArea(int page, Unit top, Unit left, Unit width, Unit height)
		{
			Page = page;
			Top = top;
			Left = left;
			Width = width;
			Height = height;
		}

		public RenderArea(int page, double top, double left, double width, double height)
		{
			Page = page;
			Top = new Unit(top, MigraDoc.DocumentObjectModel.UnitType.Point);
			Left = new Unit(left, MigraDoc.DocumentObjectModel.UnitType.Point);
			Width = new Unit(width, MigraDoc.DocumentObjectModel.UnitType.Point);
			Height = new Unit(height, MigraDoc.DocumentObjectModel.UnitType.Point);
		}
	}
}
