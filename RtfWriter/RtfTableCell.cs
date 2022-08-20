using System;
using System.Text;

namespace Elistia.DotNetRtfWriter
{
    /// <summary>
    /// Summary description for RtfTableCell
    /// </summary>
    public class RtfTableCell : RtfBlockList
    {
        private float _width;
        private Align _halign;
        private AlignVertical _valign;
        private Borders _borders;
        private CellMergeInfo _mergeInfo;
        private int _rowIndex;
        private int _colIndex;


        internal RtfTableCell(float width, int rowIndex, int colIndex, RtfTable parentTable)
            : this(width, rowIndex, colIndex, parentTable, ReadingDirection.LeftToRight)
        {
        }

        internal RtfTableCell(float width, int rowIndex, int colIndex, RtfTable parentTable, ReadingDirection direction)
            : base(true, false)
        {
            _width = width;
            _halign = Align.None;
            _valign = AlignVertical.Top;
            _borders = new Borders();
            _mergeInfo = null;
            _rowIndex = rowIndex;
            _colIndex = colIndex;
            BackgroundColor = null;
            ParentTable = parentTable;
            ReadingDirection = direction;
        }

        internal bool IsBeginOfColSpan
        {
            get {
                if (_mergeInfo == null) {
                    return false;
                }
                return (_mergeInfo.ColIndex == 0);
            }
        }

        internal bool IsBeginOfRowSpan
        {
            get {
                if (_mergeInfo == null) {
                    return false;
                }
                return (_mergeInfo.RowIndex == 0);
            }
        }

        public bool IsMerged
        {
            get {
                if (_mergeInfo == null) {
                    return false;
                }
                return true;
            }
        }

        internal CellMergeInfo MergeInfo
        {
            get {
                return _mergeInfo;
            }
            set {
                _mergeInfo = value;
            }
        }

        public float Width
        {
            get {
                return _width;
            }
            set {
                _width = value;
            }
        }

        public Borders Borders
        {
            get {
                return _borders;
            }
        }

        public RtfTable ParentTable { get; private set; }

        public ColorDescriptor BackgroundColor { get; set; }

        public Align Alignment
        {
            get {
                return _halign;
            }
            set {
                _halign = value;
            }
        }

        public AlignVertical AlignmentVertical
        {
            get {
                return _valign;
            }
            set {
                _valign = value;
            }
        }

        public int RowIndex
        {
            get {
                return _rowIndex;
            }
        }

        public int ColIndex
        {
            get {
                return _colIndex;
            }
        }

        public float OuterLeftBorderClearance { get; set; }

        public void SetBorderColor(ColorDescriptor color)
        {
            this.Borders[Direction.Top].Color = color;
            this.Borders[Direction.Bottom].Color = color;
            this.Borders[Direction.Left].Color = color;
            this.Borders[Direction.Right].Color = color;
        }

        public override string Render()
        {
            StringBuilder result = new StringBuilder();
            string align = "";

            switch (_halign) {
                case Align.Left:
                    align = @"\ql";
                    break;
                case Align.Right:
                    align = @"\qr";
                    break;
                case Align.Center:
                    align = @"\qc";
                    break;
                case Align.FullyJustify:
                    align = @"\qj";
                    break;
                case Align.Distributed:
                    align = @"\qd";
                    break;
            }


            if (base.Blocks.Count <= 0) {
                result.AppendLine(@"\pard\intbl");
            } else {
                for (int i = 0; i < base.Blocks.Count; i++) {
                    RtfBlock block = base.Blocks[i];
                    if (DefaultCharFormat != null && block.DefaultCharFormat != null) {
                        block.DefaultCharFormat.CopyFrom(DefaultCharFormat);
                    }
                    if (block.Margins[Direction.Top] < 0) {
                        block.Margins[Direction.Top] = 0;
                    }
                    if (block.Margins[Direction.Right] < 0) {
                        block.Margins[Direction.Right] = 0;
                    }
                    if (block.Margins[Direction.Bottom] < 0) {
                        block.Margins[Direction.Bottom] = 0;
                    }
                    if (block.Margins[Direction.Left] < 0) {
                        block.Margins[Direction.Left] = 0;
                    }
                    if (i == 0) {
                        block.BlockHead = @"\pard\intbl" + align;
                    } else {
                        block.BlockHead = @"\par" + align;
                    }
                    block.BlockTail = "";
                    result.AppendLine(block.Render());
                }
            }

            result.AppendLine(@"\cell");
            return result.ToString();
        }
    }
}
