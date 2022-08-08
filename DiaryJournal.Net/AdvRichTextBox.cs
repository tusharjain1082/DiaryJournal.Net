using System.Runtime.InteropServices;
using System.Buffers;
using System.Text;
using System.Runtime.InteropServices;
using System.ComponentModel;
using myJournal.Net;

namespace System.Windows.Forms
{
    /// <summary>
    /// Represents a standard <see cref="RichTextBox"/> with some
    /// minor added functionality.
    /// </summary>
    /// <remarks>
    /// AdvRichTextBox provides methods to maintain performance
    /// while it is being updated. Additional formatting features
    /// have also been added.
    /// </remarks>
    public class AdvRichTextBox : RichTextBoxEx
    {

        

        /// <summary>
        /// Maintains performance while updating.
        /// </summary>
        /// <remarks>
        /// <para>
        /// It is recommended to call this method before doing
        /// any major updates that you do not wish the user to
        /// see. Remember to call EndUpdate when you are finished
        /// with the update. Nested calls are supported.
        /// </para>
        /// <para>
        /// Calling this method will prevent redrawing. It will
        /// also setup the event mask of the underlying richedit
        /// control so that no events are sent.
        /// </para>
        /// </remarks>
        public void BeginUpdate()
        {
            // Deal with nested calls.
            ++updating;
            if (updating > 1) return;
            // Prevent the control from raising any events.
            oldEventMask = SendMessage(new HandleRef(this, Handle), EM_SETEVENTMASK, 0, 0);
            // Prevent the control from redrawing itself.
            SendMessage(new HandleRef(this, Handle), WM_SETREDRAW, 0, 0);
        }

        /// <summary>
        /// Resumes drawing and event handling.
        /// </summary>
        /// <remarks>
        /// This method should be called every time a call is made
        /// made to BeginUpdate. It resets the event mask to it's
        /// original value and enables redrawing of the control.
        /// </remarks>
        public void EndUpdate()
        {
            // Deal with nested calls.
            --updating;
            if (updating > 0) return;
            // Allow the control to redraw itself.
            SendMessage(new HandleRef(this, Handle), WM_SETREDRAW, 1, 0);
            // Allow the control to raise event messages.
            SendMessage(new HandleRef(this, Handle), EM_SETEVENTMASK, 0, oldEventMask);
        }


        //********************************************************************
        //new addition below********************************************************
        [StructLayout(LayoutKind.Sequential)]
        private struct GETTEXTEX
        {
            public int iCb;
            public int iFlags;
            public int iCodepage;
            public IntPtr lpDefaultChar;
            public IntPtr lpUsedDefChar;
        }

        [StructLayoutAttribute(LayoutKind.Sequential)]
        private struct GETTEXTLENGTHEX
        {
            public uint uiFlags;
            public uint uiCodePage;
        }


        // Flags for the GETEXTEX data structure
        private const int GT_DEFAULT = 0;
        private const int GT_NOHIDDENTEXT = 8;
        private const int GT_RAWTEXT = 4;
        private const int GT_SELECTION = 2;
        private const int GT_USECRLF = 1;
        private const int WM_USER = 0x0400;

        //Flags for EM_GETTEXTLENGTHEX
        private const int GTL_DEFAULT = 0; // Do default (return # of chars)
        private const int GTL_NUMBYTES = 16; // Return number of _bytes_
        private const int GTL_NUMCHARS = 8; // Return number of characters
        private const int GTL_PRECISE = 2; // Compute a precise answer
        private const int GTL_USECRLF = 1; // Compute answer using CRLFs for paragraphs

        private const int CP_ANSI = 0; //Ansi Code Page (same as CP_ACP)
        private const int CP_UNICODE = 1200; //Unicode Code Page
        private const int EM_GETTEXTEX = (WM_USER + 94);
        private const int EM_GETTEXTLENGTHEX = (WM_USER + 95);

        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, IntPtr msg, ref GETTEXTLENGTHEX wParam, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int SendMessage(IntPtr hWnd, int Msg, ref GETTEXTEX wParam, StringBuilder lParam);

        public static int GetTextLength(RichTextBox rt)
        {
            GETTEXTLENGTHEX GTL = new GETTEXTLENGTHEX();
            GTL.uiFlags = GTL_DEFAULT;
            GTL.uiCodePage = CP_UNICODE;
            return (int)SendMessage(rt.Handle, (IntPtr)EM_GETTEXTLENGTHEX, ref GTL, IntPtr.Zero);
        }

        public static string GetText(RichTextBox rt)
        {
            int iCharLength = GetTextLength(rt) + 1;
            int iByteLength = 2 * iCharLength;

            GETTEXTEX GT = new GETTEXTEX();
            GT.iCb = iByteLength;
            GT.iFlags = GT_DEFAULT;
            GT.iCodepage = CP_UNICODE;

            StringBuilder SB = new StringBuilder(iCharLength);
            SendMessage(rt.Handle, EM_GETTEXTEX, ref GT, SB);
            string strText = SB.ToString();
            return strText;
        }
        public override int TextLength
        {
            get
            {
                return GetTextLength(this);
            }
        }

        public override string Text
        {
            get
            {
                return GetText(this);
            }
            set => base.Text = value;
        }

      
        [StructLayout(LayoutKind.Sequential)]
        public struct CHARRANGE
        {
            public int cpMin;
            public int cpMax;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct FINDTEXTEXW
        {
            public CHARRANGE chrg = default(CHARRANGE);
            public string lpstrText = "";
            public CHARRANGE chrgText = default(CHARRANGE);
        }

        public int FindUnicodeText(RichTextBox rtb, String text, int start, int len)
        {
            FINDTEXTEXW findFormat = default(FINDTEXTEXW);
            findFormat.lpstrText = text;
            findFormat.chrg.cpMin = start;
            findFormat.chrg.cpMax = start + len;
            findFormat.chrgText.cpMin = 0;
            findFormat.chrgText.cpMax = text.Length;
            return SendMessage(new HandleRef(rtb, rtb.Handle), RichTextUser.EM_FINDTEXTEXW, RichTextUser.FR_DOWN, ref findFormat);
        }

        public bool ReplaceUnicodeText(RichTextBox rtb, String text, int start, int len, String replacement)
        {
            int index = FindUnicodeText(rtb, text, start, len);
            if (index == -1)
                return false;

            if (SetSelectionEx(rtb, index, text.Length) < 0)
                return false;

            SendMessage(new HandleRef(rtb, rtb.Handle), RichTextUser.EM_REPLACESEL, 1, replacement);
            return true;
        }

        public bool SetSelection(RichTextBox rtb, int start, int len)
        {
            SendMessage(new HandleRef(rtb, rtb.Handle), RichTextUser.EM_SETSEL, start, start + len);
            return true;
        }
        public static int SetSelectionEx(RichTextBox rtb, int start, int len)
        {
            CHARRANGE cr = default(CHARRANGE);
            cr.cpMin = start;
            cr.cpMax = start + len; //999999999;
            return SendMessage(new HandleRef(rtb, rtb.Handle), RichTextUser.EM_EXSETSEL, 0, ref cr);
        }

        [DllImport("user32", CharSet = CharSet.Auto)]
        public static extern int SendMessage(HandleRef hWnd, int uMsg, int wParam, string lParam);

        [DllImport("user32", CharSet = CharSet.Auto)]
        private static extern int SendMessage(HandleRef hWnd,
                                       int msg,
                                       int wParam,
                                       ref CHARRANGE lp);

        [DllImport("user32", CharSet = CharSet.Auto)]
        private static extern int SendMessage(HandleRef hWnd,
                               int msg,
                               int wParam,
                               ref FINDTEXTEXW lp);


        //********************************************************************
        //new addition above********************************************************

        /// <summary>
        /// Gets or sets the alignment to apply to the current
        /// selection or insertion point.
        /// </summary>
        /// <remarks>
        /// Replaces the SelectionAlignment from
        /// <see cref="RichTextBox"/>.
        /// </remarks>
        public new TextAlign SelectionAlignment
        {
            get
            {
                var fmt = new PARAFORMAT();
                fmt.cbSize = Marshal.SizeOf(fmt);

                // Get the alignment.
                SendMessage(new HandleRef(this, Handle), EM_GETPARAFORMAT, SCF_SELECTION, ref fmt);

                // Default to Left align.
                return (fmt.dwMask & PFM_ALIGNMENT) == 0 ? TextAlign.Left : (TextAlign) fmt.wAlignment;
            }

            set
            {
                var fmt = new PARAFORMAT();
                fmt.cbSize = Marshal.SizeOf(fmt);
                fmt.dwMask = PFM_ALIGNMENT;
                fmt.wAlignment = (short) value;
                // Set the alignment.
                SendMessage(new HandleRef(this, Handle), EM_SETPARAFORMAT, SCF_SELECTION, ref fmt);
            }
        }

        public override int SelectionLength 
        { 
            get => base.SelectionLength;
            set => base.SelectionLength = value; 
        }

        /// <summary>
        /// This member overrides
        /// <see cref="Control"/>.OnHandleCreated.
        /// </summary>
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            // Enable support for justification.
            SendMessage(new HandleRef(this, Handle), EM_SETTYPOGRAPHYOPTIONS, TO_ADVANCEDTYPOGRAPHY, TO_ADVANCEDTYPOGRAPHY);
        }

        int updating = 0;
        int oldEventMask = 0;

        // Constants from the Platform SDK.
        const int EM_SETEVENTMASK = 1073;
        const int EM_GETPARAFORMAT = 1085;
        const int EM_SETPARAFORMAT = 1095;
        const int EM_SETTYPOGRAPHYOPTIONS = 1226;
        const int WM_SETREDRAW = 11;
        const int TO_ADVANCEDTYPOGRAPHY = 1;
        const int PFM_ALIGNMENT = 8;
        const int SCF_SELECTION = 1;

        // It makes no difference if we use PARAFORMAT or
        // PARAFORMAT2 here, so I have opted for PARAFORMAT2.
        [StructLayout(LayoutKind.Sequential)]
        private struct PARAFORMAT
        {
            public int cbSize;
            public uint dwMask;
            public short wNumbering;
            public short wReserved;
            public int dxStartIndent;
            public int dxRightIndent;
            public int dxOffset;
            public short wAlignment;
            public short cTabCount;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public int[] rgxTabs;

            // PARAFORMAT2 from here onwards.
            public int dySpaceBefore;
            public int dySpaceAfter;
            public int dyLineSpacing;
            public short sStyle;
            public byte bLineSpacingRule;
            public byte bOutlineLevel;
            public short wShadingWeight;
            public short wShadingStyle;
            public short wNumberingStart;
            public short wNumberingStyle;
            public short wNumberingTab;
            public short wBorderSpace;
            public short wBorderWidth;
            public short wBorders;
        }

        [DllImport("user32", CharSet = CharSet.Auto)]
        private static extern int SendMessage(HandleRef hWnd,
                                               int msg,
                                               int wParam,
                                               int lParam);

        [DllImport("user32", CharSet = CharSet.Auto)]
        private static extern int SendMessage(HandleRef hWnd,
                                               int msg,
                                               int wParam,
                                               ref PARAFORMAT lp);
    }

    /// <summary>
    /// Specifies how text in a <see cref="AdvRichTextBox"/> is
    /// horizontally aligned.
    /// </summary>
    public enum TextAlign
    {
        /// <summary>
        /// The text is aligned to the left.
        /// </summary>
        Left = 1,

        /// <summary>
        /// The text is aligned to the right.
        /// </summary>
        Right = 2,

        /// <summary>
        /// The text is aligned in the center.
        /// </summary>
        Center = 3,

        /// <summary>
        /// The text is justified.
        /// </summary>
        Justify = 4
    }
}
