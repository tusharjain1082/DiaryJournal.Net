using MigraDocXML.DOM;
using MigraDocXML.DOM.Charting;
using MigraDocXML.DOM.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MigraDocXML.DOM
{
    public delegate void DOMRelation(DOMElement parent, DOMElement child);


    public static class DOMRelations
    {
        private static Dictionary<Type, Dictionary<Type, DOMRelation>> _relations = new Dictionary<Type, Dictionary<Type, DOMRelation>>();


        static DOMRelations()
        {
            Create<Document, Section>((parent, child) => (parent as Document).GetDocumentModel().Add((child as Section).GetSectionModel()));

            Create<Section, Paragraph>((parent, child) => (parent as Section).GetSectionModel().Add((child as Paragraph).GetParagraphModel()));
            Create<Section, Table>((parent, child) => (parent as Section).GetSectionModel().Add((child as Table).GetTableModel()));
            Create<Section, Image>((parent, child) => (parent as Section).GetSectionModel().Add((child as Image).GetImageModel()));
            Create<Section, TextFrame>((parent, child) => (parent as Section).GetSectionModel().Add((child as TextFrame).GetTextFrameModel()));
            Create<Section, PageBreak>((parent, child) => (parent as Section).GetSectionModel().AddPageBreak());
            Create<Section, Chart>((parent, child) => (parent as Section).GetSectionModel().Add((child as Chart).GetChartModel()));
            Create<Section, Header>((parent, child) =>
            {
                bool first = true;
                var section = parent as Section;
                var header = child as Header;
                if(header.IsPrimary)
                {
                    section.GetSectionModel().Headers.Primary = header.GetHeaderModel();
                    first = false;
                }
                if(header.IsEvenPage)
                {
                    section.GetSectionModel().Headers.EvenPage = first ? header.GetHeaderModel() : header.GetHeaderModel().Clone();
                    first = false;
                }
                if (header.IsFirstPage)
                    section.GetSectionModel().Headers.FirstPage = first ? header.GetHeaderModel() : header.GetHeaderModel().Clone();
            });
            Create<Section, Footer>((parent, child) =>
            {
                bool first = true;
                var section = parent as Section;
                var footer = child as Footer;
                if (footer.IsPrimary)
                {
                    section.GetSectionModel().Footers.Primary = footer.GetFooterModel();
                    first = false;
                }
                if(footer.IsEvenPage)
                {
                    section.GetSectionModel().Footers.EvenPage = first ? footer.GetFooterModel() : footer.GetFooterModel().Clone();
                    first = false;
                }
                if (footer.IsFirstPage)
                    section.GetSectionModel().Footers.FirstPage = first ? footer.GetFooterModel() : footer.GetFooterModel().Clone();
            });
            Create<Section, PointList>((parent, child) => { });

            Create<Footer, Paragraph>((parent, child) => (parent as Footer).GetFooterModel().Add((child as Paragraph).GetParagraphModel()));
            Create<Footer, Chart>((parent, child) => (parent as Footer).GetFooterModel().Add((child as Chart).GetChartModel()));
            Create<Footer, Image>((parent, child) => (parent as Footer).GetFooterModel().Add((child as Image).GetImageModel()));
            Create<Footer, TextFrame>((parent, child) => (parent as Footer).GetFooterModel().Add((child as TextFrame).GetTextFrameModel()));
            Create<Footer, Table>((parent, child) => (parent as Footer).GetFooterModel().Add((child as Table).GetTableModel()));

            Create<Header, Paragraph>((parent, child) => (parent as Header).GetHeaderModel().Add((child as Paragraph).GetParagraphModel()));
            Create<Header, Chart>((parent, child) => (parent as Header).GetHeaderModel().Add((child as Chart).GetChartModel()));
            Create<Header, Image>((parent, child) => (parent as Header).GetHeaderModel().Add((child as Image).GetImageModel()));
            Create<Header, TextFrame>((parent, child) => (parent as Header).GetHeaderModel().Add((child as TextFrame).GetTextFrameModel()));
            Create<Header, Table>((parent, child) => (parent as Header).GetHeaderModel().Add((child as Table).GetTableModel()));

            Create<Paragraph, FormattedText>((parent, child) => (parent as Paragraph).GetParagraphModel().Add((child as FormattedText).GetFormattedTextModel()));
            Create<Paragraph, Text>((parent, child) => (parent as Paragraph).GetParagraphModel().Add((child as Text).GetTextModel()));
            Create<Paragraph, DateField>((parent, child) => (parent as Paragraph).GetParagraphModel().Add((child as DateField).GetDateFieldModel()));
            Create<Paragraph, PageField>((parent, child) => (parent as Paragraph).GetParagraphModel().Add((child as PageField).GetPageFieldModel()));
            Create<Paragraph, NumPagesField>((parent, child) => (parent as Paragraph).GetParagraphModel().Add((child as NumPagesField).GetNumPagesFieldModel()));
            Create<Paragraph, Image>((parent, child) => (parent as Paragraph).GetParagraphModel().Add((child as Image).GetImageModel()));
            Create<Paragraph, Hyperlink>((parent, child) => (parent as Paragraph).GetParagraphModel().Add((child as Hyperlink).GetHyperlinkModel()));
            Create<Paragraph, Bookmark>((parent, child) => (parent as Paragraph).GetParagraphModel().Add((child as Bookmark).GetBookmarkModel()));

            Create<Cell, Paragraph>((parent, child) => (parent as Cell).GetCellModel().Add((child as Paragraph).GetParagraphModel()));
            Create<Cell, Image>((parent, child) => (parent as Cell).GetCellModel().Add((child as Image).GetImageModel()));
            Create<Cell, Table>((parent, child) => (parent as Cell).GetCellModel().Elements.Add((child as Table).GetTableModel()));

            Create<Row, Cell>((parent, child) =>
            {
                var cell = child as Cell;
                cell.SetCellModel((parent as Row).GetRowModel().Cells[cell.Index]);
            });

            Create<Table, Row>((parent, child) => (child as Row).SetRowModel((parent as Table).GetTableModel().AddRow()));
            Create<Table, Column>((parent, child) => (child as Column).SetColumnModel((parent as Table).GetTableModel().AddColumn()));

            Create<TextFrame, Paragraph>((parent, child) => (parent as TextFrame).GetTextFrameModel().Add((child as Paragraph).GetParagraphModel()));
            Create<TextFrame, Image>((parent, child) => (parent as TextFrame).GetTextFrameModel().Add((child as Image).GetImageModel()));
            Create<TextFrame, Table>((parent, child) => (parent as TextFrame).GetTextFrameModel().Add((child as Table).GetTableModel()));

            Create<Hyperlink, FormattedText>((parent, child) => (parent as Hyperlink).GetHyperlinkModel().Add((child as FormattedText).GetFormattedTextModel()));
            Create<Hyperlink, Text>((parent, child) => (parent as Hyperlink).GetHyperlinkModel().Add((child as Text).GetTextModel()));
            Create<Hyperlink, DateField>((parent, child) => (parent as Hyperlink).GetHyperlinkModel().Add((child as DateField).GetDateFieldModel()));
            Create<Hyperlink, PageField>((parent, child) => (parent as Hyperlink).GetHyperlinkModel().Add((child as PageField).GetPageFieldModel()));
            Create<Hyperlink, NumPagesField>((parent, child) => (parent as Hyperlink).GetHyperlinkModel().Add((child as NumPagesField).GetNumPagesFieldModel()));
            Create<Hyperlink, Image>((parent, child) => (parent as Hyperlink).GetHyperlinkModel().Add((child as Image).GetImageModel()));
            Create<Hyperlink, Bookmark>((parent, child) => (parent as Hyperlink).GetHyperlinkModel().Add((child as Bookmark).GetBookmarkModel()));

            Create<Chart, TopArea>((parent, child) => (child as TopArea).SetTextAreaModel((parent as Chart).GetChartModel().TopArea));
            Create<Chart, BottomArea>((parent, child) => (child as BottomArea).SetTextAreaModel((parent as Chart).GetChartModel().BottomArea));
            Create<Chart, FooterArea>((parent, child) => (child as FooterArea).SetTextAreaModel((parent as Chart).GetChartModel().FooterArea));
            Create<Chart, HeaderArea>((parent, child) => (child as HeaderArea).SetTextAreaModel((parent as Chart).GetChartModel().HeaderArea));
            Create<Chart, LeftArea>((parent, child) => (child as LeftArea).SetTextAreaModel((parent as Chart).GetChartModel().LeftArea));
            Create<Chart, RightArea>((parent, child) => (child as RightArea).SetTextAreaModel((parent as Chart).GetChartModel().RightArea));
            Create<Chart, PlotArea>((parent, child) => (child as PlotArea).SetPlotAreaModel((parent as Chart).GetChartModel().PlotArea));
            Create<Chart, DataLabel>((parent, child) => (child as DataLabel).SetDataLabelModel((parent as Chart).GetChartModel().DataLabel));
            Create<Chart, Series>((parent, child) => (parent as Chart).GetChartModel().SeriesCollection.Add((child as Series).GetSeriesModel()));
            Create<Chart, XSeries>((parent, child) => (parent as Chart).GetChartModel().XValues.Add((child as XSeries).GetXSeriesModel()));
            Create<Chart, XAxis>((parent, child) => (child as XAxis).SetAxisModel((parent as Chart).GetChartModel().XAxis));
            Create<Chart, YAxis>((parent, child) => (child as YAxis).SetAxisModel((parent as Chart).GetChartModel().YAxis));
            Create<Chart, ZAxis>((parent, child) => (child as ZAxis).SetAxisModel((parent as Chart).GetChartModel().ZAxis));

            Create<TextArea, Legend>((parent, child) => (child as Legend).SetLegendModel((parent as TextArea).GetTextAreaModel().AddLegend()));

			
            Create<Graphics, Bezier>((parent, child) => { });
			Create<Graphics, Line>((parent, child) => { });
			Create<Graphics, Rect>((parent, child) => { });
			Create<Graphics, Drawing.String>((parent, child) => { });

            Create<Bezier, Point>((parent, child) => { });

			Create<Line, Point>((parent, child) => { });
		}


        public static void Relate(DOMElement parent, DOMElement child)
        {
            var parentType = parent.GetType();
            Dictionary<Type, DOMRelation> relations = GetParentRelations(parentType);
            if (relations == null)
                throw new InvalidOperationException($"No relation set up between parent {parentType.Name} and child {child.GetType().Name}");

            var childType = child.GetType();
            DOMRelation relation = null;
            if (relations.ContainsKey(childType))
                relation = relations[childType];
            else
                relation = relations.FirstOrDefault(x => childType.IsSubclassOf(x.Key)).Value;
            if(relation == null)
                throw new InvalidOperationException($"No relation set up between parent {parentType.Name} and child {childType.Name}");

            relation.Invoke(parent, child);
        }


        public static void Create<TParent, TChild>(DOMRelation relation) where TParent : DOMElement where TChild : DOMElement
        {
            Type parentType = typeof(TParent);
            Type childType = typeof(TChild);

            Dictionary<Type, DOMRelation> relations = GetParentRelations(parentType);
            if (relations == null)
            {
                relations = new Dictionary<Type, DOMRelation>();
                _relations[parentType] = relations;
            }

            relations[childType] = relation;
        }


        private static Dictionary<Type, DOMRelation> GetParentRelations(Type parentType)
        {
            Dictionary<Type, DOMRelation> relations = null;
            if (_relations.ContainsKey(parentType))
                relations = _relations[parentType];
            else
                relations = _relations.FirstOrDefault(x => parentType.IsSubclassOf(x.Key)).Value;
            return relations;
        }
    }
}
