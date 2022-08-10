using System;
using System.Collections.Generic;
using System.Text;

namespace Elistia.DotNetRtfWriter
{
    /// <summary>
    /// A container for an array of content blocks. For example, a footnote
    /// is a RtfBlockList because it may contains a paragraph and an image.
    /// </summary>
    public class RtfBlockList : RtfRenderable
    {
        /// <summary>
        /// Storage for array of content blocks.
        /// </summary>
        private List<RtfBlock> _blocks;
        /// <summary>
        /// Default character formats within this container.
        /// </summary>
        private RtfCharFormat _defaultCharFormat;

        private bool _allowParagraph;
        private bool _allowFootnote;
        private bool _allowControlWord;
        protected internal bool _allowImage;
        private bool _allowTable;


        /// <summary>
        ///  Reading Direction 
        /// </summary>
        public ReadingDirection ReadingDirection { get; set; }

        /// <summary>
        /// Based on Thread Culture ContentDirection will be set
        /// </summary>
        protected string ContentDirection
        {
            get {
                return ReadingDirection == ReadingDirection.RightToLeft ? "rtl" : "ltr";
            }
        }

        protected List<RtfBlock> Blocks
        {
            get { return _blocks; }
        }

        /// <summary>
        /// Get default character formats within this container.
        /// </summary>
        public RtfCharFormat DefaultCharFormat
        {
            get {
                if (_defaultCharFormat == null) {
                    _defaultCharFormat = new RtfCharFormat(-1, -1, 1);
                }
                return _defaultCharFormat;
            }
        }


        /// <summary>
        /// Internal use only.
        /// Default constructor that allows containing all types of content blocks.
        /// </summary>
        internal RtfBlockList()
            : this(true, true, true, true, true)
        {
        }

        /// <summary>
        /// Internal use only.
        /// Constructor specifying allowed content blocks to be contained.
        /// </summary>
        /// <param name="allowParagraph">Whether an RtfParagraph is allowed.</param>
        /// <param name="allowTable">Whether RtfTable is allowed.</param>
        internal RtfBlockList(bool allowParagraph, bool allowTable)
            : this(allowParagraph, true, true, true, allowTable)
        {
        }

        /// <summary>
        /// Internal use only.
        /// Constructor specifying allowed content blocks to be contained.
        /// </summary>
        /// <param name="allowParagraph">Whether an RtfParagraph is allowed.</param>
        /// <param name="allowFootnote">Whether an RtfFootnote is allowed in contained RtfParagraph.</param>
        /// <param name="allowControlWord">Whether an field control word is allowed in contained
        /// RtfParagraph.</param>
        /// <param name="allowImage">Whether RtfImage is allowed.</param>
        /// <param name="allowTable">Whether RtfTable is allowed.</param>
        internal RtfBlockList(bool allowParagraph, bool allowFootnote, bool allowControlWord,
                              bool allowImage, bool allowTable)
        {
            _blocks = new List<RtfBlock>();
            _allowParagraph = allowParagraph;
            _allowFootnote = allowFootnote;
            _allowControlWord = allowControlWord;
            _allowImage = allowImage;
            _allowTable = allowTable;
            _defaultCharFormat = null;
        }

        protected internal void AddBlock(RtfBlock block)
        {
            if (block != null) {
                _blocks.Add(block);
            }
        }

        /// <summary>
        /// Add a paragraph to this container.
        /// </summary>
        /// <returns>Paragraph being added.</returns>
        public RtfParagraph AddParagraph()
        {
            if (!_allowParagraph) {
                throw new Exception("Paragraph is not allowed.");
            }
            RtfParagraph block = new RtfParagraph(_allowFootnote, _allowControlWord, ReadingDirection);
            AddBlock(block);
            return block;
        }

        /// <summary>
        /// Add a section to this container
        /// </summary>
        public RtfSection AddSection(SectionStartEnd type, RtfDocument doc)
        {
            var block = new RtfSection(type, doc, ReadingDirection);
            AddBlock(block);
            return block;
        }

        /// <summary>
        /// Add a table to this container.
        /// </summary>
        /// <param name="rowCount">Number of rows in the table.</param>
        /// <param name="colCount">Number of columns in the table.</param>
        /// <param name="horizontalWidth">Horizontabl width (in points) of the table.</param>
        /// <param name="fontSize">The size of font used in this table. This is used to calculate margins.</param>
        /// <returns>Table begin added.</returns>
        public RtfTable AddTable(int rowCount, int colCount, float horizontalWidth, float fontSize)
        {
            if (!_allowTable) {
                throw new Exception("Table is not allowed.");
            }
            RtfTable block = new RtfTable(rowCount, colCount, horizontalWidth, fontSize, ReadingDirection);
            AddBlock(block);
            return block;
        }

        /// <summary>
        /// Internal use only.
        /// Transfer all content blocks to another RtfBlockList object.
        /// </summary>
        /// <param name="target">Target RtfBlockList object to transfer to.</param>
        internal void TransferBlocksTo(RtfBlockList target)
        {
            for (int i = 0; i < _blocks.Count; i++) {
                target.AddBlock(_blocks[i]);
            }
            _blocks.Clear();
        }

        /// <summary>
        /// Internal use only.
        /// Emit RTF code.
        /// </summary>
        /// <returns>Resulting RTF code for this object.</returns>
        public override string Render()
        {
            StringBuilder result = new StringBuilder();

            result.AppendLine();
            for (int i = 0; i < _blocks.Count; i++) {
                if (_defaultCharFormat != null && _blocks[i].DefaultCharFormat != null) {
                    _blocks[i].DefaultCharFormat.CopyFrom(_defaultCharFormat);
                }
                result.AppendLine(_blocks[i].Render());
            }
            return result.ToString();
        }
    }
}
