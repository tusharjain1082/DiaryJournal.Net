using MigraDocXML.DOM;
using MigraDocXML.DOM.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace MigraDocXML
{
    public class Document : DOMElement
    {
        /// <summary>
        /// Signifies that the document is currently being discarded by the build process
        /// </summary>
        public event EventHandler Discarded;

        internal void DispatchDiscarded() => Discarded?.Invoke(this, null);


        private MigraDoc.DocumentObjectModel.Document _model;

        public override MigraDoc.DocumentObjectModel.DocumentObject GetModel() => _model;

        public MigraDoc.DocumentObjectModel.Document GetDocumentModel() => _model;


        private XmlElement _xmlElement;
        public XmlElement GetXmlElement() => _xmlElement;
        internal void SetXmlElement(XmlElement xmlElement) => _xmlElement = xmlElement;


        public EvalScript.Runner ScriptRunner { get; private set; }
        


        public override void SetUnknownAttribute(string name, object value)
        {
            //Ignore any unrecognised attributes, as these are most likely for referencing namespace
        }


        public override Document GetDocument()
        {
            return this;
        }


        /// <summary>
        /// </summary>
        /// <param name="modelData">The object used for passing data into the document</param>
        public Document(object modelData, EvalScript.Runner scriptRunner)
        {
            ScriptRunner = scriptRunner ?? throw new ArgumentNullException(nameof(scriptRunner));
            _model = new MigraDoc.DocumentObjectModel.Document();
            NewVariable("Document", this);
            NewVariable("Model", modelData);
		}

		
		/// <summary>
		/// Stores the version of this document post-rendering
		/// </summary>
		private MigraDoc.Rendering.PdfDocumentRenderer _renderer;

		public MigraDoc.Rendering.PdfDocumentRenderer GetRenderer() => _renderer;

		private Dictionary<int, PdfSharp.Drawing.XGraphics> _pageCanvases;
		
		/// <summary>
		/// Render the document to a PdfSharp.Pdf.PdfDocument object, then add any additional graphics contained inside Graphics elements
		/// </summary>
		/// <returns></returns>
		public PdfSharp.Pdf.PdfDocument AddGraphics()
		{
			_renderer = new MigraDoc.Rendering.PdfDocumentRenderer();
			_renderer.Document = this.GetDocumentModel();
			_renderer.RenderDocument();
			_pageCanvases = new Dictionary<int, PdfSharp.Drawing.XGraphics>();
			
			foreach (var graphics in GetAllDescendents().OfType<Graphics>())
				graphics.ChildProcessor();
			
			return _renderer.PdfDocument;
		}

		/// <summary>
		/// Get the PdfSharp.Drawing.XGraphics object for the passed in page number
		/// If AddGraphics has not yet been called, returns null
		/// </summary>
		/// <param name="page"></param>
		/// <returns></returns>
		public PdfSharp.Drawing.XGraphics GetPageCanvas(int page)
		{
			if (_renderer == null)
				return null;
			if (_pageCanvases.ContainsKey(page))
				return _pageCanvases[page];
			var canvas = PdfSharp.Drawing.XGraphics.FromPdfPage(_renderer.PdfDocument.Pages[page - 1]);
			_pageCanvases[page] = canvas;
			return canvas;
		}



		public string FootnoteLocation
        {
            get => _model.FootnoteLocation.ToString();
            set => _model.FootnoteLocation = Parse.Enum<MigraDoc.DocumentObjectModel.FootnoteLocation>(value);
        }

        public string FootnoteNumberingRule
        {
            get => _model.FootnoteNumberingRule.ToString();
            set => _model.FootnoteNumberingRule = Parse.Enum<MigraDoc.DocumentObjectModel.FootnoteNumberingRule>(value);
        }

        public string FootnoteNumberStyle
        {
            get => _model.FootnoteNumberStyle.ToString();
            set => _model.FootnoteNumberStyle = Parse.Enum<MigraDoc.DocumentObjectModel.FootnoteNumberStyle>(value);
        }

        public int FootnoteStartingNumber { get => _model.FootnoteStartingNumber; set => _model.FootnoteStartingNumber = value; }

        public string ImagePath
        {
            get => _model.ImagePath;
            set
            {
                if (System.IO.Path.IsPathRooted(value))
                    _model.ImagePath = value;
                else if (_model.ImagePath != null)
                    _model.ImagePath = System.IO.Path.Combine(_model.ImagePath, value);
                else
                    _model.ImagePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, value);
            }
        }

        private string _resourcePath;
        public string ResourcePath
        {
            get => _resourcePath;
            set
            {
                if (System.IO.Path.IsPathRooted(value))
                    _resourcePath = value;
                else if (_resourcePath != null)
                    _resourcePath = System.IO.Path.Combine(_resourcePath, value);
                else
                    _resourcePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, value);
            }
        }

        public bool UseCmykColor { get => _model.UseCmykColor; set => _model.UseCmykColor = value; }
    }
}
