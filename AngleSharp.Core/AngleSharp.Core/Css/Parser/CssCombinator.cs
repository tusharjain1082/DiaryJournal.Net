namespace AngleSharp.Css.Parser
{
    using AngleSharp.Css;
    using AngleSharp.Css.Dom;
    using AngleSharp.Dom;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// An enumeration with possible CSS combinator values.
    /// </summary>
    abstract class CssCombinator
    {
        #region Fields

        /// <summary>
        /// The child operator (>).
        /// </summary>
        public static readonly CssCombinator Child = new ChildCombinator();

        /// <summary>
        /// The deep combinator (>>>).
        /// </summary>
        public static readonly CssCombinator Deep = new DeepCombinator();

        /// <summary>
        /// The descendant operator (space, or alternatively >>).
        /// </summary>
        public static readonly CssCombinator Descendant = new DescendantCombinator();

        /// <summary>
        /// The adjacent sibling combinator +.
        /// </summary>
        public static readonly CssCombinator AdjacentSibling = new AdjacentSiblingCombinator();

        /// <summary>
        /// The sibling combinator ~.
        /// </summary>
        public static readonly CssCombinator Sibling = new SiblingCombinator();

        /// <summary>
        /// The namespace combinator |.
        /// </summary>
        public static readonly CssCombinator Namespace = new NamespaceCombinator();

        /// <summary>
        /// The column combinator ||.
        /// </summary>
        public static readonly CssCombinator Column = new ColumnCombinator();

        #endregion

        #region Properties

        /// <summary>
        /// Gets the transformation function for the combinator.
        /// </summary>
        public Func<IElement, IEnumerable<IElement>>? Transform
        {
            get;
            protected set;
        }

        /// <summary>
        /// Gets the delimiter that represents the combinator.
        /// </summary>
        public String? Delimiter
        {
            get;
            protected set;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Changes the selector on the LHS according to some special rules.
        /// </summary>
        /// <param name="selector">The original selector.</param>
        /// <returns>The modified (or unmodified) selector.</returns>
        public virtual ISelector Change(ISelector selector) => selector;

        #endregion

        #region Helpers

        protected static IEnumerable<IElement> Single(IElement? element)
        {
            if (element != null)
            {
                yield return element;
            }
        }

        #endregion

        #region Classes

        private sealed class ChildCombinator : CssCombinator
        {
            public ChildCombinator()
            {
                Delimiter = CombinatorSymbols.Child;
                Transform = el => Single(el.ParentElement);
            }
        }

        private sealed class DeepCombinator : CssCombinator
        {
            public DeepCombinator()
            {
                Delimiter = CombinatorSymbols.Deep;
                Transform = el => Single(el.Parent is IShadowRoot shadowRoot ? shadowRoot.Host : null);
            }
        }

        private sealed class DescendantCombinator : CssCombinator
        {
            public DescendantCombinator()
            {
                Delimiter = CombinatorSymbols.Descendant;
                Transform = el =>
                {
                    var parents = new List<IElement>();
                    var parent = el.ParentElement;

                    while (parent != null)
                    {
                        parents.Add(parent);
                        parent = parent.ParentElement;
                    }

                    return parents;
                };
            }
        }

        private sealed class AdjacentSiblingCombinator : CssCombinator
        {
            public AdjacentSiblingCombinator()
            {
                Delimiter = CombinatorSymbols.Adjacent;
                Transform = el => Single(el.PreviousElementSibling);
            }
        }

        private sealed class SiblingCombinator : CssCombinator
        {
            public SiblingCombinator()
            {
                Delimiter = CombinatorSymbols.Sibling;
                Transform = el =>
                {
                    var parent = el.ParentElement;

                    if (parent != null)
                    {
                        var siblings = new List<IElement>();

                        foreach (var child in parent.ChildNodes)
                        {
                            if (child is IElement element)
                            {
                                if (Object.ReferenceEquals(element, el))
                                {
                                    break;
                                }

                                siblings.Add(element);
                            }
                        }

                        return siblings;
                    }

                    return Array.Empty<IElement>();
                };
            }
        }

        private sealed class NamespaceCombinator : CssCombinator
        {
            public NamespaceCombinator()
            {
                Delimiter = CombinatorSymbols.Pipe;
                Transform = Single;
            }

            public override ISelector Change(ISelector selector)
            {
                var prefix = selector switch
                {
                    TypeSelector typeSelector => typeSelector.TypeName,
                    _ => selector.Text
                };

                return new NamespaceSelector(prefix);
            }
        }

        private sealed class ColumnCombinator : CssCombinator
        {
            public ColumnCombinator()
            {
                Delimiter = CombinatorSymbols.Column;
                //TODO no real implementation yet
                //see: http://dev.w3.org/csswg/selectors-4/#the-column-combinator
            }
        }

        #endregion
    }
}
