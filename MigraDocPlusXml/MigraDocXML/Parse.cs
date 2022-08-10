using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MigraDocXML
{
    public static class Parse
    {
        public static T Enum<T>(string value) where T : struct, IConvertible
        {
            var tType = typeof(T);
            if (!tType.IsEnum)
                throw new ArgumentException("Generic parameter must be of Enum type");

            return (T)System.Enum.Parse(tType, value);
        }



        private static List<FieldInfo> _predefinedColors;
        private static List<FieldInfo> PredefinedColors
        {
            get
            {
                if (_predefinedColors == null)
                    _predefinedColors = typeof(MigraDoc.DocumentObjectModel.Colors).GetFields().ToList();
                return _predefinedColors;
            }
        }

        public static MigraDoc.DocumentObjectModel.Color Color(string code)
        {
            if (code.StartsWith("#"))
            {
                code = code.Substring(1);
                List<string> partStrings = Enumerable.Range(0, code.Length / 2).Select(i => code.Substring(i * 2, 2)).ToList();
                List<byte> parts = partStrings.Select(x => byte.Parse(x, System.Globalization.NumberStyles.HexNumber)).ToList();

                if (parts.Count == 3)
                    return new MigraDoc.DocumentObjectModel.Color(parts[0], parts[1], parts[2]);
                else if (parts.Count == 4)
                    return new MigraDoc.DocumentObjectModel.Color(parts[0], parts[1], parts[2], parts[3]);
                else
                    throw new Exception($"Invalid color code {code}");
            }
            else
            {
                return (MigraDoc.DocumentObjectModel.Color)PredefinedColors.First(x => x.Name == code).GetValue(null);
            }
        }



        private static List<PropertyInfo> _predefinedXColors;
        private static List<PropertyInfo> PredefinedXColors
        {
            get
            {
                if (_predefinedXColors == null)
                    _predefinedXColors = typeof(PdfSharp.Drawing.XColors).GetProperties().ToList();
                return _predefinedXColors;
            }
        }

        public static PdfSharp.Drawing.XColor XColor(string code)
        {
            if (code.StartsWith("#"))
            {
                code = code.Substring(1);
                List<string> partStrings = Enumerable.Range(0, code.Length / 2).Select(i => code.Substring(i * 2, 2)).ToList();
                List<byte> parts = partStrings.Select(x => byte.Parse(x, System.Globalization.NumberStyles.HexNumber)).ToList();

                if (parts.Count == 3)
                    return PdfSharp.Drawing.XColor.FromArgb(parts[0], parts[1], parts[2]);
                else if (parts.Count == 4)
                    return PdfSharp.Drawing.XColor.FromArgb(parts[0], parts[1], parts[2], parts[3]);
                else
                    throw new Exception($"Invalid color code {code}");
            }
            else
            {
                return (PdfSharp.Drawing.XColor)PredefinedXColors.First(x => x.Name == code).GetValue(null, null);
            }
        }



        public static MigraDoc.DocumentObjectModel.Shapes.ShapePosition ShapePosition(string name)
        {
            switch (name)
            {
                case "Bottom": return MigraDoc.DocumentObjectModel.Shapes.ShapePosition.Bottom;
                case "Center": return MigraDoc.DocumentObjectModel.Shapes.ShapePosition.Center;
                case "Inside": return MigraDoc.DocumentObjectModel.Shapes.ShapePosition.Inside;
                case "Left": return MigraDoc.DocumentObjectModel.Shapes.ShapePosition.Left;
                case "Outside": return MigraDoc.DocumentObjectModel.Shapes.ShapePosition.Outside;
                case "Right": return MigraDoc.DocumentObjectModel.Shapes.ShapePosition.Right;
                case "Top": return MigraDoc.DocumentObjectModel.Shapes.ShapePosition.Top;
                case "Undefined": return MigraDoc.DocumentObjectModel.Shapes.ShapePosition.Undefined;
            }
            return MigraDoc.DocumentObjectModel.Shapes.ShapePosition.Undefined;
        }
    }
}
