using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace MigraDocXML.DOM
{
    public class Style : LogicalElement
    {
        private string _target;
        public string Target
        {
            get => _target;
            set
            {
                _target = value;
                TargetPaths.Clear();
                if (_target == null)
                    return;

				foreach(string pathString in _target.Split('|').Select(x => x.Trim()))
				{
					var targetPath = new StyleTargetPath();

					//Interpret the target string to identify all wildcards & types contained within it
					foreach (string segment in pathString.Split('/').Select(x => x.Trim()))
					{
						if (segment == "*")
							targetPath.Add(new StyleTargetWildcard(-1));
						else if (segment == "_")
							targetPath.Add(new StyleTargetWildcard(1));
						else
						{
							string typeString = segment;
							List<string> styleStrings = null;
							if (segment.Count(x => x == '(') == 1 && segment.Count(x => x == ')') == 1 && segment.EndsWith(")"))
							{
								typeString = segment.Substring(0, segment.IndexOf('('));
								styleStrings = segment
									.Substring(typeString.Length)
									.Replace("(", "").Replace(")", "")
									.Trim()
									.Split(' ')
									.Select(x => x.Trim())
									.Where(x => !string.IsNullOrEmpty(x))
									.ToList();
							}
							targetPath.Add(new StyledType(DOMTypes.Lookup(typeString.Trim()), styleStrings));
						}
					}
					TargetPaths.Add(targetPath);
				}
            }
        }

        public List<StyleTargetPath> TargetPaths { get; private set; } = new List<StyleTargetPath>();

        public List<Type> TargetTypes
        {
            get
            {
				var styledTypes = TargetPaths.Select(x => x.Last()).OfType<StyledType>().ToList();
				if (styledTypes.Any(x => x == null))
					throw new Exception("Invalid style type");
				return styledTypes.Select(x => x.Type).ToList();
            }
        }


        public bool TestMatch(DOMElement obj)
        {
            if (obj is LogicalElement || TargetPaths.Count == 0)
                return false;

            string[] styleSplit = null;
            if (!string.IsNullOrEmpty(obj.Style))
                styleSplit = obj.Style.Split(' ').Select(x => x.Trim()).Where(x => !string.IsNullOrEmpty(x)).ToArray();

            //If this is a named style, return false if the object doesn't explicitly name it
            if (!string.IsNullOrEmpty(Name) && (styleSplit == null || !styleSplit.Contains(Name)))
                return false;

			var potentialStylePathMatches = TargetPaths.Where(x => (x.LastOrDefault() as StyledType)?.MatchesDOMElement(obj) == true).ToList();
			if (potentialStylePathMatches.Count == 0)
				return false;

            List<DOMElement> path = new List<DOMElement>();
            var parent = obj.GetParent();
            while (parent != null)
            {
                if (parent.IsPresentable)
                    path.Add(parent);
                parent = parent.GetParent();
            }
            path.Reverse();

            return potentialStylePathMatches.Any(x => RecursivePathMatch(path, path.Count - 1, x, x.Count - 2));
        }


        private bool RecursivePathMatch(List<DOMElement> objPath, int objPathIndex, StyleTargetPath targetPath, int targetPathIndex)
        {
            if (targetPathIndex < 0)
                return true;
            if (objPathIndex < 0)
                return false;

            DOMElement obj = objPath[objPathIndex];
            object target = targetPath[targetPathIndex];
            StyleTargetWildcard wildcard = target as StyleTargetWildcard;
            if(wildcard != null)
            {
                if(wildcard.Count > 0)
                {
                    if (RecursivePathMatch(objPath, objPathIndex - wildcard.Count, targetPath, targetPathIndex - 1))
                        return true;
                }
                else if(wildcard.Count < 0)
                {
                    for(int i = objPathIndex; i >= 0; i--)
                    {
                        if (RecursivePathMatch(objPath, i, targetPath, targetPathIndex - 1))
                            return true;
                    }
                }
            }
            else
            {
                StyledType targetType = target as StyledType;
                if(targetType != null)
					return targetType.MatchesDOMElement(obj) && RecursivePathMatch(objPath, objPathIndex - 1, targetPath, targetPathIndex - 1);
            }
            return false;
        }


        public override void Run(Action childProcessor)
        {
            childProcessor();
        }


        public Style()
        {
            NewVariable("Style", this);
        }



        public string Name { get; set; }

        public Setters Setters => Children.OfType<Setters>().FirstOrDefault();
    }



    public class StyleTargetPath : IEnumerable<object>
    {
        private List<object> _items = new List<object>();

        public void Add(StyleTargetWildcard wildcard) => _items.Add(wildcard);

        public void Add(StyledType type) => _items.Add(type);

        public void Clear() => _items.Clear();

        public object this[int index] => _items[index];

        public int Count => _items.Count;

        public IEnumerator<object> GetEnumerator() => _items.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _items.GetEnumerator();
    }



    public class StyleTargetWildcard
    {
        public int Count { get; private set; }

        public StyleTargetWildcard(int count)
        {
            Count = count;
        }
    }



    public class StyledType
    {
        public Type Type { get; private set; }

        public IEnumerable<string> Styles { get; private set; }

        public StyledType(Type type, IEnumerable<string> styles)
        {
            Type = type ?? throw new ArgumentNullException(nameof(type));
            Styles = styles?.ToList() ?? new List<string>();
        }

		/// <summary>
		/// Tests whether the passed in DOMElement's type matches the type this targets, and if the styling matches
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public bool MatchesDOMElement(DOMElement obj)
		{
			Type objType = obj.GetType();
			if((objType == Type || objType.IsSubclassOf(Type)))
			{
				if (Styles.Count() == 0)
					return true;
				if (string.IsNullOrWhiteSpace(obj.Style))
					return false;

				List<string> objStyles = (obj.Style ?? "").Split(' ').Select(x => x.Trim()).Where(x => !string.IsNullOrEmpty(x)).ToList();
				if (Styles.All(x => objStyles.Contains(x)))
					return true;
			}
			return false;
		}
    }
}
