using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MigraDocXML.DOM
{
    public class PageField : DOMElement
    {
        private MigraDoc.DocumentObjectModel.Fields.PageField _model;

        public override MigraDoc.DocumentObjectModel.DocumentObject GetModel() => _model;

        public MigraDoc.DocumentObjectModel.Fields.PageField GetPageFieldModel() => _model;


        private void PageField_ParentSet(object sender, EventArgs e)
        {
            DOMRelations.Relate(GetPresentableParent(), this);
            ApplyStyling();
        }


        public PageField()
        {
            _model = new MigraDoc.DocumentObjectModel.Fields.PageField();
            NewVariable("PageField", this);
            ParentSet += PageField_ParentSet;
        }



        public string Format { get => _model.Format; set => _model.Format = value; }
    }
}
