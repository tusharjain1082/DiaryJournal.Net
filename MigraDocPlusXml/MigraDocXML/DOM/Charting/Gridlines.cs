using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MigraDocXML.DOM.Charting
{
    public class Gridlines
    {
        private MigraDoc.DocumentObjectModel.Shapes.Charts.Gridlines _model;
        public MigraDoc.DocumentObjectModel.Shapes.Charts.Gridlines GetModel() => _model;


        public Gridlines(MigraDoc.DocumentObjectModel.Shapes.Charts.Gridlines model)
        {
            _model = model ?? throw new ArgumentNullException(nameof(model));
        }


        private LineFormat _lineFormat;
        public LineFormat LineFormat => _lineFormat ?? (_lineFormat = new LineFormat(_model.LineFormat));
    }
}
