using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MigraDocXML.DOM.Charting
{
    public class Chart : Shape
    {
        private MigraDoc.DocumentObjectModel.Shapes.Charts.Chart _model;

        public override MigraDoc.DocumentObjectModel.DocumentObject GetModel() => _model;

        public MigraDoc.DocumentObjectModel.Shapes.Charts.Chart GetChartModel() => _model;


        private void Chart_ParentSet(object sender, EventArgs e)
        {
            DOMRelations.Relate(GetPresentableParent(), this);
            ApplyStyling();
        }


        public Chart()
        {
            _model = new MigraDoc.DocumentObjectModel.Shapes.Charts.Chart();
            ShapeModel = _model;
            NewVariable("Chart", this);
            Width = "5cm";
            Height = "5cm";
            ParentSet += Chart_ParentSet;
        }



        public string DisplayBlanksAs
        {
            get => _model.DisplayBlanksAs.ToString();
            set => _model.DisplayBlanksAs = Parse.Enum<MigraDoc.DocumentObjectModel.Shapes.Charts.BlankType>(value);
        }

        public string Type
        {
            get => _model.Type.ToString();
            set => _model.Type = Parse.Enum<MigraDoc.DocumentObjectModel.Shapes.Charts.ChartType>(value);
        }

        private ParagraphFormat _format;
        public ParagraphFormat Format => _format ?? (_format = new ParagraphFormat(_model.Format));

        public bool PivotChart { get => _model.PivotChart; set => _model.PivotChart = value; }
    }
}
