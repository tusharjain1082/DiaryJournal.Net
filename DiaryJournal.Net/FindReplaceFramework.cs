using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace DiaryJournal.Net
{
    public static class FindReplaceFramework
    {
        public class MatchedTextCollection
        {
            public MatchedText? head;
            public MatchedText? tail;
            public MatchedText? current;
            private long __count = 0;
            private long __index = 0;

            String pattern = "";
            Regex regex;
            RegexOptions options;
            FlowDocument doc;
            public MatchedTextCollection() { }

            public static MatchedTextCollection? initializeSearch(ref RegexOptions options, FlowDocument doc, String pattern)
            {
                FindReplaceFramework.MatchedTextCollection col = new FindReplaceFramework.MatchedTextCollection();

                // configure
                col.options = options;
                col.pattern = pattern;
                col.doc = doc;
                col.regex = new Regex(pattern, options);

                // find all matches on the basis of pattern
                TextRange content = new TextRange(doc.ContentStart, doc.ContentEnd);
                TextPointer current = content.Start.GetInsertionPosition(LogicalDirection.Forward);
                while (current != null)
                {
                    string textInRun = current.GetTextInRun(LogicalDirection.Forward);
                    MatchCollection matches = col.regex.Matches(textInRun);
                    if (matches.Count > 0)
                    {
                        foreach (Match listedMatch in matches)
                        {
                            Match? match = listedMatch;
                            TextPointer start = current.GetPositionAtOffset(match.Index, LogicalDirection.Forward);
                            TextPointer end = current.GetPositionAtOffset(match.Index + match.Length, LogicalDirection.Forward);
                            FindReplaceFramework.MatchedText matchedText = new FindReplaceFramework.MatchedText(ref start, ref end, ref current, ref match);
                            col.Add(ref matchedText);
                            continue;
                        }

                    }
                    current = current.GetNextContextPosition(LogicalDirection.Forward);
                }
                // done
                return col;
            }
            public void Add(ref MatchedText? value)
            {
                if (Count == 0)
                {
                    head = value;
                    tail = null;
                }
                else if (Count == 1) 
                {
                    value.previous = head;
                    head.next = value;
                    tail = value;
                }
                else
                {
                    tail.next = value;
                    value.previous = tail;
                    tail = value;
                }
                value.index = __index;
                __index++;
                __count++;
            }

            #region "complicated, unsolved, todo"

            // part function of Insert() function
            public bool InsertBefore(ref MatchedText? target, ref MatchedText? value, bool incrementIndexes)
            {
                if (target == null) return false;
                MatchedText? targetPrevNode = target.previous;
                MatchedText? targetNextNode = target.next;

                // before target, so chain with target's previous node
                value.next = target;
                target.previous = value;
                value.previous = targetPrevNode;
                if (targetPrevNode != null)
                {
                    // target is not head because there is a previous node
                    targetPrevNode.next = value;
                }
                else
                {
                    // target is head because theres no previous node, so head is moved to this node which is inserted before head
                    head = value;
                }

                if (incrementIndexes)
                {
                    value.index = __index;
                    __index++;
                    __count++;
                }
                return true;
            }
            // part function of Insert() function
            public bool InsertAfter(ref MatchedText? target, ref MatchedText? value, bool incrementIndexes)
            {
                if (target == null) return false;
                MatchedText? targetPrevNode = target.previous;
                MatchedText? targetNextNode = target.next;

                // after target
                value.previous = target;
                target.next = value;
                value.next = targetNextNode;
                if (targetNextNode != null)
                {
                    // target is a middle node because there is a next node, so set target's next node's previous to this node.
                    targetNextNode.previous = value;
                }
                else
                {
                    // target is tail because there is no next node, so set this new node as tail
                    tail = value;
                }
                if (incrementIndexes)
                {
                    value.index = __index;
                    __index++;
                    __count++;
                }

                return true;
            }
            public bool SetHead(ref MatchedText? value)
            {
                if (value == null) return false;
                if (value.previous == null) head = value;
                return true;
            }
            public bool SetTail(ref MatchedText? value)
            {
                if (value == null) return false;
                if (value.next == null)
                {
                    if (value != head) // we cannot set head as tail, head is first node and tail is the last node in chain of innumerable nodes.
                        tail = value;
                }
                return true;
            }

            #region "todo"
            /*
            public bool InsertInBetween(ref MatchedText? prev, ref MatchedText? next, ref MatchedText? value)
            {
                if (value == null) return false;
                MatchedText? PrevNodePrev = target.previous;
                MatchedText? targetNextNode = target.next;

            }
            */
            #endregion

            // insert before head, indexes are incremented
            public void InsertFirst(ref MatchedText? value)
            {
                if (value == null) return;

                if (Count == 0)
                {
                    // there is no head, this new node is first then this new node is inserted at head
                    Add(ref value);
                }
                else
                {
                    // head is present, so insert before head
                    Insert(ref head, ref value, true);
                }

            }
            // insert after tail, indexes are incremented
            public void InsertLast(ref MatchedText? value)
            {
                if (value == null) return;
                Add(ref value); // add the node at the tail, if no node exists, this node is inserted at head
            }

            // primary insert function, indexes are incremented
            public bool Insert(ref MatchedText? target, ref MatchedText? value, bool insertBeforeTarget)
            {
                if (target == null) return false;
                MatchedText? targetPrevNode = target.previous;
                MatchedText? targetNextNode = target.next;

                if (insertBeforeTarget)
                {
                    // before target, so chain with target's previous node
                    InsertBefore(ref target, ref value, false);
                }
                else
                {
                    // after target
                    InsertAfter(ref target, ref value, false);
                }
                value.index = __index;
                __index++;
                __count++;
                return true;
            }
            #endregion

            public MatchedText? ElementAt(long index) 
            {
                if (__count == 0) return null;
                if (index >= __count) return null;
                MatchedText? target = head;
                //if (index == 0) return target; // 0th node is directly returned6

                long ctr = 0;
                //while (ctr <= (index - 1))
                while (target != null)
                {
                    if (ctr == index)
                        return target;

                    target = target.next;
                    ctr++;
                }
                return target;
            }

            public long IndexOf(ref MatchedText? value)
            {
                if (value == null) return -1;
                if (__count == 0) return -1;
                MatchedText? target = head;

                long ctr = 0;
                while (target != null) 
                {
                    if (target == value)
                        return ctr;

                    target = target.next;
                    ctr++;
                }
                // node not found, return error
                return -1;
            }

            public MatchedText? Goto(long index, bool setCurrent)
            {
                if (__count == 0) return null;
                if ((index < 0) || (index >= __count)) return null;
                
                MatchedText? target = ElementAt(index);

                if (setCurrent) current = target;
                return target;
            }
            public void Replace(ref MatchedText? target, ref MatchedText? value)
            {
                target.match = value.match;
                target.currentRunPointer = value.currentRunPointer;
                target.start = value.start;
                target.end = value.end;
            }

            public bool RemoveAt(long index)
            {
                if (__count == 0) return false;
                if (index >= __count) return false;
                MatchedText? node = ElementAt(index);
                if (node == null) return false;

                Remove(ref node);
                return true;
            }
            // primary remove function
            public void Remove(ref MatchedText value)
            {
                // configure previous and next nodes
                MatchedText? prev = value.previous;
                MatchedText? next = value.next;
                if (prev != null) prev.next = next;
                if (next != null) next.previous = prev;

                // if previous node is null, means header.
                if (value == head)
                {
                    // meaning this node is head, so set next node as header because this node is removed.
                    if (next != null)
                        head = next;

                }
                else if (value == tail)
                {
                    // meaning this node is tail, so set the next node as tail because this node is removed.
                    if (next != null)
                        tail = next;
                }

                // now decrement the counter and reconfigure head and tail
                __count--;
                if (__count == 0)
                {
                    // no node left, reset all
                    head = tail = current = null;
                }
                else if (__count == 1)
                {
                    // only head left, remove tail
                    tail = null;
                }
                else if (__count == 2)
                {
                    // both head and tail left, nop
                }
                else
                {
                    // more than 2 nodes in chain, nothing to do.
                }

                //if (prev == null)
                //{
                // meaning this node is head, so set next node as header because this node is removed.
                //   head = next;
                // }
                /*
                if (next == null)
                {
                    // meaning this node is either head or tail
                    if (Count == 2)
                    {
                        // previous node is not null, this node is removed, and next node is null,
                        // so previous node becomes head
                        head = prev;
                        tail = null;
                    }
                    else if (Count >= 3) 
                    {

                    }
                }
                */

            }

            public MatchedText? Previous(bool setCurrent)
            {
                if (__count == 0) return null;
                if (current == null) return null;

                MatchedText? target = current;
                target = target.previous;

                if (setCurrent) current = target;
                return target;
            }
            public MatchedText? Next(bool setCurrent)
            {
                if (__count == 0) return null;

                MatchedText? target = current;
                if (target == null)
                    target = head;
                else
                    target = target.next;

                if (setCurrent) current = target;
                return target;
            }
            public MatchedText? First()
            {
                return head;
            }
            public MatchedText? Last()
            {
                return tail;
            }
            public void reset()
            {
                head = tail = current = null;
                __count= 0;
            }

            public void resetTo0()
            {
                current = null;
            }
            public void resetToLast()
            {
                current = Last();
            }

            public long Count
            {
                get
                {
                    return __count;
                }
            }

        }
        public class MatchedText
        {
            public TextPointer start;
            public TextPointer end;
            public TextPointer currentRunPointer;
            public Match? match;
            public MatchedText? previous;
            public MatchedText? next;
            public long index = 0;

            public MatchedText() { }

            public MatchedText(ref TextPointer start, ref TextPointer end, ref TextPointer currentRunPointer, ref Match? match)
            {
                this.start = start;
                this.end = end;
                this.currentRunPointer = currentRunPointer;
                this.match = match;
            }
        }
    }
}
