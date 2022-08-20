using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MigraDocXML.DOM
{
    public class Unit
    {
        private MigraDoc.DocumentObjectModel.Unit _model;
        public MigraDoc.DocumentObjectModel.Unit GetModel() => _model;
        

        public Unit(MigraDoc.DocumentObjectModel.Unit model)
        {
            _model = model;
        }

        public Unit(double value, MigraDoc.DocumentObjectModel.UnitType type)
        {
            _model = new MigraDoc.DocumentObjectModel.Unit(value, type);
        }



        public double Value => _model.Value;

        public MigraDoc.DocumentObjectModel.UnitType Type => _model.Type;

        public double mm => Mm;
        public double Mm
        {
            get
            {
                switch (Type)
                {
                    case MigraDoc.DocumentObjectModel.UnitType.Centimeter:
                        return Value * 10;
                    case MigraDoc.DocumentObjectModel.UnitType.Inch:
                        return Value * 25.4;
                    case MigraDoc.DocumentObjectModel.UnitType.Millimeter:
                        return Value;
                    case MigraDoc.DocumentObjectModel.UnitType.Pica:
                        return Value * 25.4 / 6;
                    case MigraDoc.DocumentObjectModel.UnitType.Point:
                        return Value * 25.4 / 72;
                    default:
                        throw new Exception();
                }
            }
        }

        public double cm => Cm;
        public double Cm => (Type == MigraDoc.DocumentObjectModel.UnitType.Centimeter) ? Value : Mm / 10;

        public double Inches => (Type == MigraDoc.DocumentObjectModel.UnitType.Inch) ? Value : Mm / 25.4;

        public double Picas => (Type == MigraDoc.DocumentObjectModel.UnitType.Pica) ? Value : Mm * 6 / 25.4;

        public double Points => (Type == MigraDoc.DocumentObjectModel.UnitType.Point) ? Value : Mm * 72 / 25.4;


        public override string ToString() => _model.ToString();



        public override bool Equals(object obj)
        {
            return Equals(obj as Unit);
        }

        public override int GetHashCode()
        {
            return _model.GetHashCode();
        }

        public bool Equals(Unit unit)
        {
            if (unit == null)
                return false;
            return Value == unit.Value && Type == unit.Type;
        }


        public static implicit operator Unit(string str)
        {
            return new Unit(str);
        }


        public static Unit operator *(Unit a, double b)
        {
            return new Unit(a.Value * b, a.Type);
        }

        public static Unit operator /(Unit a, double b)
        {
            return new Unit(a.Value / b, a.Type);
        }

        public static Unit operator +(Unit a, Unit b)
        {
            if (a.Type != b.Type)
                return new Unit(a._model.Millimeter + b._model.Millimeter, MigraDoc.DocumentObjectModel.UnitType.Millimeter);
            return new Unit(a.Value + b.Value, a.Type);
        }

        public static Unit operator -(Unit a, Unit b)
        {
            if (a.Type != b.Type)
                return new Unit(a._model.Millimeter - b._model.Millimeter, MigraDoc.DocumentObjectModel.UnitType.Millimeter);
            return new Unit(a.Value - b.Value, a.Type);
        }
    }
}
