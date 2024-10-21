using EvalScript.Evaluating;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace MigraDocXML
{
	public static class EvalScriptFileFunctions
	{
		public static List<EvalObject> LoadCsvFile(object[] args)
		{
			if (args.Length != 2 || !(args[0] is Document))
				throw new Exception("LoadCsvFile expects 2 arguments: document, relativePath");
			string path = System.IO.Path.Combine((args[0] as Document).ResourcePath, args[1].ToString());
			return EvalObject.PopulateFromCsv(System.IO.File.ReadAllText(path));
		}

		public static EvalObject LoadJsonFile(object[] args)
		{
			if (args.Length != 2 || !(args[0] is Document))
				throw new Exception("LoadJsonFile expects 2 arguments: document, relativePath");
			string path = System.IO.Path.Combine((args[0] as Document).ResourcePath, args[1].ToString());
			return EvalObject.PopulateFromJson(System.IO.File.ReadAllText(path));
		}

		public static XmlEvalObject LoadXmlFile(object[] args)
		{
			if (args.Length != 2 || !(args[0] is Document))
				throw new Exception("LoadXmlFile expects 2 arguments: document, relativePath");
			string path = System.IO.Path.Combine((args[0] as Document).ResourcePath, args[1].ToString());
			return new XmlEvalObject(XElement.Parse(System.IO.File.ReadAllText(path)));
		}
	}
}
