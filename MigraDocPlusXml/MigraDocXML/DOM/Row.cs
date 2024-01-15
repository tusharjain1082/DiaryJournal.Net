using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace MigraDocXML.DOM
{
    public class Row : DOMElement
    {
        private MigraDoc.DocumentObjectModel.Tables.Row _model;

        public override MigraDoc.DocumentObjectModel.DocumentObject GetModel() => _model;

        public MigraDoc.DocumentObjectModel.Tables.Row GetRowModel() => _model;

        internal void SetRowModel(MigraDoc.DocumentObjectModel.Tables.Row model) => _model = model;


        private void Row_ParentSet(object sender, EventArgs e)
        {
            DOMRelations.Relate(GetPresentableParent(), this);
            ApplyStyling();
        }

        public override void SetUnknownAttribute(string name, object value)
        {
            if (ParagraphFormat.AddParagraphFormattingAttribute(this, name, value))
                return;

            if (!name.StartsWith("C"))
                throw new InvalidOperationException($"Unrecognised attribute {name} on type {GetType().Name}");

			int index = int.Parse(name.Substring(1));

			Cell childCell = null;
			switch (index)
			{
				case 0: childCell = new C0(); break;
				case 1: childCell = new C1(); break;
				case 2: childCell = new C2(); break;
				case 3: childCell = new C3(); break;
				case 4: childCell = new C4(); break;
				case 5: childCell = new C5(); break;
				case 6: childCell = new C6(); break;
				case 7: childCell = new C7(); break;
				case 8: childCell = new C8(); break;
				case 9: childCell = new C9(); break;
				case 10: childCell = new C10(); break;
				case 11: childCell = new C11(); break;
				case 12: childCell = new C12(); break;
				case 13: childCell = new C13(); break;
				case 14: childCell = new C14(); break;
				case 15: childCell = new C15(); break;
				case 16: childCell = new C16(); break;
				case 17: childCell = new C17(); break;
				case 18: childCell = new C18(); break;
				case 19: childCell = new C19(); break;
				case 20: childCell = new C20(); break;
				default: childCell = new Cell() { Index = index }; break;
			}
			AddChild(childCell);
			childCell.SetTextValue(value?.ToString());
		}

        public override List<XmlAttribute> ArrangeAttributes(IEnumerable<XmlAttribute> attributes)
        {
            var output = attributes.Where(x => x.Name.StartsWith("Format.")).ToList();
            output.AddRange(attributes.Where(x => !x.Name.StartsWith("Format.")));
            return output;
        }


        public Row()
        {
            NewVariable("Row", this);
            ParentSet += Row_ParentSet;
        }



        private Borders _borders;
        public Borders Borders => _borders ?? (_borders = new Borders(_model.Borders));

        public Unit BottomPadding { get => new Unit(_model.BottomPadding); set => _model.BottomPadding = value.GetModel(); }

        public bool Heading { get => _model.HeadingFormat; set => _model.HeadingFormat = value; }

        public Unit Height { get => new Unit(_model.Height); set => _model.Height = value.GetModel(); }

        public string HeightRule
        {
            get => _model.HeightRule.ToString();
            set => _model.HeightRule = Parse.Enum<MigraDoc.DocumentObjectModel.Tables.RowHeightRule>(value);
        }

        public int KeepWith { get => _model.KeepWith; set => _model.KeepWith = value; }

        private Shading _shading;
        public Shading Shading => _shading ?? (_shading = new Shading(_model.Shading));

        public Unit TopPadding { get => new Unit(_model.TopPadding); set => _model.TopPadding = value.GetModel(); }

        public string VerticalAlignment
        {
            get => _model.VerticalAlignment.ToString();
            set => _model.VerticalAlignment = Parse.Enum<MigraDoc.DocumentObjectModel.Tables.VerticalAlignment>(value);
        }

        public int Index => _model.Index;
    }
}
