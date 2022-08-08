using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MigraDocXML.DOM
{
    public class Footer : DOMElement
    {
        private MigraDoc.DocumentObjectModel.HeaderFooter _model;

        public override MigraDoc.DocumentObjectModel.DocumentObject GetModel() => _model;

        public MigraDoc.DocumentObjectModel.HeaderFooter GetFooterModel() => _model;


        private void Footer_ParentSet(object sender, EventArgs e)
        {
            ApplyStyling();
        }

        private void Footer_FullyBuilt(object sender, EventArgs e)
        {
            DOMRelations.Relate(GetPresentableParent(), this);
        }


        public Footer()
        {
            _model = new MigraDoc.DocumentObjectModel.HeaderFooter();
            NewVariable("Footer", this);
            ParentSet += Footer_ParentSet;
            FullyBuilt += Footer_FullyBuilt;
        }



        public bool IsEvenPage { get; set; }

        public bool IsFirstPage { get; set; }

        public bool IsPrimary { get; set; }
    }
}
