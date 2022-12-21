using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows;

namespace DiaryJournal.Net
{
    public static class TextRangeHelper
    {
        public static IEnumerable<TElement> EnumerateElements<TElement>(this TextRange documentRange)
            where TElement : DependencyObject
        {
            if (!TryGetFirstElementStartPositionInContentRange<TElement>(documentRange, out TextPointer selectedElementStart))
            {
                yield break;
            }

            TextPointer contentPosition = selectedElementStart;
            while (TryGetNextElement(ref contentPosition, documentRange.End, out TElement contentElement))
            {
                yield return contentElement;
                contentPosition = contentPosition.GetNextContextPosition(LogicalDirection.Forward);
            }

            yield break;
        }

        // In case the TextRange e.g. content selection starts inside the requested element,
        // we need to adjust the text range to include the current element.
        private static bool TryGetFirstElementStartPositionInContentRange<TElement>(TextRange documentRange, out TextPointer selectedElementStart)
          where TElement : DependencyObject
        {
            selectedElementStart = documentRange.Start;
            DependencyObject? contentElement = selectedElementStart?.GetAdjacentElement(LogicalDirection.Forward);
            if (contentElement is TElement)
            {
                return true;
            }

            // Step backward to capture the first TableCell element in the current selection
            while (selectedElementStart is not null)
            {
                TextPointerContext textPointerContext = selectedElementStart!.GetPointerContext(LogicalDirection.Forward);

                // Filter pointer context of content elements (TextPointerContext.ElementStart)
                // and UIElements contained in a BlockUIContainer or InlineUIContainer (TextPointerContext.EmbeddedElement)
                if (textPointerContext is not (TextPointerContext.ElementStart or TextPointerContext.EmbeddedElement))
                {
                    selectedElementStart = selectedElementStart!.GetNextContextPosition(LogicalDirection.Backward);
                    continue;
                }

                contentElement = selectedElementStart!.GetAdjacentElement(LogicalDirection.Forward);

                // Selection starts inside or after the potential target element.
                // This means we must return the coerced start pointer to make the selection start ahead of the original.
                if (contentElement is TElement)
                {
                    return true;
                }

                // Selection starts before the potential target element.
                // This means we can return the original selection start pointer.
                else if (contentElement is not (Block or Inline)
                  || (contentElement is FrameworkContentElement frameworkContentElement && frameworkContentElement.Parent is FlowDocument))
                {
                    selectedElementStart = documentRange.Start;
                    return true;
                }

                selectedElementStart = selectedElementStart!.GetNextContextPosition(LogicalDirection.Backward);
            }

            return false;
        }

        private static bool TryGetNextElement<TElement>(ref TextPointer contentPosition, TextPointer contentEndPosition, out TElement nextElement)
          where TElement : DependencyObject
        {
            nextElement = default;

            if (IsEndOfContent(contentPosition, contentEndPosition))
            {
                return false;
            }

            nextElement = contentPosition?.GetAdjacentElement(LogicalDirection.Forward) as TElement;
            if (nextElement is not null)
            {
                return true;
            }

            // Step forward to find and collect next TableCell in current selection
            while (contentPosition is not null
              && !IsEndOfContent(contentPosition, contentEndPosition))
            {
                TextPointerContext contentPositionContext = contentPosition!.GetPointerContext(LogicalDirection.Forward);

                // Filter pointer context of content elements (TextPointerContext.ElementStart)
                // and UIElements contained in a BlockUIContainer or InlineUIContainer (TextPointerContext.EmbeddedElement)
                if (contentPositionContext is not (TextPointerContext.ElementStart or TextPointerContext.EmbeddedElement))
                {
                    contentPosition = contentPosition!.GetNextContextPosition(LogicalDirection.Forward);
                    continue;
                }

                nextElement = contentPosition!.GetAdjacentElement(LogicalDirection.Forward) as TElement;
                if (nextElement is not null)
                {
                    return true;
                }

                contentPosition = contentPosition!.GetNextContextPosition(LogicalDirection.Forward);
            }

            return false;
        }

        private static bool IsEndOfContent(TextPointer contentPosition, TextPointer contentEndPosition)
        {
            if (contentPosition is null || contentEndPosition is null)
            {
                return true;
            }

            // If the pointer is pointing to the next insertion point (LogicalDirection.Forward),
            // we must reverse the end pointer to exclude the next trailing content.
            if (contentEndPosition.LogicalDirection == LogicalDirection.Forward)
            {
                contentEndPosition = contentEndPosition?.GetNextInsertionPosition(LogicalDirection.Backward);
            }

            return contentPosition?.CompareTo(contentEndPosition) >= 0;
        }
    }
}
