using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MigraDocXML.DOM.Drawing
{
	public class XFont
	{
		public XFont()
		{
		}

		
		public PdfSharp.Drawing.XFont GetModel()
		{
			return new PdfSharp.Drawing.XFont(FamilyName, EmSize, _style, _options);
		}


		public string FamilyName { get; set; }

		public double EmSize { get; set; } = 10.0;

		private PdfSharp.Drawing.XFontStyle _style = PdfSharp.Drawing.XFontStyle.Regular;
		public string Style
		{
			get => _style.ToString();
			set => _style = Parse.Enum<PdfSharp.Drawing.XFontStyle>(value);
		}

		private PdfSharp.Drawing.XPdfFontOptions _options = PdfSharp.Drawing.XPdfFontOptions.UnicodeDefault;
		public string Options
		{
			get => _options.ToString();
			set
			{
				switch (value)
				{
					case "UnicodeDefault": _options = PdfSharp.Drawing.XPdfFontOptions.UnicodeDefault; break;
					case "WinAnsiDefault": _options = PdfSharp.Drawing.XPdfFontOptions.WinAnsiDefault; break;
					default: throw new ArgumentException("Unrecognised Font Options");
				}
			}
		}
	}
}
