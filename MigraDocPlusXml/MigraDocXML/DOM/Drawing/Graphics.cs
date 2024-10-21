using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MigraDocXML.DOM.Drawing
{
    public class Graphics : LogicalElement
    {
        public override MigraDoc.DocumentObjectModel.DocumentObject GetModel() => GetParent()?.GetModel();

        /// <summary>
        /// Stores the logic for processing all children of the Graphics element
        /// </summary>
        public Action ChildProcessor { get; private set; }


		public Graphics()
		{
			IsGraphical = true;
			NewVariable("Graphics", this);
			ParentSet += Graphics_ParentSet;
		}

		private void Graphics_ParentSet(object sender, EventArgs e)
		{
			CreateClosure();
		}
		
		/// <summary>
		/// Since the logic inside a graphics element only gets run after rendering has been performed
		/// We essentially create a closure around all of the variables as they currently exist
		/// So that even if their values change before graphics code comes to run
		/// We can still access them as though they hadn't
		/// </summary>
		private void CreateClosure()
		{
			var parent = GetParent();
			while(parent != null)
			{
				foreach(var variable in parent.GetOwnedVariables())
				{
					if (!OwnsVariable(variable.Key))
						NewVariable(variable.Key, variable.Value);
				}
				parent = parent.GetParent();
			}
		}

        
        public override void Run(Action childProcessor)
        {
            ChildProcessor = childProcessor;
        }


		public int? Page { get; set; }
    }
}
