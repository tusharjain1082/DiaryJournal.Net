using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MigraDocXML.DOM
{
    public class Image : Shape
    {
        private MigraDoc.DocumentObjectModel.Shapes.Image _model;

        public override MigraDoc.DocumentObjectModel.DocumentObject GetModel() => _model;

        public MigraDoc.DocumentObjectModel.Shapes.Image GetImageModel() => _model;


        private void Image_ParentSet(object sender, EventArgs e)
        {
            DOMRelations.Relate(GetPresentableParent(), this);
            ApplyStyling();
        }


        public Image()
        {
            _model = new MigraDoc.DocumentObjectModel.Shapes.Image();
            ShapeModel = _model;
            NewVariable("Image", this);
            ParentSet += Image_ParentSet;
        }

        public bool LockAspectRatio { get => _model.LockAspectRatio; set => _model.LockAspectRatio = value; }

        public string Name { get => _model.Name; set => _model.Name = value; }

        public double Resolution { get => _model.Resolution; set => _model.Resolution = value; }

        public double ScaleHeight { get => _model.ScaleHeight; set => _model.ScaleHeight = value; }

        public double ScaleWidth { get => _model.ScaleWidth; set => _model.ScaleWidth = value; }
    }
}
