using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace MigraDocXML.DOM.Drawing
{
	public class Rect : GraphicalElement
	{
		public Rect()
		{
			NewVariable("Rect", this);
			ParentSet += Rect_ParentSet;
			FullyBuilt += Rect_FullyBuilt;
		}

		private void Rect_ParentSet(object sender, EventArgs e)
		{
			DOMRelations.Relate(GetGraphicalParent(), this);
			Page = GetGraphics()?.Page;
		}

		public PdfSharp.Drawing.XRect GetXRect()
		{
			return new PdfSharp.Drawing.XRect(Left.Points, Top.Points, Width.Points, Height.Points);
		}

		private void Rect_FullyBuilt(object sender, EventArgs e)
		{
			if (Page == null)
				throw new InvalidOperationException("Not specified which page to add rectangle to");

			PdfSharp.Drawing.XGraphics canvas = GetDocument().GetPageCanvas(Page.Value);
			if (canvas == null)
				return;

			if (CornerX == null || CornerY == null)
			{
				if (_brush == null)
					canvas.DrawRectangle(Pen.GetModel(), GetXRect());
				else
					canvas.DrawRectangle(Pen.GetModel(), _brush.GetModel(GetXRect()), GetXRect());
			}
			else
			{
				if (_brush == null)
					canvas.DrawRoundedRectangle(Pen.GetModel(), Left.Points, Top.Points, Width.Points, Height.Points, CornerX.Points * 2, CornerY.Points * 2);
				else
					canvas.DrawRoundedRectangle(Pen.GetModel(), _brush.GetModel(GetXRect()), Left.Points, Top.Points, Width.Points, Height.Points, CornerX.Points * 2, CornerY.Points * 2);
			}
		}

		public override List<XmlAttribute> ArrangeAttributes(IEnumerable<XmlAttribute> attributes)
		{
			var list = attributes.ToList();

			var area = list.FirstOrDefault(x => x.Name == "Area");
			if(area != null)
			{
				list.Remove(area);
				list.Insert(0, area);
			}

			var right = list.FirstOrDefault(x => x.Name == "Right");
			if(right != null)
			{
				list.Remove(right);
				list.Add(right);
			}

			var bottom = list.FirstOrDefault(x => x.Name == "Bottom");
			if(bottom != null)
			{
				list.Remove(bottom);
				list.Add(bottom);
			}

			return list;
		}


		private Pen _pen;
		public Pen Pen => _pen ?? (_pen = new Pen(new PdfSharp.Drawing.XPen(PdfSharp.Drawing.XColors.Black)));

		private Brush _brush;
		public Brush Brush => _brush ?? (_brush = new Brush());

		public int? Page { get; set; }
		
		public Unit Left { get; set; }
		
		public Unit Width { get; set; }

		public Unit Right
		{
			get => Left + Width;
			set
			{
				if (value == null)
					throw new InvalidOperationException("Right attribute must be set to a non-null value");
				if (Left == null && Width == null)
					throw new InvalidOperationException("Cannot set Right attribute without setting either Left or Width first");
				if (Left != null)
					Width = value - Left;
				else
					Left = value - Width;
			}
		}
		
		public Unit Top { get; set; }
		
		public Unit Height { get; set; }
		
		public Unit Bottom
		{
			get => Top + Height;
			set
			{
				if (value == null)
					throw new InvalidOperationException("Bottom attribute must be set to a non-null value");
				if (Top == null && Height == null)
					throw new InvalidOperationException("Cannot set Bottom attribute without setting either Top or Height first");
				if (Top != null)
					Height = value - Top;
				else
					Top = value - Height;
			}
		}

		public Unit Corner
		{
			set
			{
				CornerX = value;
				CornerY = value;
			}
		}

		public Unit CornerX { get; set; }

		public Unit CornerY { get; set; }

		public RenderArea Area
		{
			set
			{
				Top = value?.Top;
				Left = value?.Left;
				Right = value?.Right;
				Bottom = value?.Bottom;
				Page = value?.Page;
			}
		}
	}
}
