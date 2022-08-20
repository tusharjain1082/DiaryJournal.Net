using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MigraDocXML.DOM
{
    public class Column : DOMElement
    {
        private MigraDoc.DocumentObjectModel.Tables.Column _model;

        public override MigraDoc.DocumentObjectModel.DocumentObject GetModel() => _model;

        public MigraDoc.DocumentObjectModel.Tables.Column GetColumnModel() => _model;

        internal void SetColumnModel(MigraDoc.DocumentObjectModel.Tables.Column model) => _model = model;


        private void Column_ParentSet(object sender, EventArgs e)
        {
            DOMRelations.Relate(GetPresentableParent(), this);
            ApplyStyling();
        }


        public override void SetUnknownAttribute(string name, object value)
        {
            if (!name.StartsWith("Format."))
                throw new InvalidOperationException($"Unrecognised attribute {name} on type {GetType().Name}");

            Table table = GetPresentableParent() as Table;
            int index = table.Children.OfType<Column>().Count() - 1;
            Type cellType = null;
            switch (index)
            {
                case 0: cellType = typeof(C0); break;
                case 1: cellType = typeof(C1); break;
                case 2: cellType = typeof(C2); break;
                case 3: cellType = typeof(C3); break;
                case 4: cellType = typeof(C4); break;
                case 5: cellType = typeof(C5); break;
                case 6: cellType = typeof(C6); break;
                case 7: cellType = typeof(C7); break;
                case 8: cellType = typeof(C8); break;
                case 9: cellType = typeof(C9); break;
                case 10: cellType = typeof(C10); break;
                case 11: cellType = typeof(C11); break;
                case 12: cellType = typeof(C12); break;
                case 13: cellType = typeof(C13); break;
                case 14: cellType = typeof(C14); break;
                case 15: cellType = typeof(C15); break;
                case 16: cellType = typeof(C16); break;
                case 17: cellType = typeof(C17); break;
                case 18: cellType = typeof(C18); break;
                case 19: cellType = typeof(C19); break;
                case 20: cellType = typeof(C20); break;
            }

            if(cellType != null)
            {
                List<Type> parentNodeTypes = new List<Type>();
                var parent = GetPresentableParent();
                while(parent != null)
                {
                    parentNodeTypes.Add(parent.GetType());
                    parent = parent.GetPresentableParent();
                }
                parentNodeTypes.Reverse();
                parentNodeTypes.Add(typeof(Row));
                parentNodeTypes.Add(cellType);

                Style pStyle = new Style();
				pStyle.TargetPaths.Add(new StyleTargetPath());
                foreach (var parentNodeType in parentNodeTypes)
                    pStyle.TargetPaths[0].Add(new StyledType(parentNodeType, null));
                pStyle.TargetPaths[0].Add(new StyleTargetWildcard(-1));
                pStyle.TargetPaths[0].Add(new StyledType(typeof(Paragraph), null));
                pStyle.AddChild(new Setters());
                pStyle.Setters.GetItems()[name] = value?.ToString();
                table.AddChild(pStyle);
            }
        }


        public Column()
        {
            NewVariable("Column", this);
            ParentSet += Column_ParentSet;
        }



        private Borders _borders;
        public Borders Borders => _borders ?? (_borders = new Borders(_model.Borders));

        public bool Heading { get => _model.HeadingFormat; set => _model.HeadingFormat = value; }

        public int KeepWith { get => _model.KeepWith; set => _model.KeepWith = value; }

        public Unit LeftPadding { get => new Unit(_model.LeftPadding); set => _model.LeftPadding = value.GetModel(); }

        public Unit RightPadding { get => new Unit(_model.RightPadding); set => _model.RightPadding = value.GetModel(); }

        private Shading _shading;
        public Shading Shading => _shading ?? (_shading = new Shading(_model.Shading));

        public Unit Width { get => new Unit(_model.Width); set => _model.Width = value.GetModel(); }
    }
}
