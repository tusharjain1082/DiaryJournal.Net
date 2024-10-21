namespace AngleSharp.Html.Dom
{
    using AngleSharp.Dom;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents an HTML slot element.
    /// </summary>
    sealed class HtmlSlotElement : HtmlElement, IHtmlSlotElement
    {
        #region ctor

        public HtmlSlotElement(Document owner, String? prefix = null)
            : base(owner, TagNames.Slot, prefix)
        {
        }

        #endregion

        #region Properties

        public String? Name
        {
            get => this.GetOwnAttribute(AttributeNames.Name);
            set => this.SetOwnAttribute(AttributeNames.Name, value);
        }

        #endregion

        #region Methods

        public IEnumerable<INode> GetDistributedNodes()
        {
            var host = this.GetAncestor<IShadowRoot>()?.Host;

            if (host != null)
            {
                var list = new List<INode>();

                foreach (var node in host.ChildNodes)
                {
                    if (Object.ReferenceEquals(GetAssignedSlot(node), this))
                    {
                        if (node is HtmlSlotElement otherSlot)
                        {
                            list.AddRange(otherSlot.GetDistributedNodes());
                        }
                        else
                        {
                            list.Add(node);
                        }
                    }
                }

                return list;
            }

            return Array.Empty<INode>();
        }

        #endregion

        #region Helpers

        private static IElement? GetAssignedSlot(INode node)
        {
            return node.NodeType switch
            {
                NodeType.Text    => ((IText)node).AssignedSlot,
                NodeType.Element => ((IElement)node).AssignedSlot,
                _                => default(IElement)
            };
        }

        #endregion
    }
}
