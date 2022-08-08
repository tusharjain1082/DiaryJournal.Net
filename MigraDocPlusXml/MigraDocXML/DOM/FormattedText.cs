using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MigraDocXML.DOM
{
    public class FormattedText : DOMElement
    {
        private MigraDoc.DocumentObjectModel.FormattedText _model;

        public override MigraDoc.DocumentObjectModel.DocumentObject GetModel() => _model;

        public MigraDoc.DocumentObjectModel.FormattedText GetFormattedTextModel() => _model;


        private void FormattedText_ParentSet(object sender, EventArgs e)
        {
            DOMRelations.Relate(GetPresentableParent(), this);
            ApplyStyling();
        }


        public override void SetTextValue(string value)
        {
            _model.AddText(value);
        }


        public FormattedText()
        {
            _model = new MigraDoc.DocumentObjectModel.FormattedText();
            NewVariable("FormattedText", this);
            ParentSet += FormattedText_ParentSet;
        }



        private Font _font;
        public Font Font => _font ?? (_font = new Font(_model.Font));
    }
    
    
    
    /// <summary>
     /// Bold FormattedText
     /// </summary>
    public class Bold : FormattedText
    {
        public Bold()
        {
            Font.Bold = true;
            NewVariable("b", this);
        }
    }

    /// <summary>
    /// Italic FormattedText
    /// </summary>
    public class Italic : FormattedText
    {
        public Italic()
        {
            Font.Italic = true;
            NewVariable("i", this);
        }
    }

    /// <summary>
    /// Underline FormattedText
    /// </summary>
    public class Underline : FormattedText
    {
        public Underline()
        {
            Font.Underline = "Single";
            NewVariable("ul", this);
        }
    }

    /// <summary>
    /// Subscript FormattedText
    /// </summary>
    public class Subscript : FormattedText
    {
        public Subscript()
        {
            Font.Subscript = true;
            NewVariable("sub", this);
        }
    }

    /// <summary>
    /// Superscript FormattedText
    /// </summary>
    public class Superscript : FormattedText
    {
        public Superscript()
        {
            Font.Superscript = true;
            NewVariable("super", this);
        }
    }
}
