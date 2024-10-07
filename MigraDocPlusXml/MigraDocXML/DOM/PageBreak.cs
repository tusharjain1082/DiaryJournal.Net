using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MigraDocXML.DOM
{
    public class PageBreak : DOMElement
    {
        private MigraDoc.DocumentObjectModel.PageBreak _model;

        public override MigraDoc.DocumentObjectModel.DocumentObject GetModel() => _model;

        public MigraDoc.DocumentObjectModel.PageBreak GetPageBreakModel() => _model;


        private void PageBreak_ParentSet(object sender, EventArgs e)
        {
            DOMRelations.Relate(GetPresentableParent(), this);
        }


        public PageBreak()
        {
            _model = new MigraDoc.DocumentObjectModel.PageBreak();
            NewVariable("PageBreak", this);
            ParentSet += PageBreak_ParentSet;
        }
    }
}
