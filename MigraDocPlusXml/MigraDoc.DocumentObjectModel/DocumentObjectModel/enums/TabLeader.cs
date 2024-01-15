#region MigraDoc - Creating Documents on the Fly
//
// Authors:
//   Stefan Lange
//   Klaus Potzesny
//   David Stephensen
//
// Copyright (c) 2001-2019 empira Software GmbH, Cologne Area (Germany)
//
// http://www.pdfsharp.com
// http://www.migradoc.com
// http://sourceforge.net/projects/pdfsharp
//
// Permission is hereby granted, free of charge, to any person obtaining a
// copy of this software and associated documentation files (the "Software"),
// to deal in the Software without restriction, including without limitation
// the rights to use, copy, modify, merge, publish, distribute, sublicense,
// and/or sell copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included
// in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE.
#endregion

namespace MigraDoc.DocumentObjectModel
{
    /// <summary>
    /// Used to determine the leader for the tab.
    /// </summary>
    public enum TabLeader
    {
        /// <summary>
        /// Blanks are used as leader.
        /// </summary>
        Spaces,

        /// <summary>
        /// Dots at the baseline.
        /// </summary>
        Dots,

        /// <summary>
        /// Dashes are used as leader.
        /// </summary>
        Dashes,

        /// <summary>
        /// Same as Heavy.
        /// </summary>
        Lines,

        /// <summary>
        /// Leader will be underlined.
        /// </summary>
        Heavy,

        /// <summary>
        /// Dots in the middle (vertical) of the line.
        /// </summary>
        MiddleDot
    }
}
