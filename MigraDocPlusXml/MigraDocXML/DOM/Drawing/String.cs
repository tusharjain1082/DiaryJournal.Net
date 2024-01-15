using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MigraDocXML.DOM.Drawing
{
    public class String : GraphicalElement
    {
        public String()
        {
            NewVariable("String", this);
            ParentSet += String_ParentSet;
            FullyBuilt += String_FullyBuilt;
        }

        private void String_ParentSet(object sender, EventArgs e)
        {
            DOMRelations.Relate(GetGraphicalParent(), this);
            Page = GetGraphics()?.Page;
        }

        private void String_FullyBuilt(object sender, EventArgs e)
        {
            if (Page == null)
                throw new InvalidOperationException("Not specified which page to add rectangle to");

            PdfSharp.Drawing.XGraphics canvas = GetDocument().GetPageCanvas(Page.Value);
            if (canvas == null)
                return;

            if (_point != null)
                canvas.DrawString(Value, Font.GetModel(), Brush.GetModel(), Point.GetPointModel(), _format);
            else if (_area != null)
			{
				var rectHeight = (_format.LineAlignment == PdfSharp.Drawing.XLineAlignment.BaseLine) ? 0 : _area.Height.Points;
                canvas.DrawString(Value, Font.GetModel(), Brush.GetModel(), new PdfSharp.Drawing.XRect(_area.Left.Points, _area.Top.Points, _area.Width.Points, rectHeight), _format);
			}
            else
                throw new Exception("Either Point or Rect must be set on String");
        }

        public override void SetTextValue(string value)
        {
            Value = value;
        }


        public int? Page { get; set; }

        private Brush _brush;
        public Brush Brush => _brush ?? (_brush = new Brush() { Color = "Black" });

		private XFont _font;
		public XFont Font => _font ?? (_font = new XFont());

        private Point _point;
        public Point Point
        {
            get
            {
                _area = null;
                return _point = _point ?? (_point = new Point());
            }
        }

        private RenderArea _area;
        public RenderArea Area
        {
			set
			{
				_point = null;
				_area = value;
				Page = value?.Page;
			}
        }

        public string Value { get; set; }

		private PdfSharp.Drawing.XStringFormat _format = PdfSharp.Drawing.XStringFormats.Default;
		public string Format
		{
			get => _format.ToString();
			set
			{
				switch (value)
				{
					case "BaseLineCenter": _format = PdfSharp.Drawing.XStringFormats.BaseLineCenter; break;
					case "BaseLineLeft": _format = PdfSharp.Drawing.XStringFormats.BaseLineLeft; break;
					case "BaseLineRight": _format = PdfSharp.Drawing.XStringFormats.BaseLineRight; break;
					case "BottomCenter": _format = PdfSharp.Drawing.XStringFormats.BottomCenter; break;
					case "BottomLeft": _format = PdfSharp.Drawing.XStringFormats.BottomLeft; break;
					case "BottomRight": _format = PdfSharp.Drawing.XStringFormats.BottomRight; break;
					case "Center": _format = PdfSharp.Drawing.XStringFormats.Center; break;
					case "CenterLeft": _format = PdfSharp.Drawing.XStringFormats.CenterLeft; break;
					case "CenterRight": _format = PdfSharp.Drawing.XStringFormats.CenterRight; break;
					case "Default": _format = PdfSharp.Drawing.XStringFormats.Default; break;
					case "TopCenter": _format = PdfSharp.Drawing.XStringFormats.TopCenter; break;
					case "TopLeft": _format = PdfSharp.Drawing.XStringFormats.TopLeft; break;
					case "TopRight": _format = PdfSharp.Drawing.XStringFormats.TopRight; break;
					default: throw new ArgumentException("Unrecognised String Format");
				}
			}
		}
    }
}
