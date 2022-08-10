using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MigraDocXML.DOM
{
    public class TextFrame : Shape
    {
        private MigraDoc.DocumentObjectModel.Shapes.TextFrame _model;

        public override MigraDoc.DocumentObjectModel.DocumentObject GetModel() => _model;

        public MigraDoc.DocumentObjectModel.Shapes.TextFrame GetTextFrameModel() => _model;


        private void TextFrame_ParentSet(object sender, EventArgs e)
        {
            DOMRelations.Relate(GetPresentableParent(), this);
            ApplyStyling();
        }


        public override void SetUnknownAttribute(string name, object value)
        {
            if (!ParagraphFormat.AddParagraphFormattingAttribute(this, name, value))
                throw new InvalidOperationException($"Unrecognised attribute {name} on type {GetType().Name}");
        }


        public TextFrame()
        {
            _model = new MigraDoc.DocumentObjectModel.Shapes.TextFrame();
            ShapeModel = _model;
            NewVariable("TextFrame", this);
            ParentSet += TextFrame_ParentSet;
        }



        public Unit MarginBottom { get => new Unit(_model.MarginBottom); set => _model.MarginBottom = value.GetModel(); }

        public Unit MarginLeft { get => new Unit(_model.MarginLeft); set => _model.MarginLeft = value.GetModel(); }

        public Unit MarginRight { get => new Unit(_model.MarginRight); set => _model.MarginRight = value.GetModel(); }

        public Unit MarginTop { get => new Unit(_model.MarginTop); set => _model.MarginTop = value.GetModel(); }

        public string Orientation
        {
            get => _model.Orientation.ToString();
            set => _model.Orientation = Parse.Enum<MigraDoc.DocumentObjectModel.Shapes.TextOrientation>(value);
        }
    }
}
