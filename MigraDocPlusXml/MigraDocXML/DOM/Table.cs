using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MigraDocXML.DOM
{
    public class Table : DOMElement
    {
        private MigraDoc.DocumentObjectModel.Tables.Table _model;

        public override MigraDoc.DocumentObjectModel.DocumentObject GetModel() => _model;

        public MigraDoc.DocumentObjectModel.Tables.Table GetTableModel() => _model;


        private void Table_ParentSet(object sender, EventArgs e)
        {
            DOMRelations.Relate(GetPresentableParent(), this);
            ApplyStyling();
        }


        public Table()
        {
            _model = new MigraDoc.DocumentObjectModel.Tables.Table();
            NewVariable("Table", this);
            ParentSet += Table_ParentSet;
        }


        public override void SetUnknownAttribute(string name, object value)
        {
            if (!ParagraphFormat.AddParagraphFormattingAttribute(this, name, value))
                throw new InvalidOperationException($"Unrecognised attribute {name} on type {GetType().Name}");
        }



        public Unit BottomPadding { get => new Unit(_model.BottomPadding); set => _model.BottomPadding = value.GetModel(); }

        public Unit LeftPadding { get => new Unit(_model.LeftPadding); set => _model.LeftPadding = value.GetModel(); }

        public Unit RightPadding { get => new Unit(_model.RightPadding); set => _model.RightPadding = value.GetModel(); }

        public Unit TopPadding { get => new Unit(_model.TopPadding); set => _model.TopPadding = value.GetModel(); }

        private Borders _borders;
        public Borders Borders => _borders ?? (_borders = new Borders(_model.Borders));

        private Shading _shading;
        public Shading Shading => _shading ?? (_shading = new Shading(_model.Shading));

        public bool KeepTogether { get => _model.KeepTogether; set => _model.KeepTogether = value; }
    }
}
