using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace MigraDocXML.DOM
{
    public class Cell : DOMElement
    {
        private MigraDoc.DocumentObjectModel.Tables.Cell _model;

        public override MigraDoc.DocumentObjectModel.DocumentObject GetModel() => _model;

        public MigraDoc.DocumentObjectModel.Tables.Cell GetCellModel() => _model;

        internal void SetCellModel(MigraDoc.DocumentObjectModel.Tables.Cell model) => _model = model;


        private void Cell_ParentSet(object sender, EventArgs e)
        {
            RelateToParent();
            ApplyStyling();
        }

        private void RelateToParent()
        {
            if (GetParent() != null && Index >= 0)
                DOMRelations.Relate(GetPresentableParent(), this);
        }
        
        public override void SetTextValue(string value)
        {
            Paragraph p = new Paragraph();
            AddChild(p);
            p.SetTextValue(value);
        }

        public override void SetUnknownAttribute(string name, object value)
        {
            if (!ParagraphFormat.AddParagraphFormattingAttribute(this, name, value))
                throw new InvalidOperationException($"Unrecognised attribute {name} on type {GetType().Name}");
        }


        public override List<XmlAttribute> ArrangeAttributes(IEnumerable<XmlAttribute> attributes)
        {
            var list = attributes.ToList();

            var index = list.FirstOrDefault(x => x.Name == "Index");
            if(index != null)
            {
                list.Remove(index);
                list.Insert(0, index);
            }

            return list;
        }


        public Cell()
        {
            NewVariable("Cell", this);
            ParentSet += Cell_ParentSet;
        }



        private int _index = -1;
        public int Index
        {
            get => _index;
            set
            {
                _index = value;
                NewVariable("C" + _index, this);
                RelateToParent();
            }
        }

        private Borders _borders;
        public Borders Borders => _borders ?? (_borders = new Borders(_model.Borders));

        public int MergeDown { get => _model.MergeDown; set => _model.MergeDown = value; }

        public int MergeRight { get => _model.MergeRight; set => _model.MergeRight = value; }

        private Shading _shading;
        public Shading Shading => _shading ?? (_shading = new Shading(_model.Shading));
        
        public string VerticalAlignment
        {
            get => _model.VerticalAlignment.ToString();
            set => _model.VerticalAlignment = Parse.Enum<MigraDoc.DocumentObjectModel.Tables.VerticalAlignment>(value);
        }
    }



    public class C0 : Cell { public C0() { Index = 0; } }
    public class C1 : Cell { public C1() { Index = 1; } }
    public class C2 : Cell { public C2() { Index = 2; } }
    public class C3 : Cell { public C3() { Index = 3; } }
    public class C4 : Cell { public C4() { Index = 4; } }
    public class C5 : Cell { public C5() { Index = 5; } }
    public class C6 : Cell { public C6() { Index = 6; } }
    public class C7 : Cell { public C7() { Index = 7; } }
    public class C8 : Cell { public C8() { Index = 8; } }
    public class C9 : Cell { public C9() { Index = 9; } }
    public class C10 : Cell { public C10() { Index = 10; } }
    public class C11 : Cell { public C11() { Index = 11; } }
    public class C12 : Cell { public C12() { Index = 12; } }
    public class C13 : Cell { public C13() { Index = 13; } }
    public class C14 : Cell { public C14() { Index = 14; } }
    public class C15 : Cell { public C15() { Index = 15; } }
    public class C16 : Cell { public C16() { Index = 16; } }
    public class C17 : Cell { public C17() { Index = 17; } }
    public class C18 : Cell { public C18() { Index = 18; } }
    public class C19 : Cell { public C19() { Index = 19; } }
    public class C20 : Cell { public C20() { Index = 20; } }
}
