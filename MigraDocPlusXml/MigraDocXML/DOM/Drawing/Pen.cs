using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MigraDocXML.DOM.Drawing
{
    public class Pen
    {
        private PdfSharp.Drawing.XPen _model;
        public PdfSharp.Drawing.XPen GetModel() => _model;


        public Pen(PdfSharp.Drawing.XPen model)
        {
            _model = model ?? throw new ArgumentNullException(nameof(model));
        }


        public string Color
        {
            get => _model.Color.ToString();
            set => _model.Color = Parse.XColor(value);
        }

        public double DashOffset
        {
            get => _model.DashOffset;
            set => _model.DashOffset = value;
        }
        
        public string DashStyle
        {
            get => _model.DashStyle.ToString();
            set => _model.DashStyle = Parse.Enum<PdfSharp.Drawing.XDashStyle>(value);
        }

        public string LineCap
        {
            get => _model.LineCap.ToString();
            set => _model.LineCap = Parse.Enum<PdfSharp.Drawing.XLineCap>(value);
        }

        public string LineJoin
        {
            get => _model.LineJoin.ToString();
            set => _model.LineJoin = Parse.Enum<PdfSharp.Drawing.XLineJoin>(value);
        }

        public double MiterLimit
        {
            get => _model.MiterLimit;
            set => _model.MiterLimit = value;
        }

        public double Width
        {
            get => _model.Width;
            set => _model.Width = value;
        }
    }
}
