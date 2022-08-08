using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MigraDocXML.DOM.Charting
{
    public class XSeries : DOMElement
    {
        private MigraDoc.DocumentObjectModel.Shapes.Charts.XSeries _model;

        public override MigraDoc.DocumentObjectModel.DocumentObject GetModel() => _model;

        public MigraDoc.DocumentObjectModel.Shapes.Charts.XSeries GetXSeriesModel() => _model;

        public void SetXSeriesModel(MigraDoc.DocumentObjectModel.Shapes.Charts.XSeries model) => _model = model;


        private void XSeries_ParentSet(object sender, EventArgs e)
        {
            DOMRelations.Relate(GetPresentableParent(), this);
            ApplyStyling();
        }


        public XSeries()
        {
            _model = new MigraDoc.DocumentObjectModel.Shapes.Charts.XSeries();
            ParentSet += XSeries_ParentSet;
        }



        private string _values;
        public string Values
        {
            get => _values;
            set
            {
                _values = value;

                var values = GetDocument().ScriptRunner.Run(value, s => GetParent().GetVariable(s));
                var strings = (values as IEnumerable).OfType<object>().Select(x => x?.ToString()).ToArray();
                if (strings == null)
                    throw new Exception("Chart XSeries Value must be an enumerable expression of string values");
                _model.Add(strings);
            }
        }
    }
}
