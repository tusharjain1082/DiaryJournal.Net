using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MigraDocXML.DOM
{
    public class Bookmark : DOMElement
    {
        private MigraDoc.DocumentObjectModel.Fields.BookmarkField _model;

        public override MigraDoc.DocumentObjectModel.DocumentObject GetModel() => _model;

        public MigraDoc.DocumentObjectModel.Fields.BookmarkField GetBookmarkModel() => _model;
        

        private void Bookmark_ParentSet(object sender, EventArgs e)
        {
            DOMRelations.Relate(GetPresentableParent(), this);
            ApplyStyling();
        }


        public Bookmark()
        {
            _model = new MigraDoc.DocumentObjectModel.Fields.BookmarkField("");
            NewVariable("Bookmark", this);
            ParentSet += Bookmark_ParentSet;
        }

        public override void SetTextValue(string value)
        {
            Name = value;
        }



        public string Name { get => _model.Name; set => _model.Name = value; }
    }
}
