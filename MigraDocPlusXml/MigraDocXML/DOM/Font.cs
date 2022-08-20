using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MigraDocXML.DOM
{
    public class Font
    {
        private MigraDoc.DocumentObjectModel.Font _model;
        public MigraDoc.DocumentObjectModel.Font GetModel() => _model;


        public Font(MigraDoc.DocumentObjectModel.Font model)
        {
            _model = model ?? throw new ArgumentNullException(nameof(model));
        }



        public bool Bold { get => _model.Bold; set => _model.Bold = value; }

        public bool Italic { get => _model.Italic; set => _model.Italic = value; }

        public Unit Size { get => new Unit(_model.Size); set => _model.Size = value.GetModel(); }

        public string Color { get => _model.Color.ToString(); set => _model.Color = Parse.Color(value); }

        public string Name { get => _model.Name; set => _model.Name = value; }

        public bool Subscript { get => _model.Subscript; set => _model.Subscript = value; }

        public bool Superscript { get => _model.Superscript; set => _model.Superscript = value; }

        public string Underline
        {
            get => _model.Underline.ToString();
            set => _model.Underline = Parse.Enum<MigraDoc.DocumentObjectModel.Underline>(value);
        }
    }
}
