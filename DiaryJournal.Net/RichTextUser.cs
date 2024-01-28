using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiaryJournal.Net
{
    public static class RichTextUser
    {
        public const int WM_USER = 1024;

        /* flags for the find text options */
        internal const int FR_DOWN = 0x00000001;
        internal const int FR_WHOLEWORD = 0x00000002;
        internal const int FR_MATCHCASE = 0x00000004;

        // RichEdit messages
        public const int WM_CONTEXTMENU = 0x007B;
        public const int WM_UNICHAR = 0x0109;
        public const int WM_PRINTCLIENT = 0x0318;

        public const int EM_NONE = 0;

        public const int EM_GETLIMITTEXT = (WM_USER + 37);
        public const int EM_POSFROMCHAR = (WM_USER + 38);
        public const int EM_CHARFROMPOS = (WM_USER + 39);
        public const int EM_SCROLLCARET = (WM_USER + 49);
        public const int EM_CANPASTE = (WM_USER + 50);
        public const int EM_DISPLAYBAND = (WM_USER + 51);
        public const int EM_EXGETSEL = (WM_USER + 52);
        public const int EM_EXLIMITTEXT = (WM_USER + 53);
        public const int EM_EXLINEFROMCHAR = (WM_USER + 54);
        public const int EM_EXSETSEL = (WM_USER + 55);
        public const int EM_FINDTEXT = (WM_USER + 56);
        public const int EM_FINDTEXTEX = (WM_USER + 79);
        public const int EM_FINDTEXTEXW = (WM_USER + 124);
        public const int EM_FORMATRANGE = (WM_USER + 57);
        public const int EM_GETCHARFORMAT = (WM_USER + 58);
        public const int EM_GETEVENTMASK = (WM_USER + 59);
        public const int EM_GETOLEINTERFACE = (WM_USER + 60);
        public const int EM_GETPARAFORMAT = (WM_USER + 61);
        public const int EM_GETSELTEXT = (WM_USER + 62);
        public const int EM_HIDESELECTION = (WM_USER + 63);
        public const int EM_PASTESPECIAL = (WM_USER + 64);
        public const int EM_REQUESTRESIZ = (WM_USER + 65);
        public const int EM_SELECTIONTYP = (WM_USER + 66);
        public const int EM_SETBKGNDCOLO = (WM_USER + 67);
        public const int EM_SETCHARFORMAT = (WM_USER + 68);
        public const int EM_SETEVENTMASK = (WM_USER + 69);
        public const int EM_SETOLECALLBACK = (WM_USER + 70);
        public const int EM_SETPARAFORMAT = (WM_USER + 71);
        public const int EM_SETTARGETDEVICE = (WM_USER + 72);
        public const int EM_STREAMIN = (WM_USER + 73);
        public const int EM_STREAMOUT = (WM_USER + 74);
        public const int EM_GETTEXTRANGE = (WM_USER + 75);
        public const int EM_FINDWORDBREAK = (WM_USER + 76);
        public const int EM_SETOPTIONS = (WM_USER + 77);
        public const int EM_GETOPTIONS = (WM_USER + 78);
        //public const int EM_FINDTEXTEX = (WM_USER + 79);
        public const int EM_GETWORDBREAKPROC = (WM_USER + 80);
        public const int EM_SETWORDBREAKPROC = (WM_USER + 81);

        // RichEdit 2.0 messages 
        public const int EM_SETUNDOLIMIT = (WM_USER + 82);
        public const int EM_REDO = (WM_USER + 84);
        public const int EM_CANREDO = (WM_USER + 85);
        public const int EM_GETUNDONAME = (WM_USER + 86);
        public const int EM_GETREDONAME = (WM_USER + 87);
        public const int EM_STOPGROUPTYPIN = (WM_USER + 88);
        public const int EM_SETTEXTMODE = (WM_USER + 89);
        public const int EM_GETTEXTMODE = (WM_USER + 90);

        public const int EM_AUTOURLDETECT = (WM_USER + 91);
        public const int EM_GETAUTOURLDETECT = (WM_USER + 92);
        public const int EM_SETPALETTE = (WM_USER + 93);
        public const int EM_GETTEXTEX = (WM_USER + 94);
        public const int EM_GETTEXTLENGTHEX = (WM_USER + 95);
        public const int EM_SHOWSCROLLBAR = (WM_USER + 96);
        public const int EM_SETTEXTEX = (WM_USER + 97);

        // East Asia specific messages 
        public const int EM_SETPUNCTUATION = (WM_USER + 100);
        public const int EM_GETPUNCTUATION = (WM_USER + 101);
        public const int EM_SETWORDWRAPMODE = (WM_USER + 102);
        public const int EM_GETWORDWRAPMODE = (WM_USER + 103);
        public const int EM_SETIMECOLOR = (WM_USER + 104);
        public const int EM_GETIMECOLOR = (WM_USER + 105);
        public const int EM_SETIMEOPTION = (WM_USER + 106);
        public const int EM_GETIMEOPTION = (WM_USER + 107);
        public const int EM_CONVPOSITION = (WM_USER + 108);

        public const int EM_SETLANGOPTION = (WM_USER + 120);
        public const int EM_GETLANGOPTION = (WM_USER + 121);
        public const int EM_GETIMECOMPMODE = (WM_USER + 122);

        public const int EM_FINDTEXTW = (WM_USER + 123);
        //public const int EM_FINDTEXTEXW = (WM_USER + 124);

        // RE3.0 FE messages 
        public const int EM_RECONVERSION = (WM_USER + 125);
        public const int EM_SETIMEMODEBIAS = (WM_USER + 126);
        public const int EM_GETIMEMODEBIAS = (WM_USER + 127);

        // BiDi specific messages 
        public const int EM_SETBIDIOPTION = (WM_USER + 200);
        public const int EM_GETBIDIOPTION = (WM_USER + 201);

        public const int EM_SETTYPOGRAPHYOPTIO = (WM_USER + 202);
        public const int EM_GETTYPOGRAPHYOPTIO = (WM_USER + 203);

        // Extended edit style specific messages 
        public const int EM_SETEDITSTYLE = (WM_USER + 204);
        public const int EM_GETEDITSTYLE = (WM_USER + 205);

        // Extended edit style masks 
        public const int SES_EMULATESYSEDI = 1;
        public const int SES_BEEPONMAXTEX = 2;
        public const int SES_EXTENDBACKCOLO = 4;
        public const int SES_MAPCPS = 8;
        public const int SES_EMULATE10 = 16;
        public const int SES_USECRLF = 32;
        public const int SES_USEAIMM = 64;
        public const int SES_NOIME = 128;

        public const int SES_ALLOWBEEPS = 256;
        public const int SES_UPPERCASE = 512;
        public const int SES_LOWERCASE = 1024;
        public const int SES_NOINPUTSEQUENCEC = 2048;
        public const int SES_BIDI = 4096;
        public const int SES_SCROLLONKILLFOC = 8192;
        public const int SES_XLTCRCRLFTOC = 16384;
        public const int SES_DRAFTMODE = 32768;

        public const int SES_USECTF = 0x0010000;
        public const int SES_HIDEGRIDLINE = 0x0020000;
        public const int SES_USEATFONT = 0x0040000;
        public const int SES_CUSTOMLOOK = 0x0080000;
        public const int SES_LBSCROLLNOTIF = 0x0100000;
        public const int SES_CTFALLOWEMBE = 0x0200000;
        public const int SES_CTFALLOWSMARTT = 0x0400000;
        public const int SES_CTFALLOWPROOFI = 0x0800000;

        // Options for EM_SETLANGOPTIONS and EM_GETLANGOPTIONS 
        public const int IMF_AUTOKEYBOAR = 0x0001;
        public const int IMF_AUTOFONT = 0x0002;
        public const int IMF_IMECANCELCOMPLE = 0x0004;  // High completes comp string when aborting, low cancel
        public const int IMF_IMEALWAYSSENDNOTI = 0x0008;
        public const int IMF_AUTOFONTSIZEADJU = 0x0010;
        public const int IMF_UIFONTS = 0x0020;
        public const int IMF_DUALFONT = 0x0080;

        // Values for EM_GETIMECOMPMODE 
        public const int ICM_NOTOPEN = 0x0000;
        public const int ICM_LEVEL3 = 0x0001;
        public const int ICM_LEVEL2 = 0x0002;
        public const int ICM_LEVEL2_5 = 0x0003;
        public const int ICM_LEVEL2_SUI = 0x0004;
        public const int ICM_CTF = 0x0005;

        // Options for EM_SETTYPOGRAPHYOPTIONS 
        public const int TO_ADVANCEDTYPOGRAP = 1;
        public const int TO_SIMPLELINEBREA = 2;
        public const int TO_DISABLECUSTOMTEXTO = 4;
        public const int TO_ADVANCEDLAYOU = 8;

        // Pegasus outline mode messages (RE 3.0) 

        // Outline mode message
        public const int EM_OUTLINE = (WM_USER + 220);
        // Message for getting and restoring scroll pos
        public const int EM_GETSCROLLPOS = (WM_USER + 221);
        public const int EM_SETSCROLLPOS = (WM_USER + 222);
        // Change fontsize in current selection by wParam
        public const int EM_SETFONTSIZE = (WM_USER + 223);
        public const int EM_GETZOOM = (WM_USER + 224);
        public const int EM_SETZOOM = (WM_USER + 225);
        public const int EM_GETVIEWKIND = (WM_USER + 226);
        public const int EM_SETVIEWKIND = (WM_USER + 227);

        // RichEdit 4.0 messages
        public const int EM_GETPAGE = (WM_USER + 228);
        public const int EM_SETPAGE = (WM_USER + 229);
        public const int EM_GETHYPHENATEINF = (WM_USER + 230);
        public const int EM_SETHYPHENATEINF = (WM_USER + 231);
        public const int EM_GETPAGEROTAT = (WM_USER + 235);
        public const int EM_SETPAGEROTAT = (WM_USER + 236);
        public const int EM_GETCTFMODEBIA = (WM_USER + 237);
        public const int EM_SETCTFMODEBIA = (WM_USER + 238);
        public const int EM_GETCTFOPENSTATU = (WM_USER + 240);
        public const int EM_SETCTFOPENSTATU = (WM_USER + 241);
        public const int EM_GETIMECOMPTEX = (WM_USER + 242);
        public const int EM_ISIME = (WM_USER + 243);
        public const int EM_GETIMEPROPERT = (WM_USER + 244);

        // EM_SETPAGEROTATE wparam values
        public const int EPR_0 = 0;     // Text flows left to right and top to bottom
        public const int EPR_270 = 1;   // Text flows top to bottom and right to left
        public const int EPR_180 = 2;       // Text flows right to left and bottom to top
        public const int EPR_90 = 3;        // Text flows bottom to top and left to right

        // EM_SETCTFMODEBIAS wparam values
        public const int CTFMODEBIAS_DEFAULT = 0x0000;
        public const int CTFMODEBIAS_FILENAME = 0x0001;
        public const int CTFMODEBIAS_NAME = 0x0002;
        public const int CTFMODEBIAS_READING = 0x0003;
        public const int CTFMODEBIAS_DATETIME = 0x0004;
        public const int CTFMODEBIAS_CONVERSATION = 0x0005;
        public const int CTFMODEBIAS_NUMERIC = 0x0006;
        public const int CTFMODEBIAS_HIRAGANA = 0x0007;
        public const int CTFMODEBIAS_KATAKANA = 0x0008;
        public const int CTFMODEBIAS_HANGUL = 0x0009;
        public const int CTFMODEBIAS_HALFWIDTHKATAKAN = 0x000A;
        public const int CTFMODEBIAS_FULLWIDTHALPHANUMER = 0x000B;
        public const int CTFMODEBIAS_HALFWIDTHALPHANUMER = 0x000C;

        // EM_SETIMEMODEBIAS lparam values
        public const int IMF_SMODE_PLAURALCLAU = 0x0001;
        public const int IMF_SMODE_NONE = 0x0002;

        public const int ICT_RESULTREADST = 1;

        // Outline mode wparam values
        public const int EMO_EXIT = 0;       // Enter normal mode,  lparam ignore
        public const int EMO_ENTER = 1;       // Enter outline mode, lparam ignore
        public const int EMO_PROMOTE = 2;       // LOWORD(lparam) == 0 ==;
                                                //  promote  to body-text
                                                // LOWORD(lparam) != 0 ==>
                                                //  promote/demote current selection
                                                //  by indicated number of levels
        public const int EMO_EXPAND = 3;       // HIWORD(lparam) = EMO_EXPANDSELECTION
                                               //  -> expands selection to level
                                               //  indicated in LOWORD(lparam)
                                               //  LOWORD(lparam) = -1/+1 corresponds
                                               //  to collapse/expand button presses
                                               //  in winword (other values are
                                               //  equivalent to having pressed these
                                               //  buttons more than once)
                                               //  HIWORD(lparam) = EMO_EXPANDDOCUMENT
                                               //  -> expands whole document to
                                               //  indicated level
        public const int EMO_MOVESELECTION = 4;      // LOWORD(lparam) != 0 -> move current
                                                     //  selection up/down by indicated amount
        public const int EMO_GETVIEWMODE = 5;       // Returns VM_NORMAL or VM_OUTLINE

        // EMO_EXPAND options
        public const int EMO_EXPANDSELECTION = 0;
        public const int EMO_EXPANDDOCUMENT = 1;

        public const int VM_NORMAL = 4;     // Agrees with RTF \viewkindN
        public const int VM_OUTLINE = 2;
        public const int VM_PAGE = 9;       // Screen page view (not print layout)

        // New notifications 
        public const int EN_MSGFILTER = 0x0700;
        public const int EN_REQUESTRESIZ = 0x0701;
        public const int EN_SELCHANGE = 0x0702;
        public const int EN_DROPFILES = 0x0703;
        public const int EN_PROTECTED = 0x0704;
        public const int EN_CORRECTTEXT = 0x0705;           // PenWin specific 
        public const int EN_STOPNOUNDO = 0x0706;
        public const int EN_IMECHANGE = 0x0707;         // East Asia specific 
        public const int EN_SAVECLIPBOAR = 0x0708;
        public const int EN_OLEOPFAILED = 0x0709;
        public const int EN_OBJECTPOSITION = 0x070a;
        public const int EN_LINK = 0x070b;
        public const int EN_DRAGDROPDONE = 0x070c;
        public const int EN_PARAGRAPHEXPAND = 0x070d;
        public const int EN_PAGECHANGE = 0x070e;
        public const int EN_LOWFIRTF = 0x070f;
        public const int EN_ALIGNLTR = 0x0710;          // BiDi specific notification
        public const int EN_ALIGNRTL = 0x0711;          // BiDi specific notification

        // Event notification masks 
        public const int ENM_NONE = 0x00000000;
        public const int ENM_CHANGE = 0x00000001;
        public const int ENM_UPDATE = 0x00000002;
        public const int ENM_SCROLL = 0x00000004;
        public const int ENM_SCROLLEVENT = 0x00000008;
        public const int ENM_DRAGDROPDON = 0x00000010;
        public const int ENM_PARAGRAPHEXPAND = 0x00000020;
        public const int ENM_PAGECHANGE = 0x00000040;
        public const int ENM_KEYEVENTS = 0x00010000;
        public const int ENM_MOUSEEVENTS = 0x00020000;
        public const int ENM_REQUESTRESIZE = 0x00040000;
        public const int ENM_SELCHANGE = 0x00080000;
        public const int ENM_DROPFILES = 0x00100000;
        public const int ENM_PROTECTED = 0x00200000;
        public const int ENM_CORRECTTEXT = 0x00400000;      // PenWin specific 
        public const int ENM_IMECHANGE = 0x00800000;        // Used by RE1.0 compatibility
        public const int ENM_LANGCHANGE = 0x01000000;
        public const int ENM_OBJECTPOSITION = 0x02000000;
        public const int ENM_LINK = 0x04000000;
        public const int ENM_LOWFIRTF = 0x08000000;


        // New edit control styles 
        public const int ES_SAVESEL = 0x00008000;
        public const int ES_SUNKEN = 0x00004000;
        public const int ES_DISABLENOSCROL = 0x00002000;
        // Same as WS_MAXIMIZE, but that doesn't make sense so we re-use the value 
        public const int ES_SELECTIONBAR = 0x01000000;
        // Same as ES_UPPERCASE, but re-used to completely disable OLE drag'n'drop 
        public const int ES_NOOLEDRAGDRO = 0x00000008;

        // New edit control extended style 
        public const int ES_EX_NOCALLOLEINI = 0x01000000;

        // These flags are used in FE Windows 
        public const int ES_VERTICAL = 0x00400000;      // Not supported in RE 2.0/3.0 
        public const int ES_NOIME = 0x00080000;
        public const int ES_SELFIME = 0x00040000;

        // Edit control options 
        public const int ECO_AUTOWORDSELECTI = 0x00000001;
        public const int ECO_AUTOVSCROLL = 0x00000040;
        public const int ECO_AUTOHSCROLL = 0x00000080;
        public const int ECO_NOHIDESEL = 0x00000100;
        public const int ECO_READONLY = 0x00000800;
        public const int ECO_WANTRETURN = 0x00001000;
        public const int ECO_SAVESEL = 0x00008000;
        public const int ECO_SELECTIONBA = 0x01000000;
        public const int ECO_VERTICAL = 0x00400000;     // FE specific 


        // ECO operations 
        public const int ECOOP_SET = 0x0001;
        public const int ECOOP_OR = 0x0002;
        public const int ECOOP_AND = 0x0003;
        public const int ECOOP_XOR = 0x0004;

        // New word break function actions 
        public const int WB_CLASSIFY = 3;
        public const int WB_MOVEWORDLEFT = 4;
        public const int WB_MOVEWORDRIGHT = 5;
        public const int WB_LEFTBREAK = 6;
        public const int WB_RIGHTBREAK = 7;

        // East Asia specific flags 
        public const int WB_MOVEWORDPREV = 4;
        public const int WB_MOVEWORDNEXT = 5;
        public const int WB_PREVBREAK = 6;
        public const int WB_NEXTBREAK = 7;

        public const int PC_FOLLOWING = 1;
        public const int PC_LEADING = 2;
        public const int PC_OVERFLOW = 3;
        public const int PC_DELIMITET = 4;
        public const int WBF_WORDWRAP = 0x010;
        public const int WBF_WORDBREAK = 0x020;
        public const int WBF_OVERFLOW = 0x040;
        public const int WBF_LEVEL1 = 0x080;
        public const int WBF_LEVEL2 = 0x100;
        public const int WBF_CUSTOM = 0x200;

        // East Asia specific flags 
        public const int IMF_FORCENONE = 0x0001;
        public const int IMF_FORCEENABLE = 0x0002;
        public const int IMF_FORCEDISABLE = 0x0004;
        public const int IMF_CLOSESTATUSWINDOW = 0x0008;
        public const int IMF_VERTICAL = 0x0020;
        public const int IMF_FORCEACTIVE = 0x0040;
        public const int IMF_FORCEINACTIVE = 0x0080;
        public const int IMF_FORCEREMEMBER = 0x0100;
        public const int IMF_MULTIPLEEDIT = 0x0400;

        // Word break flags (used with WB_CLASSIFY) 
        public const int WBF_CLASS = 0x0F;
        public const int WBF_ISWHITE = 0x10;
        public const int WBF_BREAKLINE = 0x20;
        public const int WBF_BREAKAFTER = 0x4;

        // EM_SETCHARFORMAT wParam masks 
        public const int SCF_SELECTION = 0x0001;
        public const int SCF_WORD = 0x0002;
        public const int SCF_DEFAULT = 0x0000;  // Set default charformat or paraformat
        public const int SCF_ALL = 0x0004;  // Not valid with SCF_SELECTION or SCF_WORD
        public const int SCF_USEUIRULE = 0x0008;    // Modifier for SCF_SELECTION; says that
                                                    //  format came from a toolbar, etc., and
                                                    //  hence UI formatting rules should be
                                                    //  used instead of literal formatting
        public const int SCF_ASSOCIATEFO = 0x0010; // Associate fontname with bCharSet (one
                                                   //  possible for each of Western, ME, FE,
                                                   //  Thai)
        public const int SCF_NOKBUPDAT = 0x0020;    // Do not update KB layput for this change
                                                    //  even if autokeyboard is on
        public const int SCF_ASSOCIATEFON = 0x0040;   // Associate plane-2 (surrogate) font

        // CHARFORMAT masks 
        public const int CFM_BOLD = 0x00000001;
        public const int CFM_ITALIC = 0x00000002;
        public const int CFM_UNDERLINE = 0x00000004;
        public const int CFM_STRIKEOUT = 0x00000008;
        public const int CFM_PROTECTED = 0x00000010;
        public const int CFM_LINK = 0x00000020; // Exchange hyperlink extension 
        public const uint CFM_SIZE = 0x80000000;
        public const int CFM_COLOR = 0x40000000;
        public const int CFM_FACE = 0x20000000;
        public const int CFM_OFFSET = 0x10000000;
        public const int CFM_CHARSET = 0x08000000;

        // CHARFORMAT effects 
        public const int CFE_BOLD = 0x0001;
        public const int CFE_ITALIC = 0x0002;
        public const int CFE_UNDERLINE = 0x0004;
        public const int CFE_STRIKEOUT = 0x0008;
        public const int CFE_PROTECTED = 0x0010;
        public const int CFE_LINK = 0x0020;
        public const int CFE_AUTOCOLOR = 0x40000000; // NOTE: this corresponds to 
                                                     // CFM_COLOR, which controls it 
                                                     // Masks and effects defined for CHARFORMAT2 -- an (*) indicates
                                                     // that the data is stored by RichEdit 2.0/3.0, but not displayed
        public const int CFM_SMALLCAPS = 0x0040; // (*)  
        public const int CFM_ALLCAPS = 0x0080; // Displayed by 3.0 
        public const int CFM_HIDDEN = 0x0100; // Hidden by 3.0 
        public const int CFM_OUTLINE = 0x0200; // (*)  
        public const int CFM_SHADOW = 0x0400; // (*)  
        public const int CFM_EMBOSS = 0x0800; // (*)  
        public const int CFM_IMPRINT = 0x1000; // (*)  
        public const int CFM_DISABLED = 0x2000;
        public const int CFM_REVISED = 0x4000;

        public const int CFM_BACKCOLOR = 0x04000000;
        public const int CFM_LCID = 0x02000000;
        public const int CFM_UNDERLINETYPE = 0x00800000; // Many displayed by 3.0 
        public const int CFM_WEIGHT = 0x00400000;
        public const int CFM_SPACING = 0x00200000; // Displayed by 3.0 
        public const int CFM_KERNING = 0x00100000; // (*)  
        public const int CFM_STYLE = 0x00080000; // (*)  
        public const int CFM_ANIMATION = 0x00040000; // (*)  
        public const int CFM_REVAUTHOR = 0x00008000;

        public const int CFE_SUBSCRIPT = 0x00010000; // Superscript and subscript are 
        public const int CFE_SUPERSCRIPT = 0x00020000; //  mutually exclusive       

        public const int CFM_SUBSCRIPT = CFE_SUBSCRIPT | CFE_SUPERSCRIPT;
        public const int CFM_SUPERSCRIPT = CFM_SUBSCRIPT;

        // CHARFORMAT "ALL" masks
        public const int CFM_EFFECTS = (CFM_BOLD | CFM_ITALIC | CFM_UNDERLINE | CFM_COLOR |
            CFM_STRIKEOUT | CFE_PROTECTED | CFM_LINK);
        public const uint CFM_ALL = (CFM_BOLD | CFM_ITALIC | CFM_UNDERLINE | CFM_COLOR |
          CFM_STRIKEOUT | CFE_PROTECTED | CFM_LINK | CFM_SIZE | CFM_FACE | CFM_OFFSET | CFM_CHARSET);

        public const int CFM_EFFECTS2 = (CFM_EFFECTS | CFM_DISABLED | CFM_SMALLCAPS | CFM_ALLCAPS
            | CFM_HIDDEN | CFM_OUTLINE | CFM_SHADOW | CFM_EMBOSS
            | CFM_IMPRINT | CFM_DISABLED | CFM_REVISED
            | CFM_SUBSCRIPT | CFM_SUPERSCRIPT | CFM_BACKCOLOR);

        public const uint CFM_ALL2 = (CFM_BOLD | CFM_ITALIC | CFM_UNDERLINE | CFM_COLOR |
          CFM_STRIKEOUT | CFE_PROTECTED | CFM_LINK | CFM_SIZE | CFM_FACE | CFM_OFFSET | CFM_CHARSET |
          CFM_EFFECTS2 | CFM_BACKCOLOR | CFM_LCID
            | CFM_UNDERLINETYPE | CFM_WEIGHT | CFM_REVAUTHOR
            | CFM_SPACING | CFM_KERNING | CFM_STYLE | CFM_ANIMATION);

        public const int CFE_SMALLCAPS = CFM_SMALLCAPS;
        public const int CFE_ALLCAPS = CFM_ALLCAPS;
        public const int CFE_HIDDEN = CFM_HIDDEN;
        public const int CFE_OUTLINE = CFM_OUTLINE;
        public const int CFE_SHADOW = CFM_SHADOW;
        public const int CFE_EMBOSS = CFM_EMBOSS;
        public const int CFE_IMPRINT = CFM_IMPRINT;
        public const int CFE_DISABLED = CFM_DISABLED;
        public const int CFE_REVISED = CFM_REVISED;

        // CFE_AUTOCOLOR and CFE_AUTOBACKCOLOR correspond to CFM_COLOR and
        // CFM_BACKCOLOR, respectively, which control them
        public const int CFE_AUTOBACKCOLOR = CFM_BACKCOLOR;


        // Underline types. RE 1.0 displays only CFU_UNDERLINE
        public const int CFU_CF1UNDERLINE = 0xFF;   // Map charformat's bit underline to CF2
        public const int CFU_INVERT = 0xFE;   // For IME composition fake a selection
        public const int CFU_UNDERLINETHICKLONGDASH = 18;   // (*) display as dash
        public const int CFU_UNDERLINETHICKDOTTED = 17;   // (*) display as dot
        public const int CFU_UNDERLINETHICKDASHDOTDOT = 16;   // (*) display as dash dot dot
        public const int CFU_UNDERLINETHICKDASHDOT = 15;   // (*) display as dash dot
        public const int CFU_UNDERLINETHICKDASH = 14;   // (*) display as dash
        public const int CFU_UNDERLINELONGDASH = 13;   // (*) display as dash
        public const int CFU_UNDERLINEHEAVYWAVE = 12;   // (*) display as wave
        public const int CFU_UNDERLINEDOUBLEWAVE = 11;   // (*) display as wave
        public const int CFU_UNDERLINEHAIRLINE = 10;   // (*) display as single  
        public const int CFU_UNDERLINETHICK = 9;
        public const int CFU_UNDERLINEWAVE = 8;
        public const int CFU_UNDERLINEDASHDOTDOT = 7;
        public const int CFU_UNDERLINEDASHDOT = 6;
        public const int CFU_UNDERLINEDASH = 5;
        public const int CFU_UNDERLINEDOTTED = 4;
        public const int CFU_UNDERLINEDOUBLE = 3;  // (*) display as single
        public const int CFU_UNDERLINEWORD = 2;  // (*) display as single  
        public const int CFU_UNDERLINE = 1;
        public const int CFU_UNDERLINENONE = 0;

        // Flags for the GETEXTEX data structure 
        public const int GT_DEFAULT = 0;
        public const int GT_USECRLF = 1;
        public const int GT_SELECTION = 2;
        public const int GT_RAWTEXT = 4;
        public const int GT_NOHIDDENTEXT = 8;

        // Flags for the GETTEXTLENGTHEX data structure							
        public const int GTL_DEFAULT = 0;   // Do default (return # of chars)		
        public const int GTL_USECRLF = 1;   // Compute answer using CRLFs for paragraphs
        public const int GTL_PRECISE = 2;   // Compute a precise answer					
        public const int GTL_CLOSE = 4; // Fast computation of a "close" answer		
        public const int GTL_NUMCHARS = 8;  // Return number of characters			
        public const int GTL_NUMBYTES = 16; // Return number of _bytes_				


        // PARAFORMAT mask values 
        public const int PFM_STARTINDENT = 0x00000001;
        public const int PFM_RIGHTINDENT = 0x00000002;
        public const int PFM_OFFSET = 0x00000004;
        public const int PFM_ALIGNMENT = 0x00000008;
        public const int PFM_TABSTOPS = 0x00000010;
        public const int PFM_NUMBERING = 0x00000020;
        public const uint PFM_OFFSETINDENT = 0x80000000;

        // PARAFORMAT 2.0 masks and effects 
        public const int PFM_SPACEBEFORE = 0x00000040;
        public const int PFM_SPACEAFTER = 0x00000080;
        public const int PFM_LINESPACING = 0x00000100;
        public const int PFM_STYLE = 0x00000400;
        public const int PFM_BORDER = 0x00000800;   // (*)	
        public const int PFM_SHADING = 0x00001000;  // (*)	
        public const int PFM_NUMBERINGSTYLE = 0x00002000;   // RE 3.0	
        public const int PFM_NUMBERINGTAB = 0x00004000; // RE 3.0	
        public const int PFM_NUMBERINGSTART = 0x00008000;   // RE 3.0	

        public const int PFM_RTLPARA = 0x00010000;
        public const int PFM_KEEP = 0x00020000; // (*)	
        public const int PFM_KEEPNEXT = 0x00040000; // (*)	
        public const int PFM_PAGEBREAKBEFORE = 0x00080000;  // (*)	
        public const int PFM_NOLINENUMBER = 0x00100000; // (*)	
        public const int PFM_NOWIDOWCONTROL = 0x00200000;   // (*)	
        public const int PFM_DONOTHYPHEN = 0x00400000;  // (*)	
        public const int PFM_SIDEBYSIDE = 0x00800000;   // (*)	
        public const int PFM_TABLE = 0x40000000;    // RE 3.0 
        public const int PFM_TEXTWRAPPINGBREAK = 0x20000000;    // RE 3.0 
        public const int PFM_TABLEROWDELIMITER = 0x10000000;    // RE 4.0 

        // The following three properties are read only
        public const int PFM_COLLAPSED = 0x01000000;    // RE 3.0 
        public const int PFM_OUTLINELEVEL = 0x02000000; // RE 3.0 
        public const int PFM_BOX = 0x04000000;  // RE 3.0 
        public const int PFM_RESERVED2 = 0x08000000;    // RE 4.0 

        public const int PFE_RTLPARA = (PFM_RTLPARA >> 16);
        public const int PFE_KEEP = (PFM_KEEP >> 16);   // (*)	
        public const int PFE_KEEPNEXT = (PFM_KEEPNEXT >> 16);   // (*)	
        public const int PFE_PAGEBREAKBEFORE = (PFM_PAGEBREAKBEFORE >> 16); // (*)	
        public const int PFE_NOLINENUMBER = (PFM_NOLINENUMBER >> 16);   // (*)	
        public const int PFE_NOWIDOWCONTROL = (PFM_NOWIDOWCONTROL >> 16);   // (*)	
        public const int PFE_DONOTHYPHEN = (PFM_DONOTHYPHEN >> 16); // (*)	
        public const int PFE_SIDEBYSIDE = (PFM_SIDEBYSIDE >> 16);   // (*)	
        public const int PFE_TEXTWRAPPINGBREAK = (PFM_TEXTWRAPPINGBREAK >> 16); // (*)	

        // The following four effects are read only
        public const int PFE_COLLAPSED = (PFM_COLLAPSED >> 16); // (+)	
        public const int PFE_BOX = (PFM_BOX >> 16); // (+)	
        public const int PFE_TABLE = (PFM_TABLE >> 16); // Inside table row. RE 3.0 
        public const int PFE_TABLEROWDELIMITER = (PFM_TABLEROWDELIMITER >> 16); // Table row start. RE 4.0 

        // PARAFORMAT numbering options 
        public const int PFN_BULLET = 1;        // tomListBullet

        // PARAFORMAT2 wNumbering options 
        public const int PFN_ARABIC = 2;        // tomListNumberAsArabic:   0, 1, 2,	...
        public const int PFN_LCLETTER = 3;      // tomListNumberAsLCLetter: a, b, c,	...
        public const int PFN_UCLETTER = 4;      // tomListNumberAsUCLetter: A, B, C,	...
        public const int PFN_LCROMAN = 5;       // tomListNumberAsLCRoman:  i, ii, iii,	...
        public const int PFN_UCROMAN = 6;       // tomListNumberAsUCRoman:  I, II, III,	...

        // PARAFORMAT2 wNumberingStyle options 
        public const int PFNS_PAREN = 0x000;    // default, e.g.,				  1)	
        public const int PFNS_PARENS = 0x100;   // tomListParentheses/256, e.g., (1)	
        public const int PFNS_PERIOD = 0x200;   // tomListPeriod/256, e.g.,		  1.	
        public const int PFNS_PLAIN = 0x300;    // tomListPlain/256, e.g.,		  1		
        public const int PFNS_NONUMBER = 0x400; // Used for continuation w/o number

        public const int PFNS_NEWNUMBER = 0x8000;   // Start new number with wNumberingStart		
                                                    // (can be combined with other PFNS_xxx)
                                                    // PARAFORMAT alignment options 
        public const int PFA_LEFT = 1;
        public const int PFA_RIGHT = 2;
        public const int PFA_CENTER = 3;

        // PARAFORMAT2 alignment options 
        public const int PFA_JUSTIFY = 4;   // New paragraph-alignment option 2.0 (*) 
        public const int PFA_FULL_INTERWORD = 4;    // These are supported in 3.0 with advanced
        public const int PFA_FULL_INTERLETTER = 5;  //  typography enabled
        public const int PFA_FULL_SCALED = 6;
        public const int PFA_FULL_GLYPHS = 7;
        public const int PFA_SNAP_GRID = 8;

        public const int MAX_TAB_STOPS = 32;

        public const int EM_REPLACESEL = 0x00C2;
        public const int EM_GETSEL = 0x00B0;
        public const int EM_SETSEL = 0x00B1;
    }
}
