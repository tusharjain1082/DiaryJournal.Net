using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MigraDocXML.DOM.Drawing
{
    public class Point : GraphicalElement
    {
		public PdfSharp.Drawing.XPoint GetPointModel() => new PdfSharp.Drawing.XPoint(X.Points, Y.Points);

		public Point()
		{
			NewVariable("Point", this);
			ParentSet += Point_ParentSet;
		}

		private void Point_ParentSet(object sender, EventArgs e)
		{
			DOMRelations.Relate(GetGraphicalParent(), this);
		}


		public Unit X { get; set; }

        public Unit Y { get; set; }
    }
}
