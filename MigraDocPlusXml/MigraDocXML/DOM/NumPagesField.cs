using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MigraDocXML.DOM
{
    public class NumPagesField : DOMElement
    {
        private MigraDoc.DocumentObjectModel.Fields.NumPagesField _model;

        public override MigraDoc.DocumentObjectModel.DocumentObject GetModel() => _model;

        public MigraDoc.DocumentObjectModel.Fields.NumPagesField GetNumPagesFieldModel() => _model;


        private void NumPagesField_ParentSet(object sender, EventArgs e)
        {
            DOMRelations.Relate(GetPresentableParent(), this);
            ApplyStyling();
        }


        public NumPagesField()
        {
            _model = new MigraDoc.DocumentObjectModel.Fields.NumPagesField();
            NewVariable("NumPagesField", this);
            ParentSet += NumPagesField_ParentSet;
        }



        public string Format { get => _model.Format; set => _model.Format = value; }
    }
}
