using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MigraDocXML.DOM
{
    public class Text : DOMElement
    {
        private MigraDoc.DocumentObjectModel.Text _model;

        public override MigraDoc.DocumentObjectModel.DocumentObject GetModel() => _model;

        public MigraDoc.DocumentObjectModel.Text GetTextModel() => _model;


        private void Text_ParentSet(object sender, EventArgs e)
        {
            DOMRelations.Relate(GetPresentableParent(), this);
        }


        public override void SetTextValue(string value)
        {
            _model.Content = value;
        }


        public Text()
        {
            _model = new MigraDoc.DocumentObjectModel.Text();
            ParentSet += Text_ParentSet;
        }
    }
}
