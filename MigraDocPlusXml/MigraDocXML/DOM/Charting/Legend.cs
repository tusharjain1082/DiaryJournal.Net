using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MigraDocXML.DOM.Charting
{
    public class Legend : DOMElement
    {
        private MigraDoc.DocumentObjectModel.Shapes.Charts.Legend _model;

        public override MigraDoc.DocumentObjectModel.DocumentObject GetModel() => _model;

        public MigraDoc.DocumentObjectModel.Shapes.Charts.Legend GetLegendModel() => _model;

        public void SetLegendModel(MigraDoc.DocumentObjectModel.Shapes.Charts.Legend model) => _model = model;


        private void Legend_ParentSet(object sender, EventArgs e)
        {
            DOMRelations.Relate(GetPresentableParent(), this);
            ApplyStyling();
        }


        public Legend()
        {
            ParentSet += Legend_ParentSet;
        }



        private ParagraphFormat _format;
        public ParagraphFormat Format => _format ?? (_format = new ParagraphFormat(_model.Format));

        private LineFormat _lineFormat;
        public LineFormat LineFormat => _lineFormat ?? (_lineFormat = new LineFormat(_model.LineFormat));
    }
}
