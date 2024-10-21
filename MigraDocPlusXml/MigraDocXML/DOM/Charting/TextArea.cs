using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MigraDocXML.DOM.Charting
{
    public class TextArea : DOMElement
    {
        private MigraDoc.DocumentObjectModel.Shapes.Charts.TextArea _model;

        public override MigraDoc.DocumentObjectModel.DocumentObject GetModel() => _model;

        public MigraDoc.DocumentObjectModel.Shapes.Charts.TextArea GetTextAreaModel() => _model;

        public void SetTextAreaModel(MigraDoc.DocumentObjectModel.Shapes.Charts.TextArea model) => _model = model;


        public TextArea()
        {
            ParentSet += TextArea_ParentSet;
        }


        private void TextArea_ParentSet(object sender, EventArgs e)
        {
            DOMRelations.Relate(GetPresentableParent(), this);
            ApplyStyling();
        }



        public Unit BottomPadding { get => new Unit(_model.BottomPadding); set => _model.BottomPadding = value.GetModel(); }

        private FillFormat _fillFormat;
        public FillFormat FillFormat => _fillFormat ?? (_fillFormat = new FillFormat(_model.FillFormat));

        private ParagraphFormat _format;
        public ParagraphFormat Format => _format ?? (_format = new ParagraphFormat(_model.Format));

        public Unit Height { get => new Unit(_model.Height); set => _model.Height = value.GetModel(); }

        public Unit LeftPadding { get => new Unit(_model.LeftPadding); set => _model.LeftPadding = value.GetModel(); }

        private LineFormat _lineFormat;
        public LineFormat LineFormat => _lineFormat ?? (_lineFormat = new LineFormat(_model.LineFormat));

        public Unit RightPadding { get => new Unit(_model.RightPadding); set => _model.RightPadding = value.GetModel(); }

        public Unit TopPadding { get => new Unit(_model.TopPadding); set => _model.TopPadding = value.GetModel(); }

        public string VerticalAlignment
        {
            get => _model.VerticalAlignment.ToString();
            set => _model.VerticalAlignment = Parse.Enum<MigraDoc.DocumentObjectModel.Tables.VerticalAlignment>(value);
        }

        public Unit Width { get => new Unit(_model.Width); set => _model.Width = value.GetModel(); }
    }



    public class BottomArea : TextArea { }

    public class FooterArea : TextArea { }

    public class HeaderArea : TextArea { }

    public class LeftArea : TextArea { }

    public class RightArea : TextArea { }

    public class TopArea : TextArea { }
}
