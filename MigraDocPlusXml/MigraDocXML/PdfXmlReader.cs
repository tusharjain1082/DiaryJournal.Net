using EvalScript.Evaluating;
using MigraDocXML.DOM;
using MigraDocXML.Exceptions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace MigraDocXML
{
    public class PdfXmlReader
    {
        private string _designFile;
        public string DesignFile
        {
            get => _designFile;
            set
            {
                _designFile = value;
                _designText = null;
            }
        }

        private string _designText;
        public string DesignText
        {
            get => _designText;
            set
            {
                _designText = value;
                _designFile = null;
            }
        }

        public EvalScript.Runner ScriptRunner { get; set; }


        public PdfXmlReader(string designFile = null)
        {
            DesignFile = designFile;
        }


        public Document RunWithCsvFile(string csvFilePath)
        {
            return Run(EvalObject.PopulateFromCsv(System.IO.File.ReadAllText(csvFilePath)));
        }

        public Document RunWithJsonFile(string jsonFilePath)
        {
            return Run(EvalObject.PopulateFromJson(System.IO.File.ReadAllText(jsonFilePath)));
        }

        public Document RunWithXmlFile(string xmlFilePath)
        {
            return Run(new XmlEvalObject(XElement.Parse(System.IO.File.ReadAllText(xmlFilePath))));
        }

        public Document Run(object model = null)
        {
            if (DesignFile == null && DesignText == null)
                throw new Exception("No design specified");

            ScriptRunner = ScriptRunner ?? new EvalScript.Runner(new EvalScript.Interpreting.Interpreter(), new EvalScript.Evaluating.StandardEvaluator());
			ScriptRunner.Evaluator.Functions["GetArea"] = EvalScriptGraphicsFunctions.GetArea;
			ScriptRunner.Evaluator.Functions["GetAreas"] = EvalScriptGraphicsFunctions.GetAreas;
			ScriptRunner.Evaluator.Functions["GetTaggedAreas"] = EvalScriptGraphicsFunctions.GetTaggedAreas;
			ScriptRunner.Evaluator.Functions["GetAreasOfType"] = EvalScriptGraphicsFunctions.GetAreasOfType;
			ScriptRunner.Evaluator.Functions["GetPageCount"] = EvalScriptGraphicsFunctions.GetPageCount;
			ScriptRunner.Evaluator.Functions["LoadCsvFile"] = EvalScriptFileFunctions.LoadCsvFile;
			ScriptRunner.Evaluator.Functions["LoadJsonFile"] = EvalScriptFileFunctions.LoadJsonFile;
			ScriptRunner.Evaluator.Functions["LoadXmlFile"] = EvalScriptFileFunctions.LoadXmlFile;

			string xmlText = DesignText ?? System.IO.File.ReadAllText(DesignFile);
            xmlText = PreprocessXmlString(xmlText);

            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlText);
            var root = xmlDoc.DocumentElement;
            if (root.Name != "Document")
                throw new Exception("Invalid markup");

            Document document = null;
            try
            {
                document = new Document(model, ScriptRunner);
                document.SetXmlElement(root);
                if(DesignFile != null)
                {
                    document.ImagePath = System.IO.Path.GetDirectoryName(DesignFile);
                    document.ResourcePath = System.IO.Path.GetDirectoryName(DesignFile);
                }
                SetPropertiesFromAttributes(root, document);
                ReadChildElements(root, document);
            }
            catch (QuitException)
            {
                document?.DispatchDiscarded();
                document = null;
            }
            catch (BreakException) { }
            catch (ContinueException) { }
            catch (XmlStackTraceException xstex)
            {
                xstex.Stack.Add("Document");
                throw xstex.GenerateStackedException();
            }
            catch (Exception)
            {
                document?.DispatchDiscarded();
                throw;
            }
            return document;
        }



        private void ReadChildElements(XmlElement xml, DOMElement dom)
        {
            Type parentType = dom.GetType();

			Dictionary<string, int> childNameCounts = new Dictionary<string, int>();
            
            foreach (var childElement in xml.ChildNodes.OfType<XmlNode>())
            {
				string childName = childElement.Name;
				try
                {
                    DOMElement child = null;

                    if (childElement is XmlComment)
                        continue;

                    //If the child element is just text, try to load it into the parent object using its 'SetTextValue' method
                    if (childElement is XmlText)
                        dom.SetTextValue(InterpretString(childElement.InnerText, dom)?.ToString());

                    else if (childElement is XmlElement)
                    {
                        if (childName == "Resource")
                            continue;
                        child = (DOMElement)Activator.CreateInstance(DOMTypes.Lookup(childName));

                        //Set the style property to the object now, since when the child gets added to its parent, the styling is applied
                        if (!(child is Style || child is Setters))
                        {
                            XmlAttribute styleAttr = childElement.Attributes.OfType<XmlAttribute>().FirstOrDefault(x => x.Name == "Style");
                            if (styleAttr != null)
                                child.Style = InterpretString(styleAttr.Value, dom.Children.LastOrDefault() ?? dom)?.ToString();
                        }

						if (childNameCounts.ContainsKey(childName))
							childNameCounts[childName] = childNameCounts[childName] + 1;
						else
							childNameCounts[childName] = 0;

                        dom.AddChild(child);

                        //Use each attribute to access properties on the object
                        SetPropertiesFromAttributes(childElement as XmlElement, child);

                        if (child is Insert)
                        {
                            var insert = child as Insert;
                            insert.Run(() => ReadChildElements(insert.ResourceXmlElement, dom));
                        }
                        else if (child is LogicalElement)
                        {
                            var logicalChild = child as LogicalElement;
                            logicalChild.Run(() => ReadChildElements(childElement as XmlElement, child));
                        }
                        else
                            ReadChildElements(childElement as XmlElement, child);

                        child.DispatchFullyBuilt();
                    }
                }
                catch(XmlStackTraceException xstex)
                {
					string stackString = childName;
					if (childNameCounts.ContainsKey(childName) && childNameCounts[childName] > 0)
						stackString += "[" + childNameCounts[childName] + "]";
                    xstex.Stack.Add(stackString);
                    throw;
                }
                catch(QuitException qex)
                {
                    throw;
                }
                catch(ContinueException cex)
                {
                    throw;
                }
                catch(BreakException bex)
                {
                    throw;
                }
                catch(Exception ex)
                {
                    var xstex = new XmlStackTraceException(ex);
					string stackString = childName;
					if (childNameCounts.ContainsKey(childName) && childNameCounts[childName] > 0)
						stackString += "[" + childNameCounts[childName] + "]";
					xstex.Stack.Add(stackString);
                    throw xstex;
                }
            }
        }



        private void SetPropertiesFromAttributes(XmlElement xml, DOMElement dom)
        {
            //Get the element attributes in the order they should be processed
            List<XmlAttribute> attributes = dom.ArrangeAttributes(xml.Attributes.OfType<XmlAttribute>());

            //Use each attribute to access properties on the dom object
            foreach(var attribute in attributes)
            {
                //If the node is of a logical type, don't interpret the attribute string, just store it as it is for the time being
                //Also, ensure the use of surrounding curly braces is optional
                if (dom is Setters)
                    SetPropertyFromAttribute(dom, attribute.Name, attribute.Value);

                else if (dom.IsLogical && !(dom is Insert))
                {
                    var text = attribute.Value.Trim();
                    if (text.StartsWith("{") && text.EndsWith("}"))
                        text = text.Substring(1, text.Length - 2);
                    SetPropertyFromAttribute(dom, attribute.Name, text);
                }
                else
                    SetPropertyFromAttribute(dom, attribute.Name, InterpretString(attribute.Value, dom));
            }
        }


        internal static void SetPropertyFromAttribute(DOMElement element, string attributeName, object value)
        {
            object obj = element;
            List<string> props = attributeName.Split('.').Select(x => x.Trim()).ToList();

            //Get to the object actually being affected, for example, if the attributeName="Format.Font.Bold", this bit will work through "Format" & "Font" to get us to the Font object which is being edited
            Type type = null;
            PropertyInfo propInfo = null;
            for (int i = 0; i < props.Count - 1; i++)
            {
                type = obj.GetType();
                propInfo = type.GetProperty(props[i]);
                if (propInfo == null)
                {
                    element.SetUnknownAttribute(attributeName, value);
                    return;
                }

                obj = propInfo.GetValue(obj, null);
                if (obj == null)
                    return;
            }

            //Edit the target object
            type = obj.GetType();
            propInfo = type.GetProperty(props.Last());

			if (propInfo == null && obj is DOMElement)
				(obj as DOMElement).SetUnknownAttribute(props.Last(), value);

			else if (value == null)
				propInfo.SetValue(obj, value, null);

			else if (propInfo.PropertyType == typeof(Unit))
				propInfo.SetValue(obj, new Unit(value?.ToString()), null);

			else
			{
				//Before doing any conversions, check if the property type is a nullable
				var propType = propInfo.PropertyType;
				propType = Nullable.GetUnderlyingType(propType) ?? propType;

				//If working with a primitive property, first try converting the value to its type, falling back on passing in the value directly
				//Otherwise, first try passing in the value directly, falling back on doing a conversion to the property type
				try
				{
					if (propType.IsPrimitive)
						propInfo.SetValue(obj, Convert.ChangeType(value, propType, CultureInfo.InvariantCulture), null);
					else
						propInfo.SetValue(obj, value, null);
				}
				catch
				{
					if (propType.IsPrimitive)
						propInfo.SetValue(obj, value, null);
					else
						propInfo.SetValue(obj, Convert.ChangeType(value, propType, CultureInfo.InvariantCulture), null);
				}
			}
        }


        /// <summary>
        /// If the text contains any scripts, returns the evaluated results, otherwise returns the string unaffected
        /// </summary>
        /// <param name="text"></param>
        /// <param name="caller"></param>
        /// <returns></returns>
        internal static object InterpretString(string text, DOMElement caller)
        {
			return InterpretString(text, caller, true);
        }

		private static object InterpretString(string text, DOMElement caller, bool textCleanup)
		{
			if (textCleanup)
			{
				//Clean up whitespace, eg. remove all tabs at the start of lines and remove multiple spaces
				if (text.Contains(Environment.NewLine))
				{
					var textLines = text.Split(new[] { Environment.NewLine }, StringSplitOptions.None).ToList();
					if (string.IsNullOrWhiteSpace(textLines[0]))
						textLines.RemoveAt(0);
					if (string.IsNullOrWhiteSpace(textLines.Last()))
						textLines.RemoveAt(textLines.Count - 1);
					textLines = textLines.Select(x => x.TrimStart()).ToList();
					text = string.Join(Environment.NewLine, textLines);
				}
				int oldTextLength = text.Length;
				do
				{
					oldTextLength = text.Length;
					text = text.Replace("  ", " ");
				} while (oldTextLength > text.Length);
			}

			//Exclude any string sections surrounded by backticks from being executed as code
			int snippet1Index = text.IndexOf("```");
			int snippet2Index = -1;
			if (snippet1Index >= 0)
				snippet2Index = text.IndexOf("```", snippet1Index + 3);

			if (snippet1Index >= 0 && snippet2Index >= 0)
			{
				string leftText = text.Substring(0, snippet1Index);
				string snippetText = text.Substring(snippet1Index + 3, snippet2Index - snippet1Index - 3);
				string rightText = text.Substring(snippet2Index + 3);

				return InterpretString(leftText, caller, false) + snippetText + InterpretString(rightText, caller, false);
			}

			if (text.StartsWith("```") && text.EndsWith("```"))
				return text.Substring(3, text.Length - 6);

			//Process any code snippets within the text
			if (!text.Contains("{"))
				return text;

			int braceCount = text.Count(x => x == '{');
			if (braceCount != text.Count(x => x == '}'))
				throw new Exception($"Invalid script: {text}");

			object output = null;

			while (braceCount > 0)
			{
				int leftBraceIndex = text.LastIndexOf('{');
				int rightBraceIndex = text.IndexOf('}', leftBraceIndex);

				string braceText = text.Substring(leftBraceIndex + 1, rightBraceIndex - leftBraceIndex - 1).Trim();

				output = caller.GetDocument().ScriptRunner.Run(braceText, caller.GetVariable);
				if (output is string)
					output = (output as string).Replace("{", "__MigraDocXML.LeftBrace__").Replace("}", "__MigraDocXML.RightBrace__");

				if (leftBraceIndex == 0 && rightBraceIndex == text.Length - 1)
					break;
				else
				{
					text = text.Remove(leftBraceIndex, rightBraceIndex - leftBraceIndex + 1);
					text = text.Insert(leftBraceIndex, output?.ToString() ?? "");
					output = text;
				}

				braceCount--;
			}

			if (output is string)
				output = (output as string).Replace("__MigraDocXML.LeftBrace__", "{").Replace("__MigraDocXML.RightBrace__", "}");
			return output;
		}



        /// <summary>
        /// Run through the xml text manually escaping any &lt;, &gt; & &amp; symbols found within quotes or brackets
        /// This means the markup can contain these characters without resulting in exceptions when loading the XmlDocument
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        internal static string PreprocessXmlString(string str)
        {
            List<char> output = new List<char>();

            bool inQuotes = false;
            bool inCodeSnippet = false;
            int braceDepth = 0;
            for (int i = 0; i < str.Length; i++)
            {
                char chr = str[i];
                if (chr == '"')
                    inQuotes = !inQuotes;
                else if (chr == '{' && !inCodeSnippet)
                    braceDepth++;
                else if (chr == '}' && !inCodeSnippet)
                    braceDepth--;
                else if (chr == '`' && i + 2 < str.Length && str[i + 1] == '`' && str[i + 2] == '`')
                {
                    inCodeSnippet = !inCodeSnippet;
                    output.AddRange(new char[] { '`', '`', '`' });
                    i += 2;
                    continue;
                }

                if (!inQuotes && !inCodeSnippet && braceDepth == 0)
                    output.Add(chr);

                else
                {
                    bool insertDone = false;

                    if (chr == '<')
                    {
                        output.AddRange(new char[] { '&', 'l', 't', ';' });
                        insertDone = true;
                    }
                    else if (chr == '>')
                    {
                        output.AddRange(new char[] { '&', 'g', 't', ';' });
                        insertDone = true;
                    }
                    else if (chr == '&')
                    {
                        string subString = str.Substring(i, Math.Min(6, str.Length - i));
                        if (!subString.StartsWith("&quot;") && !subString.StartsWith("&apos;") &&
                            !subString.StartsWith("&lt;") && !subString.StartsWith("&gt;") &&
                            !subString.StartsWith("&amp;"))
                        {
                            output.AddRange(new char[] { '&', 'a', 'm', 'p', ';' });
                            insertDone = true;
                        }
                    }
                    if (!insertDone)
                        output.Add(chr);
                }
            }
            return new string(output.ToArray());
        }
    }
}
