using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MigraDocXML.DOM
{
    public class Shading
    {
        private MigraDoc.DocumentObjectModel.Shading _model;
        public MigraDoc.DocumentObjectModel.Shading GetModel() => _model;


        public Shading(MigraDoc.DocumentObjectModel.Shading model)
        {
            _model = model ?? throw new ArgumentNullException(nameof(model));
        }



        public string Color { get => _model.Color.ToString(); set => _model.Color = Parse.Color(value); }

        public bool Visible { get => _model.Visible; set => _model.Visible = value; }
    }
}
