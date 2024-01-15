using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MigraDocXML.DOM.Drawing
{
	public class Brush
	{
		public Brush()
		{
		}


		public PdfSharp.Drawing.XBrush GetModel()
		{
			return new PdfSharp.Drawing.XSolidBrush(_color ?? PdfSharp.Drawing.XColors.Transparent);
		}
		
		public PdfSharp.Drawing.XBrush GetModel(PdfSharp.Drawing.XRect rect)
		{
			bool useGradient = _gradientMode != null || (_color != null && _color2 != null);
			if (!useGradient)
				return GetModel();

			var color1 = _color ?? PdfSharp.Drawing.XColors.Transparent;
			var color2 = _color2 ?? PdfSharp.Drawing.XColors.Transparent;
			var gradientMode = _gradientMode ?? PdfSharp.Drawing.XLinearGradientMode.Horizontal;
			return new PdfSharp.Drawing.XLinearGradientBrush(rect, color1, color2, gradientMode);
		}


		private PdfSharp.Drawing.XColor? _color;
		public string Color
		{
			get => _color.ToString();
			set => _color = Parse.XColor(value);
		}

		private PdfSharp.Drawing.XColor? _color2;
		public string Color2
		{
			get => _color2.ToString();
			set => _color2 = Parse.XColor(value);
		}

		private PdfSharp.Drawing.XLinearGradientMode? _gradientMode;
		public string GradientMode
		{
			get => _gradientMode.ToString();
			set => _gradientMode = Parse.Enum<PdfSharp.Drawing.XLinearGradientMode>(value);
		}
	}
}
