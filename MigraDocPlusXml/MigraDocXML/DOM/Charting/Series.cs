using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MigraDocXML.DOM.Charting
{
    public class Series : DOMElement
    {
        private MigraDoc.DocumentObjectModel.Shapes.Charts.Series _model;

        public override MigraDoc.DocumentObjectModel.DocumentObject GetModel() => _model;

        public MigraDoc.DocumentObjectModel.Shapes.Charts.Series GetSeriesModel() => _model;


        private void Series_ParentSet(object sender, EventArgs e)
        {
            DOMRelations.Relate(GetPresentableParent(), this);
            ApplyStyling();
        }


        public Series()
        {
            _model = new MigraDoc.DocumentObjectModel.Shapes.Charts.Series();
            ParentSet += Series_ParentSet;
        }



        public string Name { get => _model.Name; set => _model.Name = value; }

        public string ChartType
        {
            get => _model.ChartType.ToString();
            set => _model.ChartType = Parse.Enum<MigraDoc.DocumentObjectModel.Shapes.Charts.ChartType>(value);
        }

        private FillFormat _fillFormat;
        public FillFormat FillFormat => _fillFormat ?? (_fillFormat = new FillFormat(_model.FillFormat));

        public bool HasDataLabel { get => _model.HasDataLabel; set => _model.HasDataLabel = value; }

        private LineFormat _lineFormat;
        public LineFormat LineFormat => _lineFormat ?? (_lineFormat = new LineFormat(_model.LineFormat));

        public string MarkerBackgroundColor
        {
            get => _model.MarkerBackgroundColor.ToString();
            set => _model.MarkerBackgroundColor = Parse.Color(value);
        }

        public string MarkerForegroundColor
        {
            get => _model.MarkerForegroundColor.ToString();
            set => _model.MarkerForegroundColor = Parse.Color(value);
        }

        public Unit MarkerSize
        {
            get => new Unit(_model.MarkerSize);
            set => _model.MarkerSize = value.GetModel();
        }

        public string MarkerStyle
        {
            get => _model.MarkerStyle.ToString();
            set => _model.MarkerStyle = Parse.Enum<MigraDoc.DocumentObjectModel.Shapes.Charts.MarkerStyle>(value);
        }

        private DataLabel _dataLabel;
        public DataLabel DataLabel
        {
            get
            {
                if(_dataLabel == null)
                {
                    _dataLabel = new DataLabel();
                    _dataLabel.SetDataLabelModel(_model.DataLabel);
                }
                return _dataLabel;
            }
        }

        private string _values;
        public string Values
        {
            get => _values;
            set
            {
                _values = value;
                _model.Elements.Clear();
                
                var values = GetDocument().ScriptRunner.Run(value, s => GetParent().GetVariable(s));
                var doubles = (values as IEnumerable).OfType<object>().Select(x => Convert.ToDouble(x));
                if (doubles == null)
                    throw new Exception("Chart Series Value must be an enumerable expression of double values");
                _model.Add(doubles.ToArray());
            }
        }
    }
}
