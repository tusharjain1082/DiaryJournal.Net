using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MigraDocXML.DOM
{
    public static class DOMTypes
    {
        private static Dictionary<string, Type> _lookupTable = new Dictionary<string, Type>();

        static DOMTypes()
        {
            _lookupTable["Axis"] = typeof(Charting.Axis);
                _lookupTable["XAxis"] = typeof(Charting.XAxis);
                _lookupTable["YAxis"] = typeof(Charting.YAxis);
                _lookupTable["ZAxis"] = typeof(Charting.ZAxis);

            _lookupTable["Bezier"] = typeof(Drawing.Bezier);

            _lookupTable["Bookmark"] = typeof(Bookmark);

            _lookupTable["Break"] = typeof(Break);

            _lookupTable["Cell"] = typeof(Cell);
            var cellTypes = new[]
            {
                typeof(C0), typeof(C1), typeof(C2), typeof(C3), typeof(C4),
                typeof(C5), typeof(C6), typeof(C7), typeof(C8), typeof(C9),
                typeof(C10), typeof(C11), typeof(C12), typeof(C13), typeof(C14),
                typeof(C15), typeof(C16), typeof(C17), typeof(C18), typeof(C19),
                typeof(C20)
            };
            for (int i = 0; i <= 20; i++)
                _lookupTable["C" + i] = cellTypes[i];

            _lookupTable["Chart"] = typeof(Charting.Chart);

            _lookupTable["Column"] = typeof(Column);

            _lookupTable["Continue"] = typeof(Continue);

            _lookupTable["DataLabel"] = typeof(Charting.DataLabel);

            _lookupTable["DateField"] = typeof(DateField);

            _lookupTable["Default"] = typeof(Default);

            _lookupTable["Document"] = typeof(Document);

            _lookupTable["Else"] = typeof(Else);

            _lookupTable["ElseIf"] = typeof(ElseIf);

            _lookupTable["Footer"] = typeof(Footer);

            _lookupTable["ForEach"] = typeof(ForEach);

            _lookupTable["FormattedText"] = typeof(FormattedText);
                _lookupTable["b"] = typeof(Bold);
                _lookupTable["i"] = typeof(Italic);
                _lookupTable["ul"] = typeof(Underline);
                _lookupTable["sub"] = typeof(Subscript);
                _lookupTable["super"] = typeof(Superscript);

            _lookupTable["Graphics"] = typeof(Drawing.Graphics);

            _lookupTable["Header"] = typeof(Header);

            _lookupTable["Hyperlink"] = typeof(Hyperlink);
                _lookupTable["a"] = typeof(Hyperlink);

            _lookupTable["If"] = typeof(If);

            _lookupTable["Image"] = typeof(Image);

            _lookupTable["Insert"] = typeof(Insert);

            _lookupTable["Legend"] = typeof(Charting.Legend);

            _lookupTable["Line"] = typeof(Drawing.Line);

            _lookupTable["NumPagesField"] = typeof(NumPagesField);

            _lookupTable["PageBreak"] = typeof(PageBreak);

            _lookupTable["PageField"] = typeof(PageField);

            _lookupTable["Paragraph"] = typeof(Paragraph);
                _lookupTable["p"] = typeof(Paragraph);

            _lookupTable["PlotArea"] = typeof(Charting.PlotArea);

            _lookupTable["Point"] = typeof(Drawing.Point);

            _lookupTable["PointList"] = typeof(PointList);
                _lookupTable["list"] = typeof(PointList);

            _lookupTable["Quit"] = typeof(Quit);

			_lookupTable["Rect"] = typeof(Drawing.Rect);

            _lookupTable["Row"] = typeof(Row);

            _lookupTable["Section"] = typeof(Section);

            _lookupTable["Series"] = typeof(Charting.Series);

            _lookupTable["Set"] = typeof(Set);

            _lookupTable["Setters"] = typeof(Setters);

            _lookupTable["Style"] = typeof(Style);

			_lookupTable["String"] = typeof(Drawing.String);

            _lookupTable["Table"] = typeof(Table);

            _lookupTable["Text"] = typeof(Text);

            _lookupTable["TextArea"] = typeof(Charting.TextArea);
                _lookupTable["BottomArea"] = typeof(Charting.BottomArea);
                _lookupTable["FooterArea"] = typeof(Charting.FooterArea);
                _lookupTable["HeaderArea"] = typeof(Charting.HeaderArea);
                _lookupTable["LeftArea"] = typeof(Charting.LeftArea);
                _lookupTable["RightArea"] = typeof(Charting.RightArea);
                _lookupTable["TopArea"] = typeof(Charting.TopArea);

            _lookupTable["TextFrame"] = typeof(TextFrame);

            _lookupTable["Var"] = typeof(Var);

            _lookupTable["While"] = typeof(While);

            _lookupTable["XSeries"] = typeof(Charting.XSeries);
        }


        public static Type Lookup(string name)
        {
            if (!_lookupTable.ContainsKey(name))
                throw new Exception("Unrecognised element name " + name);
            return _lookupTable[name];
        }


        public static void Add<T>(string name) where T : DOMElement
        {
            _lookupTable[name] = typeof(T);
        }

        /// <summary>
        /// Add a type lookup, only if one with the same name isn't already defined
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool TryAdd<T>(string name) where T : DOMElement
        {
            if (_lookupTable.ContainsKey(name))
                return false;
            _lookupTable[name] = typeof(T);
            return true;
        }
    }
}
