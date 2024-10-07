#region MigraDoc - Creating Documents on the Fly
//
// Authors:
//   Klaus Potzesny
//
// Copyright (c) 2001-2019 empira Software GmbH, Cologne Area (Germany)
//
// http://www.pdfsharp.com
// http://www.migradoc.com
// http://sourceforge.net/projects/pdfsharp
//
// Permission is hereby granted, free of charge, to any person obtaining a
// copy of this software and associated documentation files (the "Software"),
// to deal in the Software without restriction, including without limitation
// the rights to use, copy, modify, merge, publish, distribute, sublicense,
// and/or sell copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included
// in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE.
#endregion

#define CACHE_FONTS_

using System;
using PdfSharp.Pdf;
using PdfSharp.Drawing;
using MigraDoc.DocumentObjectModel;

namespace MigraDoc.Rendering
{
    /// <summary>
    /// Helps measuring and handling fonts.
    /// </summary>
    internal class FontHandler
    {
#if DEBUG
        internal static int CreateFontCounter;
#endif

        /// <summary>
        /// Converts a DOM Font to an XFont.
        /// </summary>
        internal static XFont FontToXFont(Font font, PdfFontEncoding encoding)
        {
            XPdfFontOptions options = new XPdfFontOptions(encoding);
            XFontStyle style = GetXStyle(font);

#if DEBUG
            if (StringComparer.OrdinalIgnoreCase.Compare(font.Name, "Segoe UI Semilight") == 0
                && (style & XFontStyle.BoldItalic) == XFontStyle.Italic)
                font.GetType();
#endif
            XFont xFont = new XFont(font.Name, font.Size, style, options);
#if DEBUG
            CreateFontCounter++;
#endif
            return xFont;
        }

        internal static XFontStyle GetXStyle(Font font)
        {
            XFontStyle style = XFontStyle.Regular;
            if (font.Bold)
                style = font.Italic ? XFontStyle.BoldItalic : XFontStyle.Bold;
            else if (font.Italic)
                style = XFontStyle.Italic;

            return style;
        }

        internal static XUnit GetDescent(XFont font)
        {
            XUnit descent = font.Metrics.Descent;
            descent *= font.Size;
            descent /= font.FontFamily.GetEmHeight(font.Style);
            return descent;
        }

        internal static XUnit GetAscent(XFont font)
        {
            XUnit ascent = font.Metrics.Ascent;
            ascent *= font.Size;
            ascent /= font.FontFamily.GetEmHeight(font.Style);
            return ascent;
        }

        internal static double GetSubSuperScaling(XFont font)
        {
            return 0.8 * GetAscent(font) / font.GetHeight();
        }

        internal static XFont ToSubSuperFont(XFont font)
        {
            double size = font.Size * GetSubSuperScaling(font);

            return new XFont(font.Name, size, font.Style, font.PdfOptions);
        }

        internal static XBrush FontColorToXBrush(Font font)
        {
#if noCMYK
            return new XSolidBrush(XColor.FromArgb((int)font.Color.A, (int)font.Color.R, (int)font.Color.G, (int)font.Color.B));
#else
            return new XSolidBrush(ColorHelper.ToXColor(font.Color, font.Document.UseCmykColor));
#endif
        }

#if CACHE_FONTS
    static XFont XFontFromCache(Font font, bool unicode, PdfFontEmbedding fontEmbedding)
    {
      XFont xFont = null;

      XPdfFontOptions options = null;
      options = new XPdfFontOptions(fontEmbedding, unicode);
      XFontStyle style = GetXStyle(font);
      xFont = new XFont(font.Name, font.Size, style, options);

      return xFont;
    }

    static string BuildSignature(Font font, bool unicode, PdfFontEmbedding fontEmbedding)
    {
      StringBuilder signature = new StringBuilder(128);
      signature.Append(font.Name.ToLower());
      signature.Append(font.Size.Point.ToString("##0.0"));
      return signature.ToString();
    }

    static Hash_table fontCache = new Hash_table();
#endif
    }
}
