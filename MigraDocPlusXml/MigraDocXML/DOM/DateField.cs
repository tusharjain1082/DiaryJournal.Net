using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MigraDocXML.DOM
{
    public class DateField : DOMElement
    {
        private MigraDoc.DocumentObjectModel.Fields.DateField _model;

        public override MigraDoc.DocumentObjectModel.DocumentObject GetModel() => _model;

        public MigraDoc.DocumentObjectModel.Fields.DateField GetDateFieldModel() => _model;


        private void DateField_ParentSet(object sender, EventArgs e)
        {
            DOMRelations.Relate(GetPresentableParent(), this);
            ApplyStyling();
        }


        public DateField()
        {
            _model = new MigraDoc.DocumentObjectModel.Fields.DateField();
            NewVariable("DateField", this);
            ParentSet += DateField_ParentSet;
        }



        public string Format { get => _model.Format; set => _model.Format = value; }
    }
}
