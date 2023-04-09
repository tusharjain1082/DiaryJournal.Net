using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Drawing.Printing;

namespace DiaryJournal.Net
{
    /// <summary>
    /// An extension for RichTextBox suitable for printing.
    /// </summary>
    public class PrintRichTextBoxEx : RichTextBox
    {
        public PrintDocument? printDoc = null;

        // C#
        [StructLayout(LayoutKind.Sequential)]
        private struct STRUCT_RECT
        {
            public Int32 left;
            public Int32 top;
            public Int32 right;
            public Int32 bottom;
        }

        // C#
        [StructLayout(LayoutKind.Sequential)]
        private struct STRUCT_CHARRANGE
        {
            public Int32 cpMin;
            public Int32 cpMax;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct STRUCT_FORMATRANGE
        {
            public IntPtr hdc;
            public IntPtr hdcTarget;
            public STRUCT_RECT rc;
            public STRUCT_RECT rcPage;
            public STRUCT_CHARRANGE chrg;
        }

        // C#
        [DllImport("user32.dll")]
        private static extern Int32 SendMessage(IntPtr hWnd, Int32 msg,
                                                Int32 wParam, IntPtr lParam);

        private const Int32 WM_USER = 0x400;
        private const Int32 EM_FORMATRANGE = WM_USER + 57;

        // C#
        [StructLayout(LayoutKind.Sequential)]
        private struct STRUCT_CHARFORMAT
        {
            public int cbSize;
            public UInt32 dwMask;
            public UInt32 dwEffects;
            public Int32 yHeight;
            public Int32 yOffset;
            public Int32 crTextColor;
            public byte bCharSet;
            public byte bPitchAndFamily;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public char[] szFaceName;
        }

        // C#
        /// <summary>
        /// Calculate or render the contents of our RichTextBox for printing
        /// </summary>
        /// <param name="measureOnly">If true, only the calculation is performed,
        /// otherwise the text is rendered as well</param>
        /// <param name="e">The PrintPageEventArgs object from the
        /// PrintPage event</param>
        /// <param name="charFrom">Index of first character to be printed</param>
        /// <param name="charTo">Index of last character to be printed</param>
        /// <returns>(Index of last character that fitted on the
        /// page) + 1</returns>
        public int FormatRange(bool measureOnly, PrintPageEventArgs e,
                               int charFrom, int charTo)
        {
            // Specify which characters to print
            STRUCT_CHARRANGE cr;
            cr.cpMin = charFrom;
            cr.cpMax = charTo;

            // Specify the area inside page margins
            STRUCT_RECT rc;
            rc.top = HundredthInchToTwips(e.MarginBounds.Top);
            rc.bottom = HundredthInchToTwips(e.MarginBounds.Bottom);
            rc.left = HundredthInchToTwips(e.MarginBounds.Left);
            rc.right = HundredthInchToTwips(e.MarginBounds.Right);

            // Specify the page area
            STRUCT_RECT rcPage;
            rcPage.top = HundredthInchToTwips(e.PageBounds.Top);
            rcPage.bottom = HundredthInchToTwips(e.PageBounds.Bottom);
            rcPage.left = HundredthInchToTwips(e.PageBounds.Left);
            rcPage.right = HundredthInchToTwips(e.PageBounds.Right);

            // Get device context of output device
            IntPtr hdc = e.Graphics.GetHdc();

            // Fill in the FORMATRANGE struct
            STRUCT_FORMATRANGE fr;
            fr.chrg = cr;
            fr.hdc = hdc;
            fr.hdcTarget = hdc;
            fr.rc = rc;
            fr.rcPage = rcPage;

            // Non-Zero wParam means render, Zero means measure
            Int32 wParam = (measureOnly ? 0 : 1);

            // Allocate memory for the FORMATRANGE struct and
            // copy the contents of our struct to this memory
            IntPtr lParam = Marshal.AllocCoTaskMem(Marshal.SizeOf(fr));
            Marshal.StructureToPtr(fr, lParam, false);

            // Send the actual Win32 message
            int res = SendMessage(Handle, EM_FORMATRANGE, wParam, lParam);

            // Free allocated memory
            Marshal.FreeCoTaskMem(lParam);

            // and release the device context
            e.Graphics.ReleaseHdc(hdc);

            return res;
        }

        // C#
        /// <summary>
        /// Convert between 1/100 inch (unit used by the .NET framework)
        /// and twips (1/1440 inch, used by Win32 API calls)
        /// </summary>
        /// <param name="n">Value in 1/100 inch</param>
        /// <returns>Value in twips</returns>
        private Int32 HundredthInchToTwips(int n)
        {
            return (Int32)(n * 14.4);
        }

        // C#
        /// <summary>
        /// Free cached data from rich edit control after printing
        /// </summary>
        public void FormatRangeDone()
        {
            IntPtr lParam = new IntPtr(0);
            SendMessage(Handle, EM_FORMATRANGE, 0, lParam);

            // completed
            //this.Dispose();
        }

        // C#
        public void PrintRichTextContents()
        {
            // Start printing process
            printDoc.Print();
        }

        // C#
        // variable to trace text to print for pagination
        private int m_nFirstCharOnPage;

        public void printDoc_BeginPrint(object sender,
            System.Drawing.Printing.PrintEventArgs e)
        {
            // Start at the beginning of the text
            m_nFirstCharOnPage = 0;
        }

        public void printDoc_PrintPage(object sender,
            System.Drawing.Printing.PrintPageEventArgs e)
        {
            // To print the boundaries of the current page margins
            // uncomment the next line:
            // e.Graphics.DrawRectangle(System.Drawing.Pens.Blue, e.MarginBounds);

            // make the RichTextBoxEx calculate and render as much text as will
            // fit on the page and remember the last character printed for the
            // beginning of the next page
            m_nFirstCharOnPage = this.FormatRange(false,
                                                    e,
                                                    m_nFirstCharOnPage,
                                                    this.TextLength);

            // check if there are more pages to print
            if (m_nFirstCharOnPage < this.TextLength)
                e.HasMorePages = true;
            else
                e.HasMorePages = false;
        }

        public void printDoc_EndPrint(object sender,
            System.Drawing.Printing.PrintEventArgs e)
        {
            // Clean up cached information
            this.FormatRangeDone();

        }

    }
}
