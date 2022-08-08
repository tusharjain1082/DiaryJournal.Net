using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MigraDocXML.DOM.Charting
{
    public class DataLabel : DOMElement
    {
        private MigraDoc.DocumentObjectModel.Shapes.Charts.DataLabel _model;

        public override MigraDoc.DocumentObjectModel.DocumentObject GetModel() => _model;

        public MigraDoc.DocumentObjectModel.Shapes.Charts.DataLabel GetDataLabelModel() => _model;

        public void SetDataLabelModel(MigraDoc.DocumentObjectModel.Shapes.Charts.DataLabel model) => _model = model;


        private void DataLabel_ParentSet(object sender, EventArgs e)
        {
            DOMRelations.Relate(GetPresentableParent(), this);
            ApplyStyling();
        }


        public DataLabel()
        {
            ParentSet += DataLabel_ParentSet;
        }



        private Font _font;
        public Font Font => _font ?? (_font = new Font(_model.Font));

        public string Format { get => _model.Format; set => _model.Format = value; }

        public string Position
        {
            get => _model.Position.ToString();
            set => _model.Position = Parse.Enum<MigraDoc.DocumentObjectModel.Shapes.Charts.DataLabelPosition>(value);
        }

        public string Type
        {
            get => _model.Type.ToString();
            set => _model.Type = Parse.Enum<MigraDoc.DocumentObjectModel.Shapes.Charts.DataLabelType>(value);
        }
    }
}
