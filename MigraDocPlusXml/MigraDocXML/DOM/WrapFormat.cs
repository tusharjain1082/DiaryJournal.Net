using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MigraDocXML.DOM
{
    public class WrapFormat
    {
        private MigraDoc.DocumentObjectModel.Shapes.WrapFormat _model;
        public MigraDoc.DocumentObjectModel.Shapes.WrapFormat GetModel() => _model;


        public WrapFormat(MigraDoc.DocumentObjectModel.Shapes.WrapFormat model)
        {
            _model = model ?? throw new ArgumentNullException(nameof(model));
        }



        public Unit DistanceBottom
        {
            get => new Unit(_model.DistanceBottom);
            set => _model.DistanceBottom = value.GetModel();
        }

        public Unit DistanceLeft
        {
            get => new Unit(_model.DistanceLeft);
            set => _model.DistanceLeft = value.GetModel();
        }

        public Unit DistanceRight
        {
            get => new Unit(_model.DistanceRight);
            set => _model.DistanceRight = value.GetModel();
        }

        public Unit DistanceTop
        {
            get => new Unit(_model.DistanceTop);
            set => _model.DistanceTop = value.GetModel();
        }

        public string Style
        {
            get => _model.Style.ToString();
            set => Parse.Enum<MigraDoc.DocumentObjectModel.Shapes.WrapStyle>(value);
        }
    }
}
