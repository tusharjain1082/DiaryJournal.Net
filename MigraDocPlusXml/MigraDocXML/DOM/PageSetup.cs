using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MigraDocXML.DOM
{
    public class PageSetup
    {
        private MigraDoc.DocumentObjectModel.PageSetup _model;
        public MigraDoc.DocumentObjectModel.PageSetup GetModel() => _model;

        
        public PageSetup(MigraDoc.DocumentObjectModel.PageSetup model)
        {
            _model = model ?? throw new ArgumentNullException(nameof(model));
        }



        public bool DifferentFirstPageHeaderFooter { get => _model.DifferentFirstPageHeaderFooter; set => _model.DifferentFirstPageHeaderFooter = value; }

        public bool OddAndEvenPagesHeaderFooter { get => _model.OddAndEvenPagesHeaderFooter; set => _model.OddAndEvenPagesHeaderFooter = value; }

        public bool HorizontalPageBreak { get => _model.HorizontalPageBreak; set => _model.HorizontalPageBreak = value; }

        public Unit BottomMargin { get => new Unit(_model.BottomMargin); set => _model.BottomMargin = value.GetModel(); }

        public Unit LeftMargin { get => new Unit(_model.LeftMargin); set => _model.LeftMargin = value.GetModel(); }

        public Unit RightMargin { get => new Unit(_model.RightMargin); set => _model.RightMargin = value.GetModel(); }

        public Unit TopMargin { get => new Unit(_model.TopMargin); set => _model.TopMargin = value.GetModel(); }

        public Unit HorizontalMargin
        {
            get
            {
                if (LeftMargin == null || RightMargin == null)
                    return null;
                if (LeftMargin.Equals(RightMargin))
                    return LeftMargin;
                return null;
            }
            set
            {
                LeftMargin = value;
                RightMargin = value;
            }
        }

        public Unit VerticalMargin
        {
            get
            {
                if (TopMargin == null || BottomMargin == null)
                    return null;
                if (TopMargin.Equals(BottomMargin))
                    return TopMargin;
                return null;
            }
            set
            {
                TopMargin = value;
                BottomMargin = value;
            }
        }

        public Unit Margin
        {
            get
            {
                if (HorizontalMargin == null || VerticalMargin == null)
                    return null;
                if (HorizontalMargin.Equals(VerticalMargin))
                    return HorizontalMargin;
                return null;
            }
            set
            {
                HorizontalMargin = value;
                VerticalMargin = value;
            }
        }

        public bool MirrorMargins { get => _model.MirrorMargins; set => _model.MirrorMargins = value; }

        public Unit FooterDistance { get => new Unit(_model.FooterDistance); set => _model.FooterDistance = value.GetModel(); }

        public Unit HeaderDistance { get => new Unit(_model.HeaderDistance); set => _model.HeaderDistance = value.GetModel(); }

        public string Orientation
        {
            get => _model.Orientation.ToString();
            set => _model.Orientation = Parse.Enum<MigraDoc.DocumentObjectModel.Orientation>(value);
        }

        public string PageFormat
        {
            get => _model.PageFormat.ToString();
            set
            {
                _model.PageWidth = MigraDoc.DocumentObjectModel.Unit.Empty;
                _model.PageHeight = MigraDoc.DocumentObjectModel.Unit.Empty;
                _model.PageFormat = Parse.Enum<MigraDoc.DocumentObjectModel.PageFormat>(value);
            }
        }

        public Unit PageWidth
		{
			get
			{
				//This is to handle the fact that we have to empty out the PageWidth property when setting PageFormat
				if(_model.PageWidth.IsEmpty)
				{
					int a0Height = 1189;
					int a0Width = 841;
					int width = 0;
					switch(_model.PageFormat)
					{
						case MigraDoc.DocumentObjectModel.PageFormat.A0: width = a0Width; break;
						case MigraDoc.DocumentObjectModel.PageFormat.A1: width = a0Height / 2; break;
						case MigraDoc.DocumentObjectModel.PageFormat.A2: width = a0Width / 2; break;
						case MigraDoc.DocumentObjectModel.PageFormat.A3: width = a0Height / 4; break;
						case MigraDoc.DocumentObjectModel.PageFormat.A4: width = a0Width / 4; break;
						case MigraDoc.DocumentObjectModel.PageFormat.A5: width = a0Height / 8; break;
						case MigraDoc.DocumentObjectModel.PageFormat.A6: width = a0Width / 8; break;
						case MigraDoc.DocumentObjectModel.PageFormat.B5: width = 182; break;
						case MigraDoc.DocumentObjectModel.PageFormat.Ledger: return new Unit(1124, MigraDoc.DocumentObjectModel.UnitType.Point);
						case MigraDoc.DocumentObjectModel.PageFormat.Legal: 
						case MigraDoc.DocumentObjectModel.PageFormat.Letter: return new Unit(612, MigraDoc.DocumentObjectModel.UnitType.Point);
						case MigraDoc.DocumentObjectModel.PageFormat.P11x17: return new Unit(792, MigraDoc.DocumentObjectModel.UnitType.Point);
					}
					return new Unit(width, MigraDoc.DocumentObjectModel.UnitType.Millimeter);
				}
				return new Unit(_model.PageWidth);
			}
			set => _model.PageWidth = value.GetModel();
		}

        public Unit PageHeight
		{
			get
			{
				//This is to handle the fact that we have to empty out the PageHeight property when setting PageFormat
				if (_model.PageHeight.IsEmpty)
				{
					int a0Height = 1189;
					int a0Width = 841;
					int height = 0;
					switch (_model.PageFormat)
					{
						case MigraDoc.DocumentObjectModel.PageFormat.A0: height = a0Height; break;
						case MigraDoc.DocumentObjectModel.PageFormat.A1: height = a0Width; break;
						case MigraDoc.DocumentObjectModel.PageFormat.A2: height = a0Height / 2; break;
						case MigraDoc.DocumentObjectModel.PageFormat.A3: height = a0Width / 2; break;
						case MigraDoc.DocumentObjectModel.PageFormat.A4: height = a0Height / 4; break;
						case MigraDoc.DocumentObjectModel.PageFormat.A5: height = a0Width / 4; break;
						case MigraDoc.DocumentObjectModel.PageFormat.A6: height = a0Height / 8; break;
						case MigraDoc.DocumentObjectModel.PageFormat.B5: height = 257; break;
						case MigraDoc.DocumentObjectModel.PageFormat.Letter:
						case MigraDoc.DocumentObjectModel.PageFormat.Ledger: return new Unit(792, MigraDoc.DocumentObjectModel.UnitType.Point);
						case MigraDoc.DocumentObjectModel.PageFormat.Legal: return new Unit(1008, MigraDoc.DocumentObjectModel.UnitType.Point);
						case MigraDoc.DocumentObjectModel.PageFormat.P11x17: return new Unit(1224, MigraDoc.DocumentObjectModel.UnitType.Point);
					}
					return new Unit(height, MigraDoc.DocumentObjectModel.UnitType.Millimeter);
				}
				return new Unit(_model.PageHeight);
			}
			set => _model.PageHeight = value.GetModel();
		}

        public string SectionStart
        {
            get => _model.SectionStart.ToString();
            set => _model.SectionStart = Parse.Enum<MigraDoc.DocumentObjectModel.BreakType>(value);
        }

        public int StartingNumber { get => _model.StartingNumber; set => _model.StartingNumber = value; }

        public Unit ContentWidth
        {
            get => PageWidth - LeftMargin - RightMargin;
            set => PageWidth = value + LeftMargin + RightMargin;
        }

        public Unit ContentHeight
        {
            get => PageHeight - TopMargin - BottomMargin;
            set => PageHeight = value + TopMargin + BottomMargin;
        }
    }
}
