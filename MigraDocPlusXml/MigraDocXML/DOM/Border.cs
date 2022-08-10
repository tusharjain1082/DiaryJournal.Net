using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MigraDocXML.DOM
{
    public class Border
    {
        private MigraDoc.DocumentObjectModel.Border _model;
        public MigraDoc.DocumentObjectModel.Border GetModel() => _model;


        public Border(MigraDoc.DocumentObjectModel.Border model)
        {
            _model = model ?? throw new ArgumentNullException(nameof(model));
        }



        public string Color { get => _model.Color.ToString(); set => _model.Color = Parse.Color(value); }

        public string Name => _model.Name;

        public bool Visible { get => _model.Visible; set => _model.Visible = value; }

        public Unit Width { get => new Unit(_model.Width); set => _model.Width = value.GetModel(); }
    }
}
