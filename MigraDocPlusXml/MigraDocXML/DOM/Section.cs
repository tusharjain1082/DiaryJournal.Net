using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace MigraDocXML.DOM
{
    public class Section : DOMElement
    {
        private MigraDoc.DocumentObjectModel.Section _model;

        public override MigraDoc.DocumentObjectModel.DocumentObject GetModel() => _model;

        public MigraDoc.DocumentObjectModel.Section GetSectionModel() => _model;
        

        private void Section_ParentSet(object sender, EventArgs e)
        {
            DOMRelations.Relate(GetPresentableParent(), this);
            SetPageSetupToDefault();
            ApplyStyling();
        }


        private void SetPageSetupToDefault()
        {
            MigraDoc.DocumentObjectModel.PageSetup sec = _model.PageSetup;
            MigraDoc.DocumentObjectModel.PageSetup doc = GetDocument().GetDocumentModel().DefaultPageSetup;

            if (sec.BottomMargin.IsEmpty && !doc.BottomMargin.IsEmpty)
                sec.BottomMargin = doc.BottomMargin;

            if (sec.FooterDistance.IsEmpty && !doc.FooterDistance.IsEmpty)
                sec.FooterDistance = doc.FooterDistance;

            if (sec.HeaderDistance.IsEmpty && !doc.HeaderDistance.IsEmpty)
                sec.HeaderDistance = doc.HeaderDistance;

            if (sec.LeftMargin.IsEmpty && !doc.LeftMargin.IsEmpty)
                sec.LeftMargin = doc.LeftMargin;

            if (sec.PageHeight.IsEmpty && !doc.PageHeight.IsEmpty)
                sec.PageHeight = doc.PageHeight;

            if (sec.PageWidth.IsEmpty && !doc.PageWidth.IsEmpty)
                sec.PageWidth = doc.PageWidth;

            if (sec.RightMargin.IsEmpty && !doc.RightMargin.IsEmpty)
                sec.RightMargin = doc.RightMargin;

            if (sec.TopMargin.IsEmpty && !doc.TopMargin.IsEmpty)
                sec.TopMargin = doc.TopMargin;
        }


        public override List<XmlAttribute> ArrangeAttributes(IEnumerable<XmlAttribute> attributes)
        {
            var list = attributes.ToList();

            var contentWidth = list.FirstOrDefault(x => x.Name == "PageSetup.ContentWidth");
            if (contentWidth != null)
            {
                list.Remove(contentWidth);
                list.Add(contentWidth);
            }

            var contentHeight = list.FirstOrDefault(x => x.Name == "PageSetup.ContentHeight");
            if (contentHeight != null)
            {
                list.Remove(contentHeight);
                list.Add(contentHeight);
            }

            return list;
        }


        public Section()
        {
            _model = new MigraDoc.DocumentObjectModel.Section();
            NewVariable("Section", this);
            ParentSet += Section_ParentSet;
        }



        private PageSetup _pageSetup;
        public PageSetup PageSetup => _pageSetup ?? (_pageSetup = new PageSetup(_model.PageSetup));
    }
}
