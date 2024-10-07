using MigraDocXML.DOM;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MigraDocXML
{
	public static class EvalScriptGraphicsFunctions
	{
		public static List<RenderArea> GetAreas(object[] args)
		{
			if (args.Length != 1)
				throw new ArgumentException("GetAreas expects one argument of type DOMElement");
			var element = args[0] as DOMElement;
			return GetRenderAreas(new List<DOMElement>() { element });
		}

		public static RenderArea GetArea(object[] args)
		{
			if (args.Length != 1)
				throw new ArgumentException("GetArea expects one argument of type DOMElement");
			var element = args[0] as DOMElement;
			return GetRenderAreas(new List<DOMElement>() { element }).FirstOrDefault();
		}

		public static List<RenderArea> GetTaggedAreas(object[] args)
		{
			if (args.Length != 2 || !(args[0] is DOMElement) || !(args[1] is string))
				throw new ArgumentException("GetTaggedAreas expects two arguments: element, tagName");
			var element = args[0] as DOMElement;
			var tag = args[1] as string;
			var elements = element.GetAllDescendents().Where(x => x.IsPresentable && x.Tag == tag).ToList();
			if (elements.Count == 0)
				return new List<RenderArea>();
			return GetRenderAreas(elements);
		}

		public static List<RenderArea> GetAreasOfType(object[] args)
		{
			if (args.Length != 2 || !(args[0] is DOMElement) || !(args[1] is string))
				throw new ArgumentException("GetAreasByType expects two arguments: element, typeName");
			var element = args[0] as DOMElement;
			var typeName = args[1] as string;
			var type = DOMTypes.Lookup(typeName);
			var elements = element.GetAllDescendents().Where(x => x.IsPresentable && (x.GetType() == type || type.IsAssignableFrom(x.GetType()))).ToList();
			if (elements.Count == 0)
				return new List<RenderArea>();
			return GetRenderAreas(elements);
		}

		public static object GetPageCount(object[] args)
		{
			if (args.Length != 1 || !(args[0] is Document))
				throw new ArgumentException("GetPageCount expects one argument: document");
			var doc = args[0] as Document;
			var renderer = doc.GetRenderer();
			if (renderer == null)
				throw new Exception("Document hasn't yet started being rendered, can't get document page count");
			return renderer.DocumentRenderer.FormattedDocument.PageCount;
		}


        /// <summary>
        /// Takes in a collection of DOMElement objects to look for and returns a list of RenderAreas, defining where those elements are rendered on the document
        /// </summary>
        /// <param name="targetElements"></param>
        /// <returns></returns>
		private static List<RenderArea> GetRenderAreas(IEnumerable<DOMElement> targetElements)
		{
			if (targetElements.Count() == 0)
				throw new ArgumentException("targetElements parameter must contain at least one element");

            //Get the list of MigraDoc.DocumentObjectModel.DocumentObjects that we are looking for
			var targets = targetElements.Select(x => x.GetModel()).ToList();
            
            //Build up a full list of all the DocumentObjects which are parents to the targets, all the way up to Document
            //This allows for efficient filtering of where to look for our target elements
			var fullPath = new List<MigraDoc.DocumentObjectModel.DocumentObject>();
			foreach (var target in targetElements)
				fullPath.AddRange(GetElementPath(target));
			fullPath = fullPath.Distinct().ToList();

            //The list that will be returned at the end of the process
			var output = new List<RenderArea>();

            //We use the renderer object as a gateway to all the objects which store the positional data for our objects
			var renderer = targetElements.First().GetDocument().GetRenderer();
			if (renderer == null)
				throw new Exception("Document hasn't yet started being rendered, can't get element render areas");

            //Loop through each page of the document
			var formattedDoc = renderer.DocumentRenderer.FormattedDocument;
			for(int page = 1; page <= formattedDoc.PageCount; page++)
			{
                //This stores all the objects which need to be investigated further, either because they, or one or more of their children, are targets
				Queue<OffsetRenderInfo> renderInfoQueue = new Queue<OffsetRenderInfo>();
                //Loop through each RenderInfo on the current page, adding to renderInfoQueue if its document object is in the fullPath list
				foreach(var renderInfo in formattedDoc.GetRenderInfos(page).Where(x => fullPath.Contains(x.DocumentObject)))
					renderInfoQueue.Enqueue(new OffsetRenderInfo(renderInfo, 0, 0));

                //LoadHeaderChildren(targetElements.First().GetDocument(), page, targets, fullPath, renderInfoQueue);
				//LoadFooterChildren(targetElements.First().GetDocument(), page, targets, fullPath, renderInfoQueue);

                //For each item in the renderInfoQueue, if the RenderInfo's DocumentObject is in targets list, add new RenderArea to output, also try to find any relevant RenderInfo children
				while(renderInfoQueue.Count > 0)
				{
					var offsetRenderInfo = renderInfoQueue.Dequeue();
					var renderInfo = offsetRenderInfo.RenderInfo;
					if (targets.Contains(renderInfo.DocumentObject))
						output.Add(GetRenderArea(page, offsetRenderInfo));

					if (renderInfo is MigraDoc.Rendering.TableRenderInfo)
						output.AddRange(GetTableRenderAreas(offsetRenderInfo, page, targets, fullPath, renderInfoQueue));
					
					else if (renderInfo is MigraDoc.Rendering.TextFrameRenderInfo)
						LoadTextFrameChildren(offsetRenderInfo, page, targets, fullPath, renderInfoQueue);
				}
			}
			return output;
		}


		private static void LoadTextFrameChildren(
			OffsetRenderInfo offsetRenderInfo,
			int page,
			IEnumerable<MigraDoc.DocumentObjectModel.DocumentObject> targets,
			IEnumerable<MigraDoc.DocumentObjectModel.DocumentObject> fullPath,
			Queue<OffsetRenderInfo> renderInfoQueue
		)
		{
			var renderInfo = offsetRenderInfo.RenderInfo;
			var frameRenderInfo = renderInfo as MigraDoc.Rendering.TextFrameRenderInfo;
			var frame = frameRenderInfo.DocumentObject as MigraDoc.DocumentObjectModel.Shapes.TextFrame;
			var formatInfo = frameRenderInfo.FormatInfo;
			var formattedTextFrame = formatInfo.GetType()
				.GetField("FormattedTextFrame", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.FlattenHierarchy | System.Reflection.BindingFlags.Instance)
				.GetValue(formatInfo);
			var childRenderInfos = (IEnumerable<MigraDoc.Rendering.RenderInfo>)formattedTextFrame.GetType()
				.GetField("_renderInfos", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.FlattenHierarchy | System.Reflection.BindingFlags.Instance)
				.GetValue(formattedTextFrame);

			foreach(var child in childRenderInfos)
			{
				if (fullPath.Contains(child.DocumentObject))
					renderInfoQueue.Enqueue(new OffsetRenderInfo(child, renderInfo.LayoutInfo.ContentArea.X, renderInfo.LayoutInfo.ContentArea.Y));
			}
		}


		//Can't get the logic to work write for tables inside of headers/footers. Leaving for now as a limitation of the methods, rather than add buggy logic. May introduce later
        /*private static void LoadHeaderChildren(
            Document document,
            int page,
            IEnumerable<MigraDoc.DocumentObjectModel.DocumentObject> targets,
            IEnumerable<MigraDoc.DocumentObjectModel.DocumentObject> fullPath,
            Queue<OffsetRenderInfo> renderInfoQueue
        )
        {
            if (!fullPath.Any(x => x is MigraDoc.DocumentObjectModel.HeaderFooter))
                return;

            var formattedDoc = document.GetRenderer().DocumentRenderer.FormattedDocument;
            var section = document.GetAllDescendents().OfType<Section>().First();
            var pageSetup = section.PageSetup;

            var formattedHeaders = (IEnumerable)formattedDoc.GetType()
                .GetField("_formattedHeaders", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.FlattenHierarchy | System.Reflection.BindingFlags.Instance)
                .GetValue(formattedDoc);

            foreach(var header in formattedHeaders)
            {
                var pairKey = header.GetType()
                    .GetProperty("Key")
                    .GetValue(header, null);

                var pagePosition = pairKey.GetType()
                    .GetField("_pagePosition", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.FlattenHierarchy | System.Reflection.BindingFlags.Instance)
                    .GetValue(pairKey)?.ToString();

                if (pagePosition == "First" && page != 1 && pageSetup.DifferentFirstPageHeaderFooter)
                    continue;
                if (pagePosition == "Odd" && page % 2 == 0 && pageSetup.OddAndEvenPagesHeaderFooter)
                    continue;
                if (pagePosition == "Even" && page % 2 == 1 && pageSetup.OddAndEvenPagesHeaderFooter)
                    continue;

                var pairValue = header.GetType()
                    .GetProperty("Value")
                    .GetValue(header, null);

                var childRenderInfos = (IEnumerable<MigraDoc.Rendering.RenderInfo>)pairValue.GetType()
                    .GetField("_renderInfos", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.FlattenHierarchy | System.Reflection.BindingFlags.Instance)
                    .GetValue(pairValue);
                
                foreach (var child in childRenderInfos)
                {
                    if (fullPath.Contains(child.DocumentObject))
                        renderInfoQueue.Enqueue(new OffsetRenderInfo(child, 0, 0));
                }
            }
        }

		private static void LoadFooterChildren(
			Document document,
			int page,
			IEnumerable<MigraDoc.DocumentObjectModel.DocumentObject> targets,
			IEnumerable<MigraDoc.DocumentObjectModel.DocumentObject> fullPath,
			Queue<OffsetRenderInfo> renderInfoQueue
		)
		{
			if (!fullPath.Any(x => x is MigraDoc.DocumentObjectModel.HeaderFooter))
				return;

			var formattedDoc = document.GetRenderer().DocumentRenderer.FormattedDocument;
			var section = document.GetAllDescendents().OfType<Section>().First();
			var pageSetup = section.PageSetup;

			var formattedFooters = (IEnumerable)formattedDoc.GetType()
				.GetField("_formattedFooters", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.FlattenHierarchy | System.Reflection.BindingFlags.Instance)
				.GetValue(formattedDoc);

			foreach (var footer in formattedFooters)
			{
				var pairKey = footer.GetType()
					.GetProperty("Key")
					.GetValue(footer, null);

				var pagePosition = pairKey.GetType()
					.GetField("_pagePosition", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.FlattenHierarchy | System.Reflection.BindingFlags.Instance)
					.GetValue(pairKey)?.ToString();

				if (pagePosition == "First" && page != 1 && pageSetup.DifferentFirstPageHeaderFooter)
					continue;
				if (pagePosition == "Odd" && page % 2 == 0 && pageSetup.OddAndEvenPagesHeaderFooter)
					continue;
				if (pagePosition == "Even" && page % 2 == 1 && pageSetup.OddAndEvenPagesHeaderFooter)
					continue;

				var pairValue = footer.GetType()
					.GetProperty("Value")
					.GetValue(footer, null);

				var childRenderInfos = (IEnumerable<MigraDoc.Rendering.RenderInfo>)pairValue.GetType()
					.GetField("_renderInfos", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.FlattenHierarchy | System.Reflection.BindingFlags.Instance)
					.GetValue(pairValue);

				foreach (var child in childRenderInfos)
				{
					if (fullPath.Contains(child.DocumentObject))
						renderInfoQueue.Enqueue(new OffsetRenderInfo(child, 0, 0));
				}
			}
		}*/


		/// <summary>
		/// Go through table children, getting render areas for the targets
		/// Has side-effect of pushing to renderInfoQueue with OffsetRenderInfos that need to be further investigated to find target objects
		/// </summary>
		/// <param name="offsetRenderInfo"></param>
		/// <param name="page"></param>
		/// <param name="targets"></param>
		/// <param name="fullPath"></param>
		/// <param name="renderInfoQueue"></param>
		/// <returns></returns>
		private static List<RenderArea> GetTableRenderAreas(
            OffsetRenderInfo offsetRenderInfo, 
            int page,
            IEnumerable<MigraDoc.DocumentObjectModel.DocumentObject> targets, 
            IEnumerable<MigraDoc.DocumentObjectModel.DocumentObject> fullPath, 
            Queue<OffsetRenderInfo> renderInfoQueue
        )
        {
            //Get all required Table & RenderInfo objects
            var renderInfo = offsetRenderInfo.RenderInfo;
            var tblRenderInfo = renderInfo as MigraDoc.Rendering.TableRenderInfo;
            var tblFormatInfo = tblRenderInfo.FormatInfo as MigraDoc.Rendering.TableFormatInfo;
            var tbl = tblRenderInfo.DocumentObject as MigraDoc.DocumentObjectModel.Tables.Table;

            //These determine if we actually need to loop through each cell, if none of the table cells are in the fullPath list, and we're not targeting any of the table's rows or columns
            //then we can skip looping through all the cells
            var targetRows = targets.OfType<MigraDoc.DocumentObjectModel.Tables.Row>().Where(x => x.Table == tbl && x.Index >= tblFormatInfo.StartRow && x.Index <= tblFormatInfo.EndRow).ToList();
            var targetColumns = targets.OfType<MigraDoc.DocumentObjectModel.Tables.Column>().Where(x => x.Table == tbl).ToList();
            var inPathCells = fullPath.OfType<MigraDoc.DocumentObjectModel.Tables.Cell>().Where(x => x.Table == tbl && x.Row.Index >= tblFormatInfo.StartRow && x.Row.Index <= tblFormatInfo.EndRow).ToList();
            if (targetRows.Count == 0 && targetColumns.Count == 0 && inPathCells.Count == 0)
                return new List<RenderArea>();
            var targetCells = targets.OfType<MigraDoc.DocumentObjectModel.Tables.Cell>().Where(x => x.Table == tbl && x.Row.Index >= tblFormatInfo.StartRow && x.Row.Index <= tblFormatInfo.EndRow).ToList();

            //Populate list of all column offsets, this is required since individual cells don't store their bounds, and all cell children store their positions relative to the cell position
            //Do same for row offsets, but only starts getting populated as we loop through cells
            var columnOffsets = new List<double>() { offsetRenderInfo.XOffset + tblRenderInfo.LayoutInfo.ContentArea.X.Point };
            for (int i = 0; i < tbl.Columns.Count; i++)
                columnOffsets.Add(columnOffsets.Last() + tbl.Columns[i].Width.Point);
            var rowOffsets = new List<double>() { offsetRenderInfo.YOffset + tblRenderInfo.LayoutInfo.ContentArea.Y.Point };

            //Loop through all cells in the table contained on the current page
            foreach (var cell in tblFormatInfo.FormattedCells.Where(x => x.Key.Row.Index >= tblFormatInfo.StartRow && x.Key.Row.Index <= tblFormatInfo.EndRow))
            {
                //If the rowOffset for the row after the current cell's row hasn't been added yet, add it based on the current row offset plus the height of the current cell
                if (cell.Value.Cell.Row.Index - tblFormatInfo.StartRow == rowOffsets.Count - 1)
                {
                    var contentHeight = (PdfSharp.Drawing.XUnit)cell.Value.GetType()
                        .GetField("_contentHeight", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.FlattenHierarchy | System.Reflection.BindingFlags.Instance)
                        .GetValue(cell.Value);
                    rowOffsets.Add(rowOffsets.Last() + contentHeight.Point);
                }

                //If the current cell is in the fullPath list, go through each of the cell's children, adding them to the renderInfoQueue if they're also in the fullPath list
                if (inPathCells.Any(x => x == cell.Key))
                {
                    var cellChildren = (IEnumerable<MigraDoc.Rendering.RenderInfo>)cell.Value.GetType()
                        .GetField("_renderInfos", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.FlattenHierarchy | System.Reflection.BindingFlags.Instance)
                        .GetValue(cell.Value);
                    foreach (var cellChild in cellChildren.Where(x => fullPath.Contains(x.DocumentObject)))
                    {
                        renderInfoQueue.Enqueue(new OffsetRenderInfo(
                            cellChild,
                            columnOffsets[cell.Key.Column.Index] + tbl.LeftPadding.Point,
                            rowOffsets[cell.Key.Row.Index - tblFormatInfo.StartRow]
                        ));
                    }
                }
            }

            var output = new List<RenderArea>();

            //For each row in targetRows, add a new RenderArea to be returned by this function
            foreach (var row in targetRows)
            {
                output.Add(new RenderArea(page,
                    top: rowOffsets[row.Index - tblFormatInfo.StartRow],
                    left: columnOffsets[0],
                    width: columnOffsets.Last() - columnOffsets.First(),
                    height: rowOffsets[row.Index + 1 - tblFormatInfo.StartRow] - rowOffsets[row.Index - tblFormatInfo.StartRow]
                ));
            }

            //For each column in targetColumns, add a new RenderArea to be returned by this function
            foreach (var column in targetColumns)
            {
                output.Add(new RenderArea(page,
                    top: rowOffsets.First(),
                    left: columnOffsets[column.Index],
                    width: columnOffsets[column.Index + 1] - columnOffsets[column.Index],
                    height: rowOffsets.Last() - rowOffsets.First()
                ));
            }

            //For each cell in targetCells, add a new RenderArea to be returned to by this function
            foreach(var cell in targetCells)
            {
                output.Add(new RenderArea(page,
                    top: rowOffsets[cell.Row.Index - tblFormatInfo.StartRow],
                    left: columnOffsets[cell.Column.Index],
                    width: columnOffsets[cell.Column.Index + 1] - columnOffsets[cell.Column.Index],
                    height: rowOffsets[cell.Row.Index + 1 - tblFormatInfo.StartRow] - rowOffsets[cell.Row.Index - tblFormatInfo.StartRow]
                ));
            }

            return output;
        }


        /// <summary>
        /// Stores a RenderInfo object alongside X & Y offsets, since for objects nested inside others, the RenderInfo only gives positional data relative to the parent's position
        /// </summary>
		private class OffsetRenderInfo
		{
			public MigraDoc.Rendering.RenderInfo RenderInfo { get; private set; }

			public double XOffset { get; private set; }

			public double YOffset { get; private set; }

			public OffsetRenderInfo(MigraDoc.Rendering.RenderInfo renderInfo, double xOffset, double yOffset)
			{
				RenderInfo = renderInfo ?? throw new ArgumentNullException(nameof(renderInfo));
				XOffset = xOffset;
				YOffset = yOffset;
			}
		}


		private static RenderArea GetRenderArea(int page, OffsetRenderInfo offsetRenderInfo)
		{
			var contentArea = offsetRenderInfo.RenderInfo.LayoutInfo.ContentArea;
			return new RenderArea(page,
				contentArea.Y.Point + offsetRenderInfo.YOffset,
				contentArea.X.Point + offsetRenderInfo.XOffset,
				contentArea.Width.Point,
				contentArea.Height.Point
			);
		}


        /// <summary>
        /// Returns the path of presentable parent objects from the passed in element all the way up to the Document
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        private static List<MigraDoc.DocumentObjectModel.DocumentObject> GetElementPath(DOMElement element)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));

            if (!element.IsPresentable)
                throw new ArgumentException("Cannot get path to non-presentable element");

            var output = new List<DOMElement>();
            while(element != null)
            {
                output.Add(element);
                element = element.GetPresentableParent();
            }
            return output.Select(x => x.GetModel()).ToList();
        }
    }
}
