namespace AngleSharp.Dom
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents the document type node.
    /// </summary>
    sealed class DocumentType : Node, IDocumentType
    {
        #region ctor

        /// <summary>
        /// Creates a new document type node.
        /// </summary>
        internal DocumentType(Document owner, String name)
            : base(owner, name, NodeType.DocumentType)
        {
            PublicIdentifier = String.Empty;
            SystemIdentifier = String.Empty;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the element immediately preceding in this node's parent's list
        /// of nodes,  null if the current element is the first element in that
        /// list.
        /// </summary>
        public IElement? PreviousElementSibling
        {
            get
            {
                var parent = Parent;

                if (parent != null)
                {
                    var found = false;

                    for (var i = parent.ChildNodes.Length - 1; i >= 0; i--)
                    {
                        if (Object.ReferenceEquals(parent.ChildNodes[i], this))
                        {
                            found = true;
                        }
                        else if (found && parent.ChildNodes[i] is IElement childEl)
                        {
                            return childEl;
                        }
                    }
                }

                return null;
            }
        }

        /// <summary>
        /// Gets the element immediately following in this node's parent's list
        /// of nodes, or null if the current element is the last element in
        /// that list.
        /// </summary>
        public IElement? NextElementSibling
        {
            get
            {
                var parent = Parent;

                if (parent != null)
                {
                    var n = parent.ChildNodes.Length;
                    var found = false;

                    for (var i = 0; i < n; i++)
                    {
                        if (Object.ReferenceEquals(parent.ChildNodes[i], this))
                        {
                            found = true;
                        }
                        else if (found && parent.ChildNodes[i] is IElement childEl)
                        {
                            return childEl;
                        }
                    }
                }

                return null;
            }
        }

        /// <summary>
        /// Gets a list of defined entities.
        /// </summary>
        public IEnumerable<Entity> Entities => Array.Empty<Entity>();

        /// <summary>
        /// Gets a list of defined notations.
        /// </summary>
        public IEnumerable<Notation> Notations => Array.Empty<Notation>();

        /// <summary>
        /// Gets or sets the name of the document type.
        /// </summary>
        public String Name => NodeName;

        /// <summary>
        /// Gets or sets the public ID of the document type.
        /// </summary>
        public String PublicIdentifier
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the system ID of the document type.
        /// </summary>
        public String SystemIdentifier
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the internal subset of the document type.
        /// </summary>
        public String? InternalSubset
        {
            get;
            set;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Inserts nodes before the current node.
        /// </summary>
        /// <param name="nodes">The nodes to insert before.</param>
        /// <returns>The current element.</returns>
        public void Before(params INode[] nodes) => this.InsertBefore(nodes);

        /// <summary>
        /// Inserts nodes after the current node.
        /// </summary>
        /// <param name="nodes">The nodes to insert after.</param>
        /// <returns>The current element.</returns>
        public void After(params INode[] nodes) => this.InsertAfter(nodes);

        /// <summary>
        /// Replaces the current node with the nodes.
        /// </summary>
        /// <param name="nodes">The nodes to replace.</param>
        public void Replace(params INode[] nodes) => this.ReplaceWith(nodes);

        /// <summary>
        /// Removes the current element from the parent.
        /// </summary>
        public void Remove() => this.RemoveFromParent();

        /// <inheritdoc />
        public override Node Clone(Document owner, Boolean deep)
        {
            var node = new DocumentType(owner, Name)
            {
                PublicIdentifier = PublicIdentifier,
                SystemIdentifier = SystemIdentifier,
                InternalSubset = InternalSubset
            };
            CloneNode(node, owner, deep);
            return node;
        }

        #endregion

        #region Helpers

        protected override String? LocateNamespace(String prefix) => null;

        protected override String? LocatePrefix(String namespaceUri) => null;

        #endregion
    }
}
