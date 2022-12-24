using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows;

namespace DiaryJournal.Net
{
    public static class FlowDocumentHelper
    {
        public static IEnumerable<TTextElement> WalkTextRange<TTextElement>(this FlowDocument doc, TextPointer start, TextPointer end) where TTextElement : TextElement
        {
            var lastVisited = new Dictionary<int, TTextElement>();
            foreach (var stack in doc.WalkTextHierarchy())
            {
                var element = stack.Peek() as TTextElement;
                if (element != null)
                {
                    TTextElement previous;
                    if (!lastVisited.TryGetValue(stack.Count - 1, out previous) || previous != element)
                    {
                        if (element.Overlaps(start, end))
                            yield return element;
                        lastVisited[stack.Count - 1] = element;
                    }
                }
            }
        }

        public static bool Overlaps(this TextElement element, TextPointer start, TextPointer end)
        {
            return element.ContentEnd.CompareTo(start) > 0 && element.ContentStart.CompareTo(end) < 0;
        }

        public static IEnumerable<Stack<DependencyObject>> WalkTextHierarchy(this FlowDocument doc)
        {
            if (doc == null)
                throw new ArgumentNullException();

            var stack = new Stack<DependencyObject>();

            // Keep a TextPointer for FlowDocument.ContentEnd handy, so we know when we're done.
            TextPointer docEnd = doc.ContentEnd;

            // Keep going until the TextPointer is equal to or greater than ContentEnd.
            for (var iterator = doc.ContentStart;
                (iterator != null) && (iterator.CompareTo(docEnd) < 0);
                iterator = iterator.GetNextContextPosition(LogicalDirection.Forward))
            {
                var parent = iterator.Parent;

                // Identify the type of content immediately adjacent to the text pointer.
                TextPointerContext context = iterator.GetPointerContext(LogicalDirection.Forward);

                switch (context)
                {
                    case TextPointerContext.ElementStart:
                    case TextPointerContext.EmbeddedElement:
                    case TextPointerContext.Text:
                        PushElement(stack, parent);
                        yield return stack;
                        break;

                    case TextPointerContext.ElementEnd:
                        break;

                    default:
                        throw new System.Exception("Unhandled TextPointerContext " + context.ToString());
                }

                switch (context)
                {
                    case TextPointerContext.ElementEnd:
                    case TextPointerContext.EmbeddedElement:
                    case TextPointerContext.Text:
                        PopToElement(stack, parent);
                        break;

                    case TextPointerContext.ElementStart:
                        break;

                    default:
                        throw new System.Exception("Unhandled TextPointerContext " + context.ToString());
                }
            }
        }

        static int IndexOf<T>(Stack<T> source, T value)
        {
            int index = 0;
            var comparer = EqualityComparer<T>.Default;
            foreach (T item in source)
            {
                if (comparer.Equals(item, value))
                    return index;
                index++;
            }
            return -1;
        }

        static void PopToElement<T>(Stack<T> stack, T item)
        {
            for (int index = IndexOf(stack, item); index >= 0; index--)
                stack.Pop();
        }

        static void PushElement<T>(Stack<T> stack, T item)
        {
            PopToElement(stack, item);
            stack.Push(item);
        }
    }
}
