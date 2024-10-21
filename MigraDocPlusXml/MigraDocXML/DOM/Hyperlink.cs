using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MigraDocXML.DOM
{
    public class Hyperlink : DOMElement
    {
        private MigraDoc.DocumentObjectModel.Hyperlink _model;

        public override MigraDoc.DocumentObjectModel.DocumentObject GetModel() => _model;

        public MigraDoc.DocumentObjectModel.Hyperlink GetHyperlinkModel() => _model;


        private void Hyperlink_ParentSet(object sender, EventArgs e)
        {
            DOMRelations.Relate(GetPresentableParent(), this);
            ApplyStyling();
        }


        public override void SetTextValue(string value)
        {
            _model.AddText(value);
        }


        public Hyperlink()
        {
            _model = new MigraDoc.DocumentObjectModel.Hyperlink();
            NewVariable("Hyperlink", this);
            NewVariable("a", this);
            ParentSet += Hyperlink_ParentSet;
        }



        private Font _font;
        public Font Font => _font ?? (_font = new Font(_model.Font));

        public string Name
        {
            get => _model.Name;
            set => _model.Name = value;
        }

        public string Type
        {
            get => _model.Type.ToString();
            set => _model.Type = Parse.Enum<MigraDoc.DocumentObjectModel.HyperlinkType>(value);
        }
    }
}
