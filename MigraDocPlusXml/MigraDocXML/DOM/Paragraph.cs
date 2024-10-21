using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MigraDocXML.DOM
{
    public class Paragraph : DOMElement
    {
        private MigraDoc.DocumentObjectModel.Paragraph _model;

        public override MigraDoc.DocumentObjectModel.DocumentObject GetModel() => _model;

        public MigraDoc.DocumentObjectModel.Paragraph GetParagraphModel() => _model;


        public override void SetTextValue(string value)
        {
            _model.AddText(value ?? "");
        }


        private void Paragraph_ParentSet(object sender, EventArgs e)
        {
            DOMRelations.Relate(GetPresentableParent(), this);
            ApplyStyling();
        }


        public Paragraph()
        {
            _model = new MigraDoc.DocumentObjectModel.Paragraph();
            NewVariable("Paragraph", this);
            NewVariable("p", this);
            ParentSet += Paragraph_ParentSet;
        }



        private ParagraphFormat _format;
        public ParagraphFormat Format => _format ?? (_format = new ParagraphFormat(_model.Format));
    }
}
