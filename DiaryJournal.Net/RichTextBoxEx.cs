#define UNICODE

using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System;

namespace System.Windows.Forms
{
    public class RichTextBoxEx : RichTextBox
    {
        //private void SetCharFormatFont(bool selectionOnly, Font value)
        //{
        // byte[] bytes;
        // this.ForceHandleCreate();
        // NativeMethods.LOGFONT lOGFONT = new NativeMethods.LOGFONT();
        // RichTextBox.FontToLogFont(value, lOGFONT);
        // int num = -1476394993;
        // int num1 = 0;
        // if (value.Bold)
        // {
        // num1 = num1 | 1;
        // }
        // if (value.Italic)
        // {
        // num1 = num1 | 2;
        // }
        // if (value.Strikeout)
        // {
        // num1 = num1 | 8;
        // }
        // if (value.Underline)
        // {
        // num1 = num1 | 4;
        // }
        // if (Marshal.SystemDefaultCharSize == 1)
        // {
        // bytes = Encoding.Default.GetBytes(lOGFONT.lfFaceName);
        // NativeMethods.CHARFORMATA cHARFORMATum = new NativeMethods.CHARFORMATA();
        // for (int i = 0; i < (int)bytes.Length; i++)
        // {
        // cHARFORMATum.szFaceName[i] = bytes[i];
        // }
        // cHARFORMATum.dwMask = num;
        // cHARFORMATum.dwEffects = num1;
        // cHARFORMATum.yHeight = (int)(value.SizeInPoints * 20f);
        // cHARFORMATum.bCharSet = lOGFONT.lfCharSet;
        // cHARFORMATum.bPitchAndFamily = lOGFONT.lfPitchAndFamily;
        // UnsafeNativeMethods.SendMessage(new HandleRef(this, base.Handle), 1092, (selectionOnly ? 1 : 4), cHARFORMATum);
        // return;
        // }
        // bytes = Encoding.Unicode.GetBytes(lOGFONT.lfFaceName);
        // NativeMethods.CHARFORMATW cHARFORMATW = new NativeMethods.CHARFORMATW();
        // for (int j = 0; j < (int)bytes.Length; j++)
        // {
        // cHARFORMATW.szFaceName[j] = bytes[j];
        // }
        // cHARFORMATW.dwMask = num;
        // cHARFORMATW.dwEffects = num1;
        // cHARFORMATW.yHeight = (int)(value.SizeInPoints * 20f);
        // cHARFORMATW.bCharSet = lOGFONT.lfCharSet;
        // cHARFORMATW.bPitchAndFamily = lOGFONT.lfPitchAndFamily;
        // SendMessage(new HandleRef(this, base.Handle), 1092, (selectionOnly ? 1 : 4), cHARFORMATW);
        //}


        //private Font GetCharFormatFont(bool selectionOnly)
        //{
        // Font font;
        // this.ForceHandleCreate();
        // NativeMethods.CHARFORMATA charFormat = this.GetCharFormat(selectionOnly);
        // if ((charFormat.dwMask & 536870912) == 0)
        // {
        // return null;
        // }
        // string str = Encoding.Default.GetString(charFormat.szFaceName);
        // int num = str.IndexOf('\0');
        // if (num != -1)
        // {
        // str = str.Substring(0, num);
        // }
        // float single = 13f;
        // if ((charFormat.dwMask & -2147483648) != 0)
        // {
        // single = (float)charFormat.yHeight / 20f;
        // if (single == 0f && charFormat.yHeight > 0)
        // {
        // single = 1f;
        // }
        // }
        // FontStyle fontStyle = FontStyle.Regular;
        // if ((charFormat.dwMask & 1) != 0 && (charFormat.dwEffects & 1) != 0)
        // {
        // fontStyle = fontStyle | FontStyle.Bold;
        // }
        // if ((charFormat.dwMask & 2) != 0 && (charFormat.dwEffects & 2) != 0)
        // {
        // fontStyle = fontStyle | FontStyle.Italic;
        // }
        // if ((charFormat.dwMask & 8) != 0 && (charFormat.dwEffects & 8) != 0)
        // {
        // fontStyle = fontStyle | FontStyle.Strikeout;
        // }
        // if ((charFormat.dwMask & 4) != 0 && (charFormat.dwEffects & 4) != 0)
        // {
        // fontStyle = fontStyle | FontStyle.Underline;
        // }
        // try
        // {
        // font = new Font(str, single, fontStyle, GraphicsUnit.Point, charFormat.bCharSet);
        // }
        // catch
        // {
        // return null;
        // }
        // return font;
        //}

        //***************************************************************
        // new addition *************************************************

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        static extern IntPtr LoadLibrary(string dllName);
        // tushar jain: optional select RichEdit Control by version and select unicode "W" Class from it.
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams baseParams = base.CreateParams;
                if (LoadLibrary("msftedit.dll") != IntPtr.Zero)
                {
                    baseParams.ClassName = "RICHEDIT50W";
                }
                return baseParams;
            }
        }


        /*
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr LoadLibrary(string lpFileName);

        private IntPtr Handle;

        
        protected override CreateParams CreateParams
        {

            get
            {
                if (Handle == IntPtr.Zero)
                {
                    //string path = Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFilesX86);
                    String path = Application.StartupPath + "\\" + "msftedit.dll";//"riched20.dll";
                    Handle = LoadLibrary(path);
                    if ((long)Handle < 0x20) throw new Win32Exception(Marshal.GetLastWin32Error(), "RichEdit control appears to be missing");
                }
                CreateParams createParams = base.CreateParams;
                //createParams.ClassName = "RichEdit50W";
                if (this.Multiline)
                {
                    if (((this.ScrollBars & RichTextBoxScrollBars.Horizontal) != RichTextBoxScrollBars.None) && !base.WordWrap)
                    {
                        createParams.Style |= 0x100000;
                        if ((this.ScrollBars & ((RichTextBoxScrollBars)0x10)) != RichTextBoxScrollBars.None)
                        {
                            createParams.Style |= 0x2000;
                        }
                    }
                    if ((this.ScrollBars & RichTextBoxScrollBars.Vertical) != RichTextBoxScrollBars.None)
                    {
                        createParams.Style |= 0x200000;
                        if ((this.ScrollBars & ((RichTextBoxScrollBars)0x10)) != RichTextBoxScrollBars.None)
                        {
                            createParams.Style |= 0x2000;
                        }
                    }
                }
                if ((BorderStyle.FixedSingle == base.BorderStyle) && ((createParams.Style & 0x800000) != 0))
                {
                    createParams.Style &= -8388609;
                    createParams.ExStyle |= 0x200;
                }
                return createParams;
            }
            /*
            get
            {
                var cp = base.CreateParams;
                if (LoadLibrary("riched20.dll") != IntPtr.Zero)
                {
                    cp.ClassName = "RICHEDIT50W";
                }
                return cp;
            }

        }
    */

        //***************************************************************
        // new addition *************************************************
        public RichTextBoxEx()
            : base()
        {
            Selectable = true;
        }

        // ReSharper disable once InconsistentNaming
        private const int WM_SETFOCUS = 0x0007;
        // ReSharper disable once InconsistentNaming
        private const int WM_KILLFOCUS = 0x0008;


        protected override void OnReadOnlyChanged(EventArgs e)
        {
            base.OnReadOnlyChanged(e);
            this.OnBackColorChanged(EventArgs.Empty);
        }

        ///<summary>
        /// Enables or disables selection highlight. 
        /// If you set `Selectable` to `false` then the selection highlight
        /// will be disabled. 
        /// It's enabled by default.
        ///</summary>
        [DefaultValue(true)]
        public bool Selectable { get; set; }

        public override Cursor? Cursor
        {
            get => Selectable ? (Cursor?)base.Cursor : Cursors.Default;
            set => base.Cursor = value;
        }


        protected override void WndProc(ref Message m)
        {

            switch (m.Msg)
            {
                case WM_SETFOCUS when Selectable is false:
                    m.Msg = WM_KILLFOCUS;
                    base.WndProc(ref m);
                    break;
                default:
                    base.WndProc(ref m);
                    break;
            }
        }

        //***************************************************************
        // new addition *************************************************

        // **********************************************************
        // Add Numbered List Support

        #region PARAFORMAT MASK VALUES
        // PARAFORMAT mask values
        private const uint PFM_STARTINDENT = 0x00000001;
        private const uint PFM_RIGHTINDENT = 0x00000002;
        private const uint PFM_OFFSET = 0x00000004;
        //private const uint PFM_ALIGNMENT = 0x00000008;
        private const uint PFM_TABSTOPS = 0x00000010;
        private const uint PFM_NUMBERING = 0x00000020;
        private const uint PFM_OFFSETINDENT = 0x80000000;

        // PARAFORMAT 2.0 masks and effects
        private const uint PFM_SPACEBEFORE = 0x00000040;
        private const uint PFM_SPACEAFTER = 0x00000080;
        private const uint PFM_LINESPACING = 0x00000100;
        private const uint PFM_STYLE = 0x00000400;
        private const uint PFM_BORDER = 0x00000800; // (*)
        private const uint PFM_SHADING = 0x00001000; // (*)
        private const uint PFM_NUMBERINGSTYLE = 0x00002000; // RE 3.0
        private const uint PFM_NUMBERINGTAB = 0x00004000; // RE 3.0
        private const uint PFM_NUMBERINGSTART = 0x00008000; // RE 3.0

        private const uint PFM_RTLPARA = 0x00010000;
        private const uint PFM_KEEP = 0x00020000; // (*)
        private const uint PFM_KEEPNEXT = 0x00040000; // (*)
        private const uint PFM_PAGEBREAKBEFORE = 0x00080000; // (*)
        private const uint PFM_NOLINENUMBER = 0x00100000; // (*)
        private const uint PFM_NOWIDOWCONTROL = 0x00200000; // (*)
        private const uint PFM_DONOTHYPHEN = 0x00400000; // (*)
        private const uint PFM_SIDEBYSIDE = 0x00800000; // (*)
        private const uint PFM_TABLE = 0x40000000; // RE 3.0
        private const uint PFM_TEXTWRAPPINGBREAK = 0x20000000; // RE 3.0
        private const uint PFM_TABLEROWDELIMITER = 0x10000000; // RE 4.0

        // The following three properties are read only
        private const uint PFM_COLLAPSED = 0x01000000; // RE 3.0
        private const uint PFM_OUTLINELEVEL = 0x02000000; // RE 3.0
        private const uint PFM_BOX = 0x04000000; // RE 3.0
        private const uint PFM_RESERVED2 = 0x08000000; // RE 4.0

        public enum AdvRichTextBulletType
        {
            Normal = 1,
            Number = 2,
            LowerCaseLetter = 3,
            UpperCaseLetter = 4,
            LowerCaseRoman = 5,
            UpperCaseRoman = 6
        }

        public enum AdvRichTextBulletStyle
        {
            RightParenthesis = 0x000,
            DoubleParenthesis = 0x100,
            Period = 0x200,
            Plain = 0x300,
            NoNumber = 0x400
        }
        #endregion

        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        public static extern IntPtr SendMessage(System.Runtime.InteropServices.HandleRef hWnd, int msg, int wParam, [In, Out, MarshalAs(UnmanagedType.LPStruct)] PARAFORMAT2 lParam);

        private AdvRichTextBulletType _BulletType = AdvRichTextBulletType.Normal;
        private AdvRichTextBulletStyle _BulletStyle = AdvRichTextBulletStyle.Plain;
        private short _BulletNumberStart = 1;

        public AdvRichTextBulletType BulletType
        {
            get
            {
                return _BulletType;
            }
            set
            {
                _BulletType = value;
            }
        }
        public AdvRichTextBulletStyle BulletStyle
        {
            get
            {
                return _BulletStyle;
            }
            set
            {
                _BulletStyle = value;
            }
        }
        public short BulletNumberStart
        {
            get
            {
                return _BulletNumberStart;
            }
            set
            {
                _BulletNumberStart = value;
            }
        }
        public void NumberedBullet(bool TurnOn)
        {
            /*
            PARAFORMAT2 paraformat1 = new PARAFORMAT2();
            paraformat1.dwMask = (PARAFORMAT_MASKS)(PFM_NUMBERING | PFM_OFFSET | PFM_NUMBERINGSTYLE | PFM_NUMBERINGSTART | PFM_NUMBERINGTAB);
            if (!TurnOn)
            {
                paraformat1.wNumbering = 0;
                paraformat1.dxOffset = 0;
            }
            else
            {
                paraformat1.wNumbering = (PARAFORMAT_NUMBERING)_BulletType;
                paraformat1.dxOffset = this.BulletIndent;
                paraformat1.wNumberingStyle = (PARAFORMAT_NUMBERINGSTYLE)_BulletStyle;
                paraformat1.wNumberingStart = (ushort)_BulletNumberStart;
            }
            SendMessage(new System.Runtime.InteropServices.HandleRef(this, this.Handle), 0x447, 0, paraformat1); //tushar //paraformat1);
            */

            PARAFORMAT2 paraformat1 = new PARAFORMAT2();
            paraformat1.dwMask = (int)(PFM_NUMBERING | PFM_OFFSET | PFM_NUMBERINGSTART |
                PFM_NUMBERINGSTYLE | PFM_NUMBERINGTAB);
            if (!TurnOn)
            {
                paraformat1.wNumbering = 0;
                paraformat1.dxOffset = 0;
            }
            else
            {
                paraformat1.wNumbering = (short)_BulletType;
                paraformat1.dxOffset = this.BulletIndent;
                paraformat1.wNumberingStyle = (short)_BulletStyle;
                paraformat1.wNumberingStart = _BulletNumberStart;
                paraformat1.wNumberingTab = 500;
            }
            SendMessage(new System.Runtime.InteropServices.HandleRef(this, this.Handle),
                0x447, 0, paraformat1);

        }






        //***************************************************************
        // new addition *************************************************

        public const int WM_USER = 0x400;
        public const int EM_GETCHARFORMAT = WM_USER + 58;
        public const int EM_SETCHARFORMAT = WM_USER + 68;
        public const int EM_GETPARAFORMAT = WM_USER + 61;
        public const int EM_SETPARAFORMAT = WM_USER + 71;

        public const int EM_SETUNDOLIMIT = WM_USER + 82;

        public CHARFORMAT2W GetCharFormat(bool fSelection)
        {
            var format = new CHARFORMAT2W();
            format.cbSize = CHARFORMAT2W.SIZE;
            SendMessage(new HandleRef(this, Handle), EM_GETCHARFORMAT, (fSelection ? 1 : 0), ref format);
            return format;
        }

        public void SetCharFormat(bool fSelection, CHARFORMAT2W format)
        {
            format.cbSize = CHARFORMAT2W.SIZE;
            SendMessage(new HandleRef(this, Handle), EM_SETCHARFORMAT, (fSelection ? 1 : 0), ref format);
        }

        private const uint CFM_FACE = 536870912;
        private const uint CFM_SIZE = 2147483648;
        private const int TWIPS_PER_POINT = 20;
        private const int SCF_DEFAULT = 0;
        private const int SCF_SELECTION = 1;
        private const byte BL_SINGLE_LINE_SPACING = 0;
        private const byte BL_ONE_AND_HALF_LINE_SPACING = 1;
        private const byte BL_DOUBLE_LINE_SPACING = 2;
        //public const int PFM_SPACEAFTER = 128;
        //public const int PFM_LINESPACING = 256;

        /// <summary>
        /// Sets vertical line spacing between lines
        /// of text, for the current text selection.
        /// </summary>
        /// <param name="spacing">Set to one of the bLineSpacingRule (BL_) constants,
        /// such as "BL_SINGLE_LINE_SPACING".</param>
        /*
        public void SelectionLineSpacing(byte spacing)
        {
            // Ignore request if spacing value not valid.
            // Valid values are 0, 1 and 2.
            if (spacing <= 3)
            {
                PARAFORMAT2 pf = new PARAFORMAT2();
                pf.cbSize = Marshal.SizeOf(pf);
                pf.dwMask |= PFM_LINESPACING | PFM_SPACEAFTER;
                pf.bLineSpacingRule = spacing;
                SendMessage(new HandleRef(this, Handle), EM_SETPARAFORMAT, SCF_SELECTION, ref pf);
            }
        }
         */

        /*
        /// <summary>
        /// 设置行距
        /// </summary>
        /// <param name="ctl">控件</param>
        /// <param name="dyLineSpacing">间距</param>
        public void SelectionLineSpacing(Control ctl, int dyLineSpacing)
        {
            PARAFORMAT2 fmt = new PARAFORMAT2();
            fmt.cbSize = Marshal.SizeOf(fmt);
            fmt.bLineSpacingRule = 4;// bLineSpacingRule;
            fmt.dyLineSpacing = dyLineSpacing;
            fmt.dwMask |= PFM_LINESPACING | PFM_SPACEAFTER;
            try
            {
                SendMessage(new HandleRef(ctl, ctl.Handle), EM_SETPARAFORMAT, 0, ref fmt);
            }
            catch
            {

            }
        }
        */

        public enum LineSpaceTypes
        {
            Single, OneAndHalf, Double
        }

        //http://msdn.microsoft.com/en-us/library/bb787942(VS.85).aspx
        /// <summary>
        /// just testing, this is quite complicated
        /// http://msdn2.microsoft.com/en-us/library/aa140277(office.10).aspx
        /// </summary>
        public void LineSpace(LineSpaceTypes types, bool bSelectAll)
        {
            PARAFORMAT2 paraformat1 = new PARAFORMAT2();
            paraformat1.dwMask = (int)PFM_LINESPACING;
            paraformat1.cbSize = (int)Marshal.SizeOf(paraformat1);//(UInt32)Marshal.SizeOf(paraformat1);
            paraformat1.bLineSpacingRule = (byte)(((int)types));
            //paraformat1.wReserved = 0;

            switch (types)
            {
                case LineSpaceTypes.Single: paraformat1.dyLineSpacing = 20; break;
                case LineSpaceTypes.OneAndHalf: paraformat1.dyLineSpacing = 30; break;
                case LineSpaceTypes.Double: paraformat1.dyLineSpacing = 40; break;
            }

            //  paraformat1.dyLineSpacing = ((int)types)40; // the above commented lie. This does need to be set
            if (bSelectAll == true) this.SelectAll();

            SendMessage(new System.Runtime.InteropServices.HandleRef(this, this.Handle), 0x447, 0, paraformat1);
            this.SelectionLength = 0;
        }
        public CHARFORMAT2W SelectionCharFormat
        {
            get { return GetCharFormat(true); }
            set { SetCharFormat(true, value); }
        }

        public PARAFORMAT2 GetParaFormat(bool fSelection)
        {
            var format = new PARAFORMAT2();
            format.cbSize = (int)PARAFORMAT2.SIZE;
            SendMessage(new HandleRef(this, Handle), EM_GETPARAFORMAT, (fSelection ? 1 : 0), ref format);
            return format;
        }

        public void SetParaFormat(bool fSelection, PARAFORMAT2 format)
        {
            format.cbSize = (int)PARAFORMAT2.SIZE;
            SendMessage(new HandleRef(this, Handle), EM_SETPARAFORMAT, (fSelection ? 1 : 0), ref format);
        }

        public PARAFORMAT2 SelectionParaFormat
        {
            get { return GetParaFormat(true); }
            set { SetParaFormat(true, value); }
        }

        public int SetUndoLimit(int limit)
        {
            return (int)SendMessage(new HandleRef(this, Handle), EM_SETUNDOLIMIT, limit, 0);
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = false)]
        public static extern IntPtr SendMessage(HandleRef hWnd, int msg, int wParam, [In][Out] ref CHARFORMAT2W lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = false)]
        public static extern IntPtr SendMessage(HandleRef hWnd, int msg, int wParam, [In][Out] ref PARAFORMAT2 lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = false)]
        public static extern IntPtr SendMessage(HandleRef hWnd, int msg, int wParam, int lParam);
    }

    /*
    [StructLayout(LayoutKind.Sequential)]
    public class PARAFORMAT2
    {
        public static uint SIZE = (uint)Marshal.SizeOf(typeof(PARAFORMAT2));

        public uint cbSize;
        public PARAFORMAT_MASKS dwMask;
        public PARAFORMAT_NUMBERING wNumbering;
        public PARAFORMAT_EFFECTS wEffects;
        public int dxStartIndent;
        public int dxRightIndent;
        public int dxOffset;
        public PARAFORMAT_ALIGNMENT wAlignment;
        public short cTabCount;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public int[] rgxTabs;
        public int dySpaceBefore;
        public int dySpaceAfter;
        public int dyLineSpacing;
        public short sStyle;
        public byte bLineSpacingRule;
        public byte bOutlineLevel;
        public ushort wShadingWeight;
        public ushort wShadingStyle;
        public ushort wNumberingStart;
        public PARAFORMAT_NUMBERINGSTYLE wNumberingStyle;
        public ushort wNumberingTab;
        public ushort wBorderSpace;
        public ushort wBorderWidth;
        public ushort wBorders;


    }
    */

    #region BULLETING

    [StructLayout(LayoutKind.Sequential)]
    public class PARAFORMAT2
    {
        public static uint SIZE = (uint)Marshal.SizeOf(typeof(PARAFORMAT2));

        public int cbSize;
        public int dwMask;
        public short wNumbering;
        public short wReserved;
        public int dxStartIndent;
        public int dxRightIndent;
        public int dxOffset;
        public short wAlignment;
        public short cTabCount;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x20)]
        public int[] rgxTabs;

        public int dySpaceBefore;   // Vertical spacing before para
        public int dySpaceAfter;    // Vertical spacing after para
        public int dyLineSpacing;   // Line spacing depending on Rule
        public short sStyle;        // Style handle
        public byte bLineSpacingRule;   // Rule for line spacing (see tom.doc)
        public byte bOutlineLevel;  // Outline Level
        public short wShadingWeight;    // Shading in hundredths of a per cent
        public short wShadingStyle;     // Byte 0: style, nib 2: cfpat, 3: cbpat
        public short wNumberingStart;   // Starting value for numbering
        public short wNumberingStyle;   // Alignment, Roman/Arabic, (), ), ., etc.
        public short wNumberingTab;     // Space bet 1st indent and 1st-line text
        public short wBorderSpace;  // Border-text spaces (nbl/bdr in pts)
        public short wBorderWidth;  // Pen widths (nbl/bdr in half twips)
        public short wBorders;      // Border styles (nibble/border)

        public PARAFORMAT2()
        {
            this.cbSize = Marshal.SizeOf(typeof(PARAFORMAT2));
        }
    }
    #endregion

    public enum PARAFORMAT_LINESPACINGRULE : byte
    {
        Single = 0,
        OneAndAHalf = 1,
        Double = 2,

        Use_min_single_dyLineSpacing = 3,
        Use_exact_dyLineSpacing = 4,
        Use_dyLineSpacing_div_20 = 5 // aka use points (or, dyLineSpacing is in points, not twips)
    }

    [Flags]
    public enum PARAFORMAT_MASKS : uint
    {
        PFM_STARTINDENT = 0x00000001,
        PFM_RIGHTINDENT = 0x00000002,
        PFM_OFFSET = 0x00000004,
        PFM_ALIGNMENT = 0x00000008,
        PFM_TABSTOPS = 0x00000010,
        PFM_NUMBERING = 0x00000020,
        PFM_OFFSETINDENT = 0x80000000,

        PFM_SPACEBEFORE = 0x00000040,
        PFM_SPACEAFTER = 0x00000080,
        PFM_LINESPACING = 0x00000100,
        PFM_STYLE = 0x00000400,
        PFM_BORDER = 0x00000800,
        PFM_SHADING = 0x00001000,
        PFM_NUMBERINGSTYLE = 0x00002000,
        PFM_NUMBERINGTAB = 0x00004000,
        PFM_NUMBERINGSTART = 0x00008000,

        PFM_RTLPARA = 0x00010000,
        PFM_KEEP = 0x00020000,
        PFM_KEEPNEXT = 0x00040000,
        PFM_PAGEBREAKBEFORE = 0x00080000,
        PFM_NOLINENUMBER = 0x00100000,
        PFM_NOWIDOWCONTROL = 0x00200000,
        PFM_DONOTHYPHEN = 0x00400000,
        PFM_SIDEBYSIDE = 0x00800000,
        PFM_TABLE = 0x40000000,
        PFM_TEXTWRAPPINGBREAK = 0x20000000,
        PFM_TABLEROWDELIMITER = 0x10000000,

        PFM_COLLAPSED = 0x01000000,
        PFM_OUTLINELEVEL = 0x02000000,
        PFM_BOX = 0x04000000,
        PFM_RESERVED2 = 0x08000000,

        PFM_ALL = (PFM_STARTINDENT | PFM_RIGHTINDENT | PFM_OFFSET | PFM_ALIGNMENT | PFM_TABSTOPS | PFM_NUMBERING | PFM_OFFSETINDENT | PFM_RTLPARA),
        PFM_EFFECTS = (PFM_RTLPARA | PFM_KEEP | PFM_KEEPNEXT | PFM_TABLE | PFM_PAGEBREAKBEFORE | PFM_NOLINENUMBER | PFM_NOWIDOWCONTROL | PFM_DONOTHYPHEN | PFM_SIDEBYSIDE | PFM_TABLE | PFM_TABLEROWDELIMITER),
        PFM_ALL2 = (PFM_ALL | PFM_EFFECTS | PFM_SPACEBEFORE | PFM_SPACEAFTER | PFM_LINESPACING | PFM_STYLE | PFM_SHADING | PFM_BORDER | PFM_NUMBERINGTAB | PFM_NUMBERINGSTART | PFM_NUMBERINGSTYLE),
    }

    [Flags]
    public enum PARAFORMAT_EFFECTS : ushort
    {
        PFE_RTLPARA = (ushort)(PARAFORMAT_MASKS.PFM_RTLPARA >> 16),
        PFE_KEEP = (ushort)(PARAFORMAT_MASKS.PFM_KEEP >> 16),
        PFE_KEEPNEXT = (ushort)(PARAFORMAT_MASKS.PFM_KEEPNEXT >> 16),
        PFE_PAGEBREAKBEFORE = (ushort)(PARAFORMAT_MASKS.PFM_PAGEBREAKBEFORE >> 16),
        PFE_NOLINENUMBER = (ushort)(PARAFORMAT_MASKS.PFM_NOLINENUMBER >> 16),
        PFE_NOWIDOWCONTROL = (ushort)(PARAFORMAT_MASKS.PFM_NOWIDOWCONTROL >> 16),
        PFE_DONOTHYPHEN = (ushort)(PARAFORMAT_MASKS.PFM_DONOTHYPHEN >> 16),
        PFE_SIDEBYSIDE = (ushort)(PARAFORMAT_MASKS.PFM_SIDEBYSIDE >> 16),
        PFE_TEXTWRAPPINGBREAK = (ushort)(PARAFORMAT_MASKS.PFM_TEXTWRAPPINGBREAK >> 16),

        // The following four effects are read only
        PFE_COLLAPSED = (ushort)(PARAFORMAT_MASKS.PFM_COLLAPSED >> 16),
        PFE_BOX = (ushort)(PARAFORMAT_MASKS.PFM_BOX >> 16),
        PFE_TABLE = (ushort)(PARAFORMAT_MASKS.PFM_TABLE >> 16),
        PFE_TABLEROWDELIMITER = (ushort)(PARAFORMAT_MASKS.PFM_TABLEROWDELIMITER >> 16),
    }

    public enum PARAFORMAT_NUMBERING : ushort
    {
        // PARAFORMAT numbering options 
        PFN_BULLET = 1, // tomListBullet

        // PARAFORMAT2 wNumbering options 
        PFN_ARABIC = 2, // tomListNumberAsArabic: 0, 1, 2, ...
        PFN_LCLETTER = 3, // tomListNumberAsLCLetter: a, b, c, ...
        PFN_UCLETTER = 4, // tomListNumberAsUCLetter: A, B, C, ...
        PFN_LCROMAN = 5, // tomListNumberAsLCRoman: i, ii, iii, ...
        PFN_UCROMAN = 6, // tomListNumberAsUCRoman: I, II, III, ...
    }

    [Flags]
    public enum PARAFORMAT_NUMBERINGSTYLE : ushort
    {
        // PARAFORMAT2 wNumberingStyle options 
        PFNS_PAREN = 0x000, // default, e.g., 1) 
        PFNS_PARENS = 0x100, // tomListParentheses/256, e.g., (1) 
        PFNS_PERIOD = 0x200, // tomListPeriod/256, e.g., 1. 
        PFNS_PLAIN = 0x300, // tomListPlain/256, e.g., 1 
        PFNS_NONUMBER = 0x400, // Used for continuation w/o number

        PFNS_NEWNUMBER = 0x8000, // Start new number with wNumberingStart (can be combined with other PFNS_xxx)
    }

    public enum PARAFORMAT_ALIGNMENT : ushort
    {
        // PARAFORMAT alignment options 
        PFA_LEFT = 1,
        PFA_RIGHT = 2,
        PFA_CENTER = 3,

        // PARAFORMAT2 alignment options 
        PFA_JUSTIFY = 4,
        PFA_FULL_INTERWORD = 4,
        PFA_FULL_INTERLETTER = 5,
        PFA_FULL_SCALED = 6,
        PFA_FULL_GLYPHS = 7,
        PFA_SNAP_GRID = 8,
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct CHARFORMAT2W
    {
        public static uint SIZE = (uint)Marshal.SizeOf(typeof(CHARFORMAT2W));

        public uint cbSize;
        public CHARFORMAT_MASKS dwMask;
        public CHARFORMAT_EFFECTS dwEffects;
        public int yHeight;
        public int yOffset;
        public int crTextColor; // 0x00bbggrr
        public byte bCharSet;
        public byte bPitchAndFamily;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        public byte[] szFaceName;
        public ushort wWeight;
        public short sSpacing;
        public int crBackColor; // 0x00bbggrr
        public uint lcid;
        public uint dwReserved;
        public short sStyle;
        public ushort wKerning;
        public CHARFORMAT_UNDERLINE bUnderlineType;
        public byte bAnimation;
        public byte bRevAuthor;
        public byte bUnderlineColor; // bReserved1

        public string FaceName
        {
            get { return Encoding.Unicode.GetString(szFaceName); }
            set { szFaceName = Encoding.Unicode.GetBytes(value); }
        }

        public int yHeight_Points
        {
            get { return yHeight / 20; }
            set { yHeight = value * 20; }
        }
    }

    [Flags]
    public enum CHARFORMAT_MASKS : uint
    {
        CFM_BOLD = 0x00000001,
        CFM_ITALIC = 0x00000002,
        CFM_UNDERLINE = 0x00000004,
        CFM_STRIKEOUT = 0x00000008,
        CFM_PROTECTED = 0x00000010,
        CFM_LINK = 0x00000020, // Exchange hyperlink extension 
        CFM_SIZE = 0x80000000,
        CFM_COLOR = 0x40000000,
        CFM_FACE = 0x20000000,
        CFM_OFFSET = 0x10000000,
        CFM_CHARSET = 0x08000000,

        CFM_SMALLCAPS = 0x0040,
        CFM_ALLCAPS = 0x0080,
        CFM_HIDDEN = 0x0100,
        CFM_OUTLINE = 0x0200,
        CFM_SHADOW = 0x0400,
        CFM_EMBOSS = 0x0800,
        CFM_IMPRINT = 0x1000,
        CFM_DISABLED = 0x2000,
        CFM_REVISED = 0x4000,

        CFM_BACKCOLOR = 0x04000000,
        CFM_LCID = 0x02000000,
        CFM_UNDERLINETYPE = 0x00800000,
        CFM_WEIGHT = 0x00400000,
        CFM_SPACING = 0x00200000,
        CFM_KERNING = 0x00100000,
        CFM_STYLE = 0x00080000,
        CFM_ANIMATION = 0x00040000,
        CFM_REVAUTHOR = 0x00008000,

        CFM_SUBSCRIPT = (CHARFORMAT_EFFECTS.CFE_SUBSCRIPT | CHARFORMAT_EFFECTS.CFE_SUPERSCRIPT),
        CFM_SUPERSCRIPT = CFM_SUBSCRIPT,

        //CFM_EFFECTS = (CFM_BOLD | CFM_ITALIC | CFM_UNDERLINE | CFM_COLOR | CFM_STRIKEOUT | CHARFORMAT_EFFECTS.CFE_PROTECTED | CFM_LINK),
        //CFM_ALL = (CFM_EFFECTS | CFM_SIZE | CFM_FACE | CFM_OFFSET | CFM_CHARSET),
        //CFM_EFFECTS2 = (CFM_EFFECTS | CFM_DISABLED | CFM_SMALLCAPS | CFM_ALLCAPS | CFM_HIDDEN | CFM_OUTLINE | CFM_SHADOW | CFM_EMBOSS | CFM_IMPRINT | CFM_DISABLED | CFM_REVISED | CFM_SUBSCRIPT | CFM_SUPERSCRIPT | CFM_BACKCOLOR),
        //CFM_ALL2 = (CFM_ALL | CFM_EFFECTS2 | CFM_BACKCOLOR | CFM_LCID | CFM_UNDERLINETYPE | CFM_WEIGHT | CFM_REVAUTHOR | CFM_SPACING | CFM_KERNING | CFM_STYLE | CFM_ANIMATION),
    }

    [Flags]
    public enum CHARFORMAT_EFFECTS : uint
    {
        CFE_BOLD = 0x0001,
        CFE_ITALIC = 0x0002,
        CFE_UNDERLINE = 0x0004,
        CFE_STRIKEOUT = 0x0008,
        CFE_PROTECTED = 0x0010,
        CFE_LINK = 0x0020,
        CFE_AUTOCOLOR = 0x40000000, // NOTE: this corresponds to CFM_COLOR, which controls it

        CFE_SUBSCRIPT = 0x00010000, // Superscript and subscript are mutually exclusive
        CFE_SUPERSCRIPT = 0x00020000,

        CFE_SMALLCAPS = CHARFORMAT_MASKS.CFM_SMALLCAPS,
        CFE_ALLCAPS = CHARFORMAT_MASKS.CFM_ALLCAPS,
        CFE_HIDDEN = CHARFORMAT_MASKS.CFM_HIDDEN,
        CFE_OUTLINE = CHARFORMAT_MASKS.CFM_OUTLINE,
        CFE_SHADOW = CHARFORMAT_MASKS.CFM_SHADOW,
        CFE_EMBOSS = CHARFORMAT_MASKS.CFM_EMBOSS,
        CFE_IMPRINT = CHARFORMAT_MASKS.CFM_IMPRINT,
        CFE_DISABLED = CHARFORMAT_MASKS.CFM_DISABLED,
        CFE_REVISED = CHARFORMAT_MASKS.CFM_REVISED,

        CFE_AUTOBACKCOLOR = CHARFORMAT_MASKS.CFM_BACKCOLOR, // CFE_AUTOCOLOR and CFE_AUTOBACKCOLOR correspond to CFM_COLOR and // CFM_BACKCOLOR, respectively, which control them
    }

    public enum CHARFORMAT_UNDERLINE : byte
    {
        CFU_CF1UNDERLINE = 0xFF, // Map charformat's bit underline to CF2
        CFU_INVERT = 0xFE, // For IME composition fake a selection

        CFU_UNDERLINETHICKLONGDASH = 18,
        CFU_UNDERLINETHICKDOTTED = 17,
        CFU_UNDERLINETHICKDASHDOTDOT = 16,
        CFU_UNDERLINETHICKDASHDOT = 15,
        CFU_UNDERLINETHICKDASH = 14,
        CFU_UNDERLINELONGDASH = 13,
        CFU_UNDERLINEHEAVYWAVE = 12,
        CFU_UNDERLINEDOUBLEWAVE = 11,
        CFU_UNDERLINEHAIRLINE = 10,
        CFU_UNDERLINETHICK = 9,
        CFU_UNDERLINEWAVE = 8,
        CFU_UNDERLINEDASHDOTDOT = 7,
        CFU_UNDERLINEDASHDOT = 6,
        CFU_UNDERLINEDASH = 5,
        CFU_UNDERLINEDOTTED = 4,
        CFU_UNDERLINEDOUBLE = 3,
        CFU_UNDERLINEWORD = 2,
        CFU_UNDERLINE = 1,
        CFU_UNDERLINENONE = 0,

    }
    


}
