using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MigraDocXML.DOM.Charting
{
    public abstract class Axis : DOMElement
    {
        private MigraDoc.DocumentObjectModel.Shapes.Charts.Axis _model;

        public override MigraDoc.DocumentObjectModel.DocumentObject GetModel() => _model;

        public MigraDoc.DocumentObjectModel.Shapes.Charts.Axis GetAxisModel() => _model;

        public void SetAxisModel(MigraDoc.DocumentObjectModel.Shapes.Charts.Axis model) => _model = model;


        private void Axis_ParentSet(object sender, EventArgs e)
        {
            DOMRelations.Relate(GetPresentableParent(), this);
            ApplyStyling();
        }


        public Axis()
        {
            ParentSet += Axis_ParentSet;
        }



        public bool HasMajorGridlines { get => _model.HasMajorGridlines; set => _model.HasMajorGridlines = value; }

        public bool HasMinorGridlines { get => _model.HasMinorGridlines; set => _model.HasMinorGridlines = value; }

        private LineFormat _lineFormat;
        public LineFormat LineFormat => _lineFormat ?? (_lineFormat = new LineFormat(_model.LineFormat));

        private Gridlines _majorGridlines;
        public Gridlines MajorGridlines => _majorGridlines ?? (_majorGridlines = new Gridlines(_model.MajorGridlines));

        public double MajorTick { get => _model.MajorTick; set => _model.MajorTick = value; }

        public string MajorTickMark
        {
            get => _model.MajorTickMark.ToString();
            set => _model.MajorTickMark = Parse.Enum<MigraDoc.DocumentObjectModel.Shapes.Charts.TickMarkType>(value);
        }

        public double MaximumScale { get => _model.MaximumScale; set => _model.MaximumScale = value; }

        public double MinimumScale { get => _model.MinimumScale; set => _model.MinimumScale = value; }

        private Gridlines _minorGridlines;
        public Gridlines MinorGridlines => _minorGridlines ?? (_minorGridlines = new Gridlines(_model.MinorGridlines));

        public double MinorTick { get => _model.MinorTick; set => _model.MinorTick = value; }

        public string MinorTickMark
        {
            get => _model.MinorTickMark.ToString();
            set => _model.MinorTickMark = Parse.Enum<MigraDoc.DocumentObjectModel.Shapes.Charts.TickMarkType>(value);
        }

        private TickLabels _tickLabels;
        public TickLabels TickLabels => _tickLabels ?? (_tickLabels = new TickLabels(_model.TickLabels));

        private AxisTitle _title;
        public AxisTitle Title => _title ?? (_title = new AxisTitle(_model.Title));
    }



    public class XAxis : Axis { }

    public class YAxis : Axis { }

    public class ZAxis : Axis { }
}
