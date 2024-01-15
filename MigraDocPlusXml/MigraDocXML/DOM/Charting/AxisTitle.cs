using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MigraDocXML.DOM.Charting
{
    public class AxisTitle
    {
        private MigraDoc.DocumentObjectModel.Shapes.Charts.AxisTitle _model;
        public MigraDoc.DocumentObjectModel.Shapes.Charts.AxisTitle GetModel() => _model;


        public AxisTitle(MigraDoc.DocumentObjectModel.Shapes.Charts.AxisTitle model)
        {
            _model = model ?? throw new ArgumentNullException(nameof(model));
        }



        public string Alignment
        {
            get => _model.Alignment.ToString();
            set => _model.Alignment = Parse.Enum<MigraDoc.DocumentObjectModel.Shapes.Charts.HorizontalAlignment>(value);
        }

        public string Caption { get => _model.Caption; set => _model.Caption = value; }

        private Font _font;
        public Font Font => _font ?? (_font = new Font(_model.Font));

        public Unit Orientation { get => new Unit(_model.Orientation); set => _model.Orientation = value.GetModel(); }

        public string VerticalAlignment
        {
            get => _model.VerticalAlignment.ToString();
            set => _model.VerticalAlignment = Parse.Enum<MigraDoc.DocumentObjectModel.Tables.VerticalAlignment>(value);
        }
    }
}
