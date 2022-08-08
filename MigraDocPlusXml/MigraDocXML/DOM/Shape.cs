using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MigraDocXML.DOM
{
    public abstract class Shape : DOMElement
    {
        protected MigraDoc.DocumentObjectModel.Shapes.Shape ShapeModel { get; set; }



        private FillFormat _fillFormat;
        public FillFormat FillFormat => _fillFormat ?? (_fillFormat = new FillFormat(ShapeModel.FillFormat));

        public Unit Height { get => new Unit(ShapeModel.Height); set => ShapeModel.Height = value.GetModel(); }

        public string Left { get => ShapeModel.Left.ToString(); set => ShapeModel.Left = Parse.ShapePosition(value); }

        private LineFormat _lineFormat;
        public LineFormat LineFormat => _lineFormat ?? (_lineFormat = new LineFormat(ShapeModel.LineFormat));

        public string RelativeHorizontal
        {
            get => ShapeModel.RelativeHorizontal.ToString();
            set => ShapeModel.RelativeHorizontal = Parse.Enum<MigraDoc.DocumentObjectModel.Shapes.RelativeHorizontal>(value);
        }

        public string RelativeVertical
        {
            get => ShapeModel.RelativeVertical.ToString();
            set => ShapeModel.RelativeVertical = Parse.Enum<MigraDoc.DocumentObjectModel.Shapes.RelativeVertical>(value);
        }

        public string Top { get => ShapeModel.Top.ToString(); set => ShapeModel.Top = Parse.ShapePosition(value); }

        public Unit Width { get => new Unit(ShapeModel.Width); set => ShapeModel.Width = value.GetModel(); }

        private WrapFormat _wrapFormat;
        public WrapFormat WrapFormat => _wrapFormat ?? (_wrapFormat = new WrapFormat(ShapeModel.WrapFormat));
    }
}
