using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MigraDocXML.DOM.Charting
{
    public class PlotArea : DOMElement
    {
        private MigraDoc.DocumentObjectModel.Shapes.Charts.PlotArea _model;

        public override MigraDoc.DocumentObjectModel.DocumentObject GetModel() => _model;

        public MigraDoc.DocumentObjectModel.Shapes.Charts.PlotArea GetPlotAreaModel() => _model;

        public void SetPlotAreaModel(MigraDoc.DocumentObjectModel.Shapes.Charts.PlotArea model) => _model = model;


        private void PlotArea_ParentSet(object sender, EventArgs e)
        {
            DOMRelations.Relate(GetPresentableParent(), this);
            ApplyStyling();
        }


        public PlotArea()
        {
            ParentSet += PlotArea_ParentSet;
        }



        public Unit BottomPadding { get => new Unit(_model.BottomPadding); set => _model.BottomPadding = value.GetModel(); }

        private FillFormat _fillFormat;
        public FillFormat FillFormat => _fillFormat ?? (_fillFormat = new FillFormat(_model.FillFormat));

        public Unit LeftPadding { get => new Unit(_model.LeftPadding); set => _model.LeftPadding = value.GetModel(); }

        private LineFormat _lineFormat;
        public LineFormat LineFormat => _lineFormat ?? (_lineFormat = new LineFormat(_model.LineFormat));

        public Unit RightPadding { get => new Unit(_model.RightPadding); set => _model.RightPadding = value.GetModel(); }

        public Unit TopPadding { get => new Unit(_model.TopPadding); set => _model.TopPadding = value.GetModel(); }
    }
}
