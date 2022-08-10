using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MigraDocXML.DOM.Drawing
{
    public class Bezier : GraphicalElement
    {
        public Bezier()
        {
            NewVariable("Bezier", this);
            ParentSet += Bezier_ParentSet;
            FullyBuilt += Bezier_FullyBuilt;
        }

        private void Bezier_ParentSet(object sender, EventArgs e)
        {
            DOMRelations.Relate(GetGraphicalParent(), this);
            Page = GetGraphics()?.Page;
        }

        private void Bezier_FullyBuilt(object sender, EventArgs e)
        {
            if (Page == null)
                throw new InvalidOperationException("Not specified which page to add bezier to");

            var canvas = GetDocument().GetPageCanvas(Page.Value);
            if (canvas == null)
                return;

            var points = GetAllDescendents().OfType<Point>().ToList();
            if (points.Count <= 1)
                return;
            canvas.DrawBeziers(Pen.GetModel(), points.Select(x => x.GetPointModel()).ToArray());
        }


        private Pen _pen;
        public Pen Pen => _pen ?? (_pen = new Pen(new PdfSharp.Drawing.XPen(PdfSharp.Drawing.XColors.Black)));

        public int? Page { get; set; }
    }
}
