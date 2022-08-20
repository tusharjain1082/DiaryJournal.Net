using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MigraDocXML.DOM.Drawing
{
    public class Line : GraphicalElement
	{
		public Line()
		{
			NewVariable("Line", this);
			ParentSet += Line_ParentSet;
			FullyBuilt += Line_FullyBuilt;
		}

		private void Line_ParentSet(object sender, EventArgs e)
		{
			DOMRelations.Relate(GetGraphicalParent(), this);
			Page = GetGraphics()?.Page;
		}

		private void Line_FullyBuilt(object sender, EventArgs e)
		{
			if (Page == null)
				throw new InvalidOperationException("Not specified which page to add line to");

			var canvas = GetDocument().GetPageCanvas(Page.Value);
			if (canvas == null)
				return;

			var points = GetAllDescendents().OfType<Point>().ToList();
			if (points.Count <= 1)
				return;
			canvas.DrawLines(Pen.GetModel(), points.Select(x => x.GetPointModel()).ToArray());
		}


		private Pen _pen;
        public Pen Pen => _pen ?? (_pen = new Pen(new PdfSharp.Drawing.XPen(PdfSharp.Drawing.XColors.Black)));

		public int? Page { get; set; }
    }
}
