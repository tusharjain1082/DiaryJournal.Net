using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MigraDocXML.DOM
{
    public class FillFormat
    {
        private MigraDoc.DocumentObjectModel.Shapes.FillFormat _model;
        public MigraDoc.DocumentObjectModel.Shapes.FillFormat GetModel() => _model;


        public FillFormat(MigraDoc.DocumentObjectModel.Shapes.FillFormat model)
        {
            _model = model ?? throw new ArgumentNullException(nameof(model));
        }



        public string Color
        {
            get => _model.Color.ToString();
            set => _model.Color = Parse.Color(value);
        }

        public bool Visible { get => _model.Visible; set => _model.Visible = value; }
    }
}
