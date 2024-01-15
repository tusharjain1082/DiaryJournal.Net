using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MigraDocXML.DOM
{
    public class PointList : DOMElement
    {
        public override MigraDoc.DocumentObjectModel.DocumentObject GetModel() => GetParent()?.GetModel();

        private int _childCount = 0;


        private void PointList_ChildAdded(object sender, DOMElementChildEventArgs args)
        {
            Paragraph para = args.Child as Paragraph;
            if(para != null)
            {
                MigraDoc.DocumentObjectModel.ListInfo listInfo = new MigraDoc.DocumentObjectModel.ListInfo();
                listInfo.ListType = _type;
                listInfo.ContinuePreviousList = _childCount > 0;
                if (NumberPosition != null)
                    listInfo.NumberPosition = NumberPosition.GetModel();
                _childCount++;
                para.GetParagraphModel().Format.ListInfo = listInfo;
            }
			else if (args.Child.IsLogical)
			{
				args.Child.ChildAdded += PointList_ChildAdded;
			}
        }


        public override void SetUnknownAttribute(string name, object value)
        {
            if (ParagraphFormat.AddParagraphFormattingAttribute(this, name, value))
                return;

            base.SetUnknownAttribute(name, value);
        }


        public PointList()
        {
            IsPresentable = false;
            IsLogical = false;
            NewVariable("PointList", this);
            NewVariable("list", this);
            ChildAdded += PointList_ChildAdded;
        }



        private MigraDoc.DocumentObjectModel.ListType _type;
        public string Type
        {
            get => _type.ToString();
            set => _type = Parse.Enum<MigraDoc.DocumentObjectModel.ListType>(value);
        }

        public Unit NumberPosition { get; set; }
    }
}
