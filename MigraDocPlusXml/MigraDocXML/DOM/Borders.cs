using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MigraDocXML.DOM
{
    public class Borders
    {
        private MigraDoc.DocumentObjectModel.Borders _model;
        public MigraDoc.DocumentObjectModel.Borders GetModel() => _model;


        public Borders(MigraDoc.DocumentObjectModel.Borders model)
        {
            _model = model ?? throw new ArgumentNullException(nameof(model));
        }



        private Border _bottom;
        public Border Bottom => _bottom ?? (_bottom = new Border(_model.Bottom));

        public string Color { get => _model.Color.ToString(); set => _model.Color = Parse.Color(value); }

        private Border _diagonalDown;
        public Border DiagonalDown => _diagonalDown ?? (_diagonalDown = new Border(_model.DiagonalDown));

        private Border _diagonalUp;
        public Border DiagonalUp => _diagonalUp ?? (_diagonalUp = new Border(_model.DiagonalUp));

        public Unit Distance { set => _model.Distance = value.GetModel(); }

        public Unit DistanceFromBottom { get => new Unit(_model.DistanceFromBottom); set => _model.DistanceFromBottom = value.GetModel(); }

        public Unit DistanceFromLeft { get => new Unit(_model.DistanceFromLeft); set => _model.DistanceFromLeft = value.GetModel(); }

        public Unit DistanceFromRight { get => new Unit(_model.DistanceFromRight); set => _model.DistanceFromRight = value.GetModel(); }

        public Unit DistanceFromTop { get => new Unit(_model.DistanceFromTop); set => _model.DistanceFromTop = value.GetModel(); }

        private Border _left;
        public Border Left => _left ?? (_left = new Border(_model.Left));

        private Border _right;
        public Border Right => _right ?? (_right = new Border(_model.Right));

        private Border _top;
        public Border Top => _top ?? (_top = new Border(_model.Top));

        public bool Visible { get => _model.Visible; set => _model.Visible = value; }

        public Unit Width { get => new Unit(_model.Width); set => _model.Width = value.GetModel(); }
    }
}
