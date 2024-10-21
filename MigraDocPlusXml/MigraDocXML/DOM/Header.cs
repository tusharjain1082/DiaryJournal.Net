using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MigraDocXML.DOM
{
    public class Header : DOMElement
    {
        private MigraDoc.DocumentObjectModel.HeaderFooter _model;

        public override MigraDoc.DocumentObjectModel.DocumentObject GetModel() => _model;

        public MigraDoc.DocumentObjectModel.HeaderFooter GetHeaderModel() => _model;


        private void Header_ParentSet(object sender, EventArgs e)
        {
            ApplyStyling();
        }

        private void Header_FullyBuilt(object sender, EventArgs e)
        {
            DOMRelations.Relate(GetPresentableParent(), this);
        }


        public Header()
        {
            _model = new MigraDoc.DocumentObjectModel.HeaderFooter();
            NewVariable("Header", this);
            ParentSet += Header_ParentSet;
            FullyBuilt += Header_FullyBuilt;
        }



        public bool IsEvenPage { get; set; }

        public bool IsFirstPage { get; set; }

        public bool IsPrimary { get; set; }
    }
}
