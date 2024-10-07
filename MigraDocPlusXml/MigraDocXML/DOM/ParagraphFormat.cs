using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MigraDocXML.DOM
{
    public class ParagraphFormat
    {
        private MigraDoc.DocumentObjectModel.ParagraphFormat _model;
        public MigraDoc.DocumentObjectModel.ParagraphFormat GetModel() => _model;


        public ParagraphFormat(MigraDoc.DocumentObjectModel.ParagraphFormat model)
        {
            _model = model ?? throw new ArgumentNullException(nameof(model));
        }



        public string Alignment
        {
            get => _model.Alignment.ToString();
            set => _model.Alignment = Parse.Enum<MigraDoc.DocumentObjectModel.ParagraphAlignment>(value);
        }

        private Borders _borders;
        public Borders Borders => _borders ?? (_borders = new Borders(_model.Borders));

        public Unit FirstLineIndent { get => new Unit(_model.FirstLineIndent); set => _model.FirstLineIndent = value.GetModel(); }

        private Font _font;
        public Font Font => _font ?? (_font = new Font(_model.Font));

        public bool KeepTogether { get => _model.KeepTogether; set => _model.KeepTogether = value; }

        public bool KeepWithNext { get => _model.KeepWithNext; set => _model.KeepWithNext = value; }

        public Unit LeftIndent { get => new Unit(_model.LeftIndent); set => _model.LeftIndent = value.GetModel(); }

        public Unit LineSpacing { get => new Unit(_model.LineSpacing); set => _model.LineSpacing = value.GetModel(); }

        public string LineSpacingRule
        {
            get => _model.LineSpacingRule.ToString();
            set => _model.LineSpacingRule = Parse.Enum<MigraDoc.DocumentObjectModel.LineSpacingRule>(value);
        }

        public string OutlineLevel
        {
            get => _model.OutlineLevel.ToString();
            set => _model.OutlineLevel = Parse.Enum<MigraDoc.DocumentObjectModel.OutlineLevel>(value);
        }

        public bool PageBreakBefore { get => _model.PageBreakBefore; set => _model.PageBreakBefore = value; }

        public Unit RightIndent { get => new Unit(_model.RightIndent); set => _model.RightIndent = value.GetModel(); }

        private Shading _shading;
        public Shading Shading => _shading ?? (_shading = new Shading(_model.Shading));

        public Unit SpaceAfter { get => new Unit(_model.SpaceAfter); set => _model.SpaceAfter = value.GetModel(); }

        public Unit SpaceBefore { get => new Unit(_model.SpaceBefore); set => _model.SpaceBefore = value.GetModel(); }

        public bool WidowControl { get => _model.WidowControl; set => _model.WidowControl = value; }



        /// <summary>
        /// Many elements such as tables, rows, columns etc. have attributes to allow for styling of all paragraphs that they contain
        /// In MigraDocXML, these attributes are implemented as styles on the element
        /// Use this static method to implement that logic
        /// </summary>
        /// <param name="caller"></param>
        /// <param name="attribute"></param>
        /// <param name="value"></param>
        /// <returns>Returns whether the attribute was suitable to be added as a paragraph formatting style or not</returns>
        public static bool AddParagraphFormattingAttribute(DOMElement caller, string attribute, object value)
        {
            if (!attribute.StartsWith("Format."))
                return false;

            Type pType = typeof(Paragraph);
            Style pStyle = caller.Children.OfType<Style>().FirstOrDefault(x => x.TargetPaths.Count == 1 && x.TargetPaths[0].Count == 1 && x.TargetPaths[0][0].Equals(pType));
            if(pStyle == null)
            {
                pStyle = new Style();
				pStyle.TargetPaths.Add(new StyleTargetPath());
                pStyle.TargetPaths[0].Add(new StyledType(pType, null));
                caller.AddChild(pStyle);
            }
            if (pStyle.Setters == null)
                pStyle.AddChild(new Setters());
            pStyle.Setters.GetItems()[attribute] = value?.ToString();
            return true;
        }
    }
}
