using MigraDoc.DocumentObjectModel.Tables;
using PdfSharp.Pdf.Content.Objects;
using RtfPipe;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.IO;

namespace DiaryJournal.Net
{
    public static class RtbTablesFramework
    {


        //********************************************************************
        //new addition below********************************************************
        private const int EM_INSERTTABLE = (RichTextUser.WM_USER + 232);
        private const int EM_GETTABLEPARMS = (RichTextUser.WM_USER + 265);
        private const int EM_SETTABLEPARMS = (RichTextUser.WM_USER + 307);

        public const uint E_INVALIDARG = 0x80070057;
        public const int S_OK = 0;

        internal const int PFE_TABLEROW = 0xc000;           /* These 3 options are mutually */
        internal const int PFE_TABLECELLEND = 0x8000;           /*  exclusive and each imply    */
        internal const int PFE_TABLECELL = 0x4000;           /*  that para is part of a table*/
        internal const int PFM_TABLE = unchecked((int)0xc0000000);       /* (*)  */

        internal const int MAX_TABLE_CELLS = 63;

        // Data type defining table rows for EM_INSERTTABLE
        [StructLayout(LayoutKind.Sequential)]//, CharSet = CharSet.Auto, Pack = 0)]
        public struct TABLEROWPARMS
        {
            public byte cbRow;        // Count of bytes in this structure
            public byte cbCell;                 // Count of bytes in TABLECELLPARMS
            public byte cCell;        // Count of cells
            public byte cRow;        // Count of rows
            public Int32 dxCellMargin;     // Cell left/right margin (\trgaph)
            public Int32 dxIndent;     // Row left (right if fRTL indent (similar to \trleft)
            public Int32 dyHeight;     // Row height (\trrh)
            public UInt32 nParams;       // 0 - 2 bits - Row alignment (like PARAFORMAT::bAlignment, 1/2/3) (\trql, trqr, \trqc)
                                         // 3 bit - Display cells in RTL order (\rtlrow)
                                         // 4 bit - Keep row together (\trkeep}
                                         // 5 bit - Keep row on same page as following row (\trkeepfollow)
                                         // 6 bit - Wrap text to right/left (depending on bAlignment) (see \tdfrmtxtLeftN, \tdfrmtxtRightN)
                                         // 7 bit - lparam points at single struct valid for all cells
            public Int32 cpStartRow;     // The character position that indicates where to insert table. A value of –1 indicates the character position of the selection. 
            public byte bTableLevel;        // The table nesting level (EM_GETTABLEPARMS only).
            public byte iCell;        // The index of the cell to insert or delete (EM_SETTABLEPARMS only).
        }

        // Data type defining table cells for EM_INSERTTABLE
        [StructLayout(LayoutKind.Sequential)]//, CharSet = CharSet.Auto, Pack = 0)]
        public struct TABLECELLPARMS
        {

            public Int32 dxWidth;
            public UInt16 _bitfield;
            public UInt16 wShading;
            public Int16 dxBrdrLeft;
            public Int16 dyBrdrTop;
            public Int16 dxBrdrRight;
            public Int16 dyBrdrBottom;
            public UInt32 crBrdrLeft;
            public UInt32 crBrdrTop;
            public UInt32 crBrdrRight;
            public UInt32 crBrdrBottom;
            public UInt32 crBackPat;
            public UInt32 crForePat;
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int SendMessage(IntPtr hWnd, uint msg, ref TABLEROWPARMS wParam, ref TABLECELLPARMS lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int SendMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int SendMessage(IntPtr hWnd, uint msg, ref TABLEROWPARMS wParam, IntPtr lParam);

        [DllImport("kernel32.dll")]
        private static extern void RtlZeroMemory(IntPtr dst, UIntPtr length);

        public static int GetTableParms(ref RichTextBox rtb, ref int outTotalCellsInRow)
        {
            //            IntPtr rowparmsptr = Marshal.AllocHGlobal(sizeof(TABLEROWPARMS));
            //          IntPtr cellparmsptr = Marshal.AllocHGlobal(sizeof(TABLECELLPARMS));

            TABLEROWPARMS rowparms = new TABLEROWPARMS();
            TABLECELLPARMS cellparms = new TABLECELLPARMS();
            rowparms.cbRow = (byte)Marshal.SizeOf(rowparms);
            rowparms.cbCell = (byte)Marshal.SizeOf(cellparms);
            rowparms.cpStartRow = 0;
            rowparms.cRow = 0;
            rowparms.cCell = 0;// MAX_TABLE_CELLS;
            rowparms.iCell = 0;
            //        Marshal.StructureToPtr(rowparms, rowparmsptr, true);

            //      int result = SendMessage(rtb.Handle, EM_GETTABLEPARMS, rowparmsptr, cellparmsptr);
            int result = SendMessage(rtb.Handle, EM_GETTABLEPARMS, ref rowparms, ref cellparms);
            outTotalCellsInRow = rowparms.cCell;

            //rowparms = (TABLEROWPARMS)Marshal.PtrToStructure(rowparmsptr, typeof(TABLEROWPARMS));
            //List<TABLECELLPARMS> outCells = findAllCellsParms(rowparms.cCell, cellparmsptr);
            return result;
        }
        public static int InsertTableCell(RichTextBox rtb, int currentCellIndex, bool insertBefore)
        {
            int selindex = rtb.SelectionStart;
            int sellen = rtb.SelectionLength;

            // first we get some config
            TABLEROWPARMS rowparms = new TABLEROWPARMS();
            TABLECELLPARMS cellparms = new TABLECELLPARMS();
            IntPtr cellsParmsPtr = Marshal.AllocHGlobal((Marshal.SizeOf(cellparms) * MAX_TABLE_CELLS));
            rowparms.cbRow = (byte)Marshal.SizeOf(rowparms);
            rowparms.cbCell = (byte)Marshal.SizeOf(cellparms);
            rowparms.cpStartRow = 1;
            rowparms.cRow = 3;
            rowparms.cCell = MAX_TABLE_CELLS;
            int result = SendMessage(rtb.Handle, EM_GETTABLEPARMS, ref rowparms, cellsParmsPtr);
            if (result != S_OK) return result;

            // we received cells config
            // check if cells are at limit
            if (rowparms.cCell >= MAX_TABLE_CELLS) return -1;

            // now take the current Cell into view
            Int64 currentCellParmsIndex = (cellsParmsPtr.ToInt64() + ((Marshal.SizeOf(cellparms) * currentCellIndex)));
            IntPtr currentCellParmsPtr = new IntPtr(currentCellParmsIndex);
            TABLECELLPARMS currentCellparms = (TABLECELLPARMS)Marshal.PtrToStructure(currentCellParmsPtr, typeof(TABLECELLPARMS));
            //return -1;

            //TABLECELLPARMS currentCellParam = 
            // now try to insert a cell
            //rowparms.cRow = 255;
            //rowparms.cCell++;
            //rowparms.iCell = (byte)currentCellIndex;
            //rowparms.cpStartRow = 0;

            // configure row (required)
            rowparms.nParams = 0;
            rowparms.nParams = TurnBitOnUInt32(rowparms.nParams, 0);
            //newrowparms.nParams = TurnBitOnUInt32(newrowparms.nParams, 4);
            //newrowparms.nParams = TurnBitOnUInt32(newrowparms.nParams, 5);
            rowparms.nParams = TurnBitOnUInt32(rowparms.nParams, 6);
            rowparms.nParams = TurnBitOnUInt32(rowparms.nParams, 7);

            // if insertBefore then cell is inserted before the given index
            // if not insertBefore then cell is inserted after the given index
            if (insertBefore)
            {
                // if always inserts before the given cell index, so we do not change given cell index
            }
            else
            {
                // we need to increment the cell index by one so that the new cell is inserted after the given cell index.
                if (currentCellIndex >= 0)
                    rowparms.iCell++;
            }

            // now take the new Cell into view and copy the current cell's config into it.
            //Int64 newCellParmsIndex = (cellsParmsPtr.ToInt64() + ((Marshal.SizeOf(cellparms) * rowparms.iCell)));
            //IntPtr newCellParmsPtr = new IntPtr(newCellParmsIndex);
            //Marshal.StructureToPtr(currentCellparms, newCellParmsPtr, true);

            //rtb.SelectionStart = selindex;
            //rtb.SelectionLength = 0;

            // finally insert cell
            //result = SendMessage(rtb.Handle, EM_SETTABLEPARMS, ref rowparms, cellsParmsPtr);

            rtb.SelectionStart = selindex;
            rtb.SelectionLength = 0;

            rowparms.cRow = 1;
            rowparms.cpStartRow = 0;
            result = SendMessage(rtb.Handle, EM_INSERTTABLE, ref rowparms, cellsParmsPtr);

            // free memory
            Marshal.FreeHGlobal(cellsParmsPtr);

            return result;
        }

        public static bool findTables(String rtf, out List<RtfTable> outTables)
        {
            // prepare regex
            RegexOptions regexOptions = new RegexOptions();
            regexOptions |= RegexOptions.IgnoreCase;
            regexOptions |= RegexOptions.Singleline;

            String tableCellTag = @"\cellx";
            String flattened = Regex.Escape(tableCellTag);
            tableCellTag = flattened;
            String pattern = String.Format("{0}{1}", tableCellTag, @"(\d+)");

            // now load regex with pattern and options and find matches
            Regex regexCellTag = new Regex(pattern, regexOptions);
            MatchCollection matchesCellTag = regexCellTag.Matches(rtf, 0);

            // error checking
            if (matchesCellTag.Count == 0)
            {
                outTables = null;
                return false;
            }

            // found tables, parse and build objects


            outTables = null;
            return true;
        }

        // insert a table row
        public static int InsertTable(ref AdvRichTextBox rtb)
        {
            //            rtb.SelectedRtf = @"{\rtf1\ansi\deff0\v hello world\v0\trowd\cellx1000\cellx2000\cellx3000\intbl cell 1\cell\intbl cell 2\cell\intbl cell 3\cell\row}";
            //            rtb.SelectedRtf = @"{\rtf1\ansi\deff0{\comment 123}\trowd\cellx1000\cellx2000\cellx3000\intbl cell 1\cell\intbl cell 2\cell\intbl cell 3\cell\row}";
            //            rtb.SelectedRtf = @"{\rtf1\ansi\deff0{\info{\title Template}{\author John Doe}{\operator JOHN DOE}{\creatim\yr1999\mo4\dy27\min1}{\revtim\yr1999\mo4\dy27\min1}{\printim\yr1999\mo3\dy17\hr23\min5}{\version2}{\edmins2}{\nofpages183}{\nofwords53170}{\nofchars303071}{\*\company Microsoft}{\nofcharsws372192}{\vern8247}{\id10101} }\trowd\cellx1000\cellx2000\cellx3000\intbl cell 1\cell\intbl cell 2\cell\intbl cell 3\cell\row}";
            //return 0;

            TABLEROWPARMS newrowparms = new TABLEROWPARMS();
            TABLECELLPARMS newcellparms = new TABLECELLPARMS();
            newrowparms.cbRow = (byte)Marshal.SizeOf(newrowparms);
            newrowparms.cbCell = (byte)Marshal.SizeOf(newcellparms);
            newrowparms.bTableLevel = 1;
            newrowparms.cCell = 4;
            newrowparms.cRow = 5;
            newrowparms.cpStartRow = -1;
            newrowparms.dxCellMargin = 10;
            newrowparms.dxIndent = 0;
            newrowparms.dyHeight = 1000;

            /*
             * _bits in both structures
             *  newrowparms->nAlignment = 1;
                newrowparms->fRTL = 0;
                newrowparms->fKeep = 1;
                newrowparms->fKeepFollow = 1;
                newrowparms->fWrap = 1;
                newrowparms->fIdentCells = 1;

                newcellparms->nVertAlign = 0;
                //cells.fMergeTop := 1;
                //cells.fMergePrev := 1;
                newcellparms->fVertical = 1;

            public UInt32 nParams;       // 0 - 2 bits - Row alignment (like PARAFORMAT::bAlignment, 1/2/3) (\trql, trqr, \trqc)
                                         // 3 bit - Display cells in RTL order (\rtlrow)
                                         // 4 bit - Keep row together (\trkeep}
                                         // 5 bit - Keep row on same page as following row (\trkeepfollow)
                                         // 6 bit - Wrap text to right/left (depending on bAlignment) (see \tdfrmtxtLeftN, \tdfrmtxtRightN)
                                         // 7 bit - lparam points at single struct valid for all cells

            // cell struct
	        WORD	nVertAlign:2;	// Vertical alignment (0/1/2 = top/center/bottom
	        //	\clvertalt (def), \clvertalc, \clvertalb)
	        WORD	fMergeTop:1;	// Top cell for vertical merge (\clvmgf)
	        WORD	fMergePrev:1;	// Merge with cell above (\clvmrg)
	        WORD	fVertical:1;	// Display text top to bottom, right to left (\cltxtbrlv)
	        WORD	fMergeStart:1;	// Start set of horizontally merged cells (\clmgf)
	        WORD	fMergeCont:1;	// Merge with previous cell (\clmrg)

            */

            
            newrowparms.nParams = 0;
            newrowparms.nParams = TurnBitOnUInt32(newrowparms.nParams, 0);
            //newrowparms.nParams = TurnBitOnUInt32(newrowparms.nParams, 4);
            //newrowparms.nParams = TurnBitOnUInt32(newrowparms.nParams, 5);
            newrowparms.nParams = TurnBitOnUInt32(newrowparms.nParams, 6);
            newrowparms.nParams = TurnBitOnUInt32(newrowparms.nParams, 7);
            
            newcellparms.dxWidth = 1000;
            newcellparms._bitfield = 0;
            newcellparms._bitfield = TurnBitOnUInt16(newcellparms._bitfield, 4);
            newcellparms.crBrdrBottom = 0;
            newcellparms.crBrdrTop = 0;
            newcellparms.crBrdrLeft = 0;
            newcellparms.crBrdrRight = 0;
            newcellparms.dxBrdrLeft = 100;
            newcellparms.dyBrdrTop = 100;
            newcellparms.dxBrdrRight = 100;
            newcellparms.dyBrdrBottom = 100;
            newcellparms.crBackPat = 0xFFFFFF;
            newcellparms.crForePat = 0x0;
            newcellparms.wShading = 0;

            //rtb.SelectedRtf = @"{\rtf1\ansi\deff0\v table-start\v0}";
            int result = SendMessage(rtb.Handle, EM_INSERTTABLE, ref newrowparms, ref newcellparms);
            //rtb.SelectedRtf += @"{\rtf1\ansi\deff0\v table-end\v0}";


            return result;
        }

        public static uint TurnBitOnUInt32(uint value, int bitToTurnOn)
        {
            // Set a bit at position to 1.
            value |= (uint)1 << bitToTurnOn;

            return value;
        }
        public static uint TurnBitOffUInt32(uint value, int bitToTurnOff)
        {
            // unset a bit at position to 0.
            value &= ~((uint)1 << bitToTurnOff);
            return value;
        }
        public static ushort TurnBitOnUInt16(ushort value, int bitToTurnOn)
        {
            // Set a bit at position to 1.
            value |= (ushort)(1 << bitToTurnOn);
            return value;
        }
        public static ushort TurnBitOffUInt16(ushort value, int bitToTurnOff)
        {
            // unset a bit at position to 0.
            value &= (ushort)~((1 << bitToTurnOff));
            return value;
        }

        private static UInt32 GetUIntFromBitArray(BitArray bitArray)
        {
            var array = new byte[4];
            bitArray.CopyTo(array, 0);
            return BitConverter.ToUInt32(array, 0);
        }

        public static uint ToggleBitUInt(uint value, int bitPosition, bool toggle)
        {
            uint mask = 1;
            if (toggle)
            {
                mask = mask << bitPosition;
                value |= mask; //((uint)1 << bitPosition);
            }
            else
            {
                mask = ~(mask << bitPosition);
                value &= mask;
            }
            return value;
        }
        public static UInt16 ToggleBitUInt16(UInt16 value, int bitPosition, bool toggle)
        {
            UInt16 mask = 1;
            if (toggle)
            {
                mask = (UInt16)(mask << bitPosition);
                value |= mask; //((uint)1 << bitPosition);
            }
            else
            {
                mask = ((UInt16)(mask << bitPosition));
                mask = (UInt16)~mask;
                value &= mask;
            }
            return value;
        }

        // this method retrieves an individual cell from all the row's cells
        public static int getTableCell(ref AdvRichTextBox rtb, int cellindex, out CellParameter? outCell)
        {
            // error checking
            if (rtb == null)
            {
                outCell = null;
                return -1;
            }
            // error checking
            if (cellindex > MAX_TABLE_CELLS)
            {
                outCell = null;
                return -1;
            }
            List<CellParameter> list = null;
            int result = getTableCells(ref rtb, out list);
            // error checking
            if (result != S_OK)
            {
                outCell = null;
                return result; // error
            }
            // error checking
            if (cellindex >= list.Count())
            {
                outCell = null;
                return -1;
            }

            // success
            outCell = list[cellindex];
            return result;
        }

        // this method retrieves all the cells in the row at rtb selection point.
        public static int getTableCells(ref AdvRichTextBox rtb, out List<CellParameter> outCells)
        {
            List<CellParameter> list = new List<CellParameter>();

            // first we get some config
            TABLEROWPARMS rowparms = new TABLEROWPARMS();
            TABLECELLPARMS cellparms = new TABLECELLPARMS();
            IntPtr cellsParmsPtr = Marshal.AllocHGlobal((Marshal.SizeOf(cellparms) * MAX_TABLE_CELLS));
            RtlZeroMemory(cellsParmsPtr, new UIntPtr((ulong)(Marshal.SizeOf(cellparms) * MAX_TABLE_CELLS)));
            rowparms.cbRow = (byte)Marshal.SizeOf(rowparms);
            rowparms.cbCell = (byte)Marshal.SizeOf(cellparms);
            rowparms.cpStartRow = 1;
            rowparms.cRow = 0;
            rowparms.cCell = MAX_TABLE_CELLS;

            // do query
            int result = SendMessage(rtb.Handle, EM_GETTABLEPARMS, ref rowparms, cellsParmsPtr);

            // error checking
            if (result != S_OK)
            {
                Marshal.FreeHGlobal(cellsParmsPtr);
                outCells = null;
                return result;
            }

            // we received cells config
            // now collect all cells into a list
            Int64 offset = cellsParmsPtr.ToInt64();
            Int64 stride = Marshal.SizeOf(cellparms);
            Int64 size = stride * rowparms.cCell;           
            while (size > 0) 
            {
                // collect the cell and add it into list
                CellParameter cell = new CellParameter(offset);
                list.Add(cell);

                // configure loop
                offset += stride;
                size -= stride;
            }

            // success
            Marshal.FreeHGlobal(cellsParmsPtr);
            outCells = list;
            return result;
        }
        
        // this method sets all the given cells into the table
        public static int setTableCells(ref AdvRichTextBox rtb, ref List<CellParameter> cells)
        {
            List<CellParameter> list = new List<CellParameter>();

            // first we get some config
            TABLEROWPARMS rowparms = new TABLEROWPARMS();
            TABLECELLPARMS cellparms = new TABLECELLPARMS();
            IntPtr cellsParmsPtr = Marshal.AllocHGlobal((Marshal.SizeOf(cellparms) * MAX_TABLE_CELLS));
            RtlZeroMemory(cellsParmsPtr, new UIntPtr((ulong)(Marshal.SizeOf(cellparms) * MAX_TABLE_CELLS)));
            rowparms.cbRow = (byte)Marshal.SizeOf(rowparms);
            rowparms.cbCell = (byte)Marshal.SizeOf(cellparms);
            rowparms.cpStartRow = 1;
            rowparms.cRow = 0;
            rowparms.cCell = MAX_TABLE_CELLS;

            // do query
            int result = SendMessage(rtb.Handle, EM_GETTABLEPARMS, ref rowparms, cellsParmsPtr);

            // error checking
            if (result != S_OK)
            {
                Marshal.FreeHGlobal(cellsParmsPtr);
                return result;
            }

            // we received cells config
            // now collect all cells into a list
            Int64 offset = cellsParmsPtr.ToInt64();
            Int64 stride = Marshal.SizeOf(cellparms);
            Int64 size = stride * cells.Count();
            int index = 0;
            while (size > 0)
            {
                // copy the current cell into the destination
                CellParameter cell = cells[index++];
                cell.ToMemoryPtr(offset);

                // configure loop
                offset += stride;
                size -= stride;
            }

            rowparms.nParams = 0;
            rowparms.nParams = TurnBitOnUInt32(rowparms.nParams, 0);
            rowparms.nParams = TurnBitOnUInt32(rowparms.nParams, 4);
            rowparms.nParams = TurnBitOnUInt32(rowparms.nParams, 5);
            rowparms.nParams = TurnBitOnUInt32(rowparms.nParams, 6);
            rowparms.nParams = TurnBitOnUInt32(rowparms.nParams, 7);

            // finally set table
            result = SendMessage(rtb.Handle, EM_SETTABLEPARMS, ref rowparms, cellsParmsPtr);

            // success
            Marshal.FreeHGlobal(cellsParmsPtr);
            return result;
        }


        public class CellParameter
        {
            public Int32 dxWidth;
            public UInt16 _bitfield;
            public UInt16 wShading;
            public Int16 dxBrdrLeft;
            public Int16 dyBrdrTop;
            public Int16 dxBrdrRight;
            public Int16 dyBrdrBottom;
            public UInt32 crBrdrLeft;
            public UInt32 crBrdrTop;
            public UInt32 crBrdrRight;
            public UInt32 crBrdrBottom;
            public UInt32 crBackPat;
            public UInt32 crForePat;

            public CellParameter()
            {

            }

            public CellParameter(Int64 src)
            {
                IntPtr ptr = new IntPtr(src);
                byte[] block = new byte[Marshal.SizeOf(typeof(TABLECELLPARMS))];
                Marshal.Copy(ptr, block, 0, block.Length);
                MemoryStream ms = new MemoryStream(block);
                BinaryReader reader = new BinaryReader(ms);
                this.dxWidth = reader.ReadInt32();
                this._bitfield = reader.ReadUInt16();
                this.wShading = reader.ReadUInt16();
                this.dxBrdrLeft = reader.ReadInt16();
                this.dyBrdrTop = reader.ReadInt16();
                this.dxBrdrRight = reader.ReadInt16();
                this.dyBrdrBottom = reader.ReadInt16();
                this.crBrdrLeft = reader.ReadUInt32();
                this.crBrdrTop = reader.ReadUInt32();
                this.crBrdrRight = reader.ReadUInt32();
                this.crBrdrBottom = reader.ReadUInt32();
                this.crBackPat = reader.ReadUInt32();
                this.crForePat = reader.ReadUInt32();
            }
            public CellParameter(TABLECELLPARMS src)
            {
                this.dxWidth = src.dxWidth;
                this._bitfield = src._bitfield;
                this.wShading = src.wShading;
                this.dxBrdrLeft = src.dxBrdrLeft;
                this.dyBrdrTop = src.dyBrdrTop;
                this.dxBrdrRight = src.dxBrdrRight;
                this.dyBrdrBottom = src.dyBrdrBottom;
                this.crBrdrLeft = src.crBrdrLeft;
                this.crBrdrTop = src.crBrdrTop;
                this.crBrdrRight = src.crBrdrRight;
                this.crBrdrBottom = src.crBrdrBottom;
                this.crBackPat = src.crBackPat;
                this.crForePat = src.crForePat;
            }

            public void ToStructure(ref TABLECELLPARMS dest)
            {
                dest.dxWidth = this.dxWidth;
                dest._bitfield = this._bitfield;
                dest.wShading = this.wShading;
                dest.dxBrdrLeft = this.dxBrdrLeft;
                dest.dyBrdrTop = this.dyBrdrTop;
                dest.dxBrdrRight = this.dxBrdrRight;
                dest.dyBrdrBottom = this.dyBrdrBottom;
                dest.crBrdrLeft = this.crBrdrLeft;
                dest.crBrdrTop = this.crBrdrTop;
                dest.crBrdrRight = this.crBrdrRight;
                dest.crBrdrBottom = this.crBrdrBottom;
                dest.crBackPat = this.crBackPat;
                dest.crForePat = this.crForePat;
            }
            public void ToMemoryPtr(Int64 dest)
            {
                IntPtr ptr = new IntPtr(dest);
                byte[] block = new byte[Marshal.SizeOf(typeof(TABLECELLPARMS))];
                MemoryStream ms = new MemoryStream(block);
                BinaryWriter writer = new BinaryWriter(ms);
                writer.Write(this.dxWidth);
                writer.Write(this._bitfield);
                writer.Write(this.wShading);
                writer.Write(this.dxBrdrLeft);
                writer.Write(this.dyBrdrTop);
                writer.Write(this.dxBrdrRight);
                writer.Write(this.dyBrdrBottom);
                writer.Write(this.crBrdrLeft);
                writer.Write(this.crBrdrTop);
                writer.Write(this.crBrdrRight);
                writer.Write(this.crBrdrBottom);
                writer.Write(this.crBackPat);
                writer.Write(this.crForePat);
                Marshal.Copy(block, 0, ptr, block.Length);
            }


        }

        public class RtfTable
        {
            public int rowscount = 0;
            public List<RtfTableRow> rows = new List<RtfTableRow>();
        }

        public class RtfTableRow
        {
            public int cellscount = 0;
            public int height = 0;
            public List<RtfTableCell> cells = new List<RtfTableCell>();
        }

        public class RtfTableCell
        {
            public int width = 0;
            public int cellx = 0;
        }
    }
}
