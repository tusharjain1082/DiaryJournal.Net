using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MigraDocXML.DOM.Charting
{
    public class TickLabels
    {
        private MigraDoc.DocumentObjectModel.Shapes.Charts.TickLabels _model;
        public MigraDoc.DocumentObjectModel.Shapes.Charts.TickLabels GetModel() => _model;


        public TickLabels(MigraDoc.DocumentObjectModel.Shapes.Charts.TickLabels model)
        {
            _model = model ?? throw new ArgumentNullException(nameof(model));
        }


        private Font _font;
        public Font Font => _font ?? (_font = new Font(_model.Font));

        public string Format { get => _model.Format; set => _model.Format = value; }
    }
}
