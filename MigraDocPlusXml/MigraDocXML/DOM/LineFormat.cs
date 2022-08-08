using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MigraDocXML.DOM
{
    public class LineFormat
    {
        private MigraDoc.DocumentObjectModel.Shapes.LineFormat _model;
        public MigraDoc.DocumentObjectModel.Shapes.LineFormat GetModel() => _model;


        public LineFormat(MigraDoc.DocumentObjectModel.Shapes.LineFormat model)
        {
            _model = model ?? throw new ArgumentNullException(nameof(model));
        }



        public string Color
        {
            get => _model.Color.ToString();
            set => _model.Color = Parse.Color(value);
        }

        public string DashStyle
        {
            get => _model.DashStyle.ToString();
            set => _model.DashStyle = Parse.Enum<MigraDoc.DocumentObjectModel.Shapes.DashStyle>(value);
        }

        public bool Visible { get => _model.Visible; set => _model.Visible = value; }

        public Unit Width { get => new Unit(_model.Width); set => _model.Width = value.Value; }
    }
}
